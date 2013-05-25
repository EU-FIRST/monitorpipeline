/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    OccurrenceWriterComponent.cs
 *  Desc:    Entity occurrence writer component
 *  Created: May-2013
 *
 *  Author:  Petra Kralj Novak
 *
 ***************************************************************************/

using System;
using Latino.Workflows.TextMining;
using SemanticAnotation; 

namespace Latino.Workflows.Persistance
{
    public class OccurrenceWriterComponent : StreamDataConsumer
    {
        public OccurrenceWriterComponent() : base(typeof(OccurrenceWriterComponent))
        {
        }

        public static void Initialize(string connectionString)
        {
            ToDb.InitializeDatabase(connectionString);
        }

        protected override void ConsumeData(IDataProducer sender, object data)
        {
            DocumentCorpus c = (DocumentCorpus)data;
            foreach (Document doc in c.Documents)
            {
                short sentenceNum = 0, blockNum = 0;
                int tokensPerDocument = 0;
                //docNum++;
                string documentId = doc.Features.GetFeatureValue("guid");
                documentId = documentId.Replace("-", "");
                //doc.Features.SetFeatureValue("fullId", corpusId + "_" + documentId);             //add feature fullId for Achim

                string responseUrl = doc.Features.GetFeatureValue("responseUrl") ?? "";
                string urlKey = doc.Features.GetFeatureValue("urlKey") ?? "";
                string title = doc.Features.GetFeatureValue("title") ?? "";
                string domainName = doc.Features.GetFeatureValue("domainName") ?? "";

                //********************* date = pubdate if |pubDate-timeGet|<3 days
                string pubDate = doc.Features.GetFeatureValue("pubDate") ?? "";
                DateTime timeGet = DateTime.Parse(doc.Features.GetFeatureValue("time"));
                string date = timeGet.ToString("yyyy-MM-dd");
                try
                {

                    DateTime mPubDate = DateTime.Parse(pubDate);
                    if (DateTime.Compare(mPubDate, timeGet) < 0 && timeGet.Subtract(mPubDate).CompareTo(TimeSpan.FromDays(3)) < 0)
                    {
                        date = mPubDate.ToString("yyyy-MM-dd");
                    }
                }
                catch { } // supress errors

                //******************* Document to database
                double pumpDumpIndex = 0; // TODO
                bool isFinancial = doc.Features.GetFeatureValue("isFinancial") == "True";
                long docId = ToDb.DocumentToDb(title, date, pubDate, timeGet.ToString("yyyy-MM-dd HH:mm"), responseUrl, urlKey, domainName, isFinancial, pumpDumpIndex, documentId);

                //******************* occurrences

                string blockSelector = "TextBlock/Content";
                string rev = doc.Features.GetFeatureValue("rev");
                if (rev != "1")
                {
                    blockSelector = "TextBlock/Content/Unseen";
                }

                //******************** occurrence to database
                int documentNeg = 0, documentPoz = 0;

                foreach (TextBlock tb in doc.GetAnnotatedBlocks(blockSelector)) //"TextBlock/Content" if rev = "1", else "TextBlock/Content/Unseen"
                {
                    int tokensPerBlock = doc.GetAnnotatedBlocks("Token", tb.SpanStart, tb.SpanEnd).Length;
                    tokensPerDocument += tokensPerBlock;
                    int blockNeg = 0, blockPoz = 0;
                    blockNum++;

                    foreach (TextBlock s in doc.GetAnnotatedBlocks("Sentence", tb.SpanStart, tb.SpanEnd)) // *** sentence selector within TextBlock tb
                    {
                        int sentenceNeg = 0;
                        int sentencePoz = 0;
                        sentenceNum++;
                        int tokensPerSentence = doc.GetAnnotatedBlocks("Token", s.SpanStart, s.SpanEnd).Length;
                        // sentiment object
                        foreach (TextBlock so in doc.GetAnnotatedBlocks("SentimentObject", s.SpanStart, s.SpanEnd)) // *** SentimentObject selector within sentence s
                        {
                            Annotation annot = so.Annotation;
                            //     string gazUri = annot.Features.GetFeatureValue("gazetteerUri");
                            string instUri = annot.Features.GetFeatureValue("instanceUri");
                            //     string instClassUri = annot.Features.GetFeatureValue("instanceClassUri");
                            string term = so.Text; // takole pa dobis dejanski tekst...
                            //   Console.WriteLine("\n" + gazUri + " \t" + instUri + " \t" + instClassUri + " \t" + term);
                            long occId = ToDb.OccurrenceToDb(date, annot.SpanStart, annot.SpanEnd, sentenceNum, blockNum, docId, instUri);
                            ToDb.TermToDb(occId, term);
                        }

                        // sentiment word
                        foreach (TextBlock so in doc.GetAnnotatedBlocks("SentimentWord", s.SpanStart, s.SpanEnd)) // *** SentimentWord selector within sentence s
                        {
                            Annotation annot = so.Annotation;
                            string gazUri = annot.Features.GetFeatureValue("gazetteerUri");
                            string instUri = annot.Features.GetFeatureValue("instanceUri");
                            string instClassUri = annot.Features.GetFeatureValue("instanceClassUri");
                            string term = so.Text; // takole pa dobis dejanski tekst...
                            //   Console.WriteLine("\n" + gazUri + " \t" + instUri + " \t" + instClassUri + " \t" + term);
                            if (instClassUri.EndsWith("PositiveWord"))
                            {
                                sentencePoz++;
                                blockPoz++;
                                documentPoz++;
                            }
                            else if (instClassUri.EndsWith("NegativeWord"))
                            {
                                sentenceNeg++;
                                blockNeg++;
                                documentNeg++;
                            }
                            // Insert into SQL table SentimentWordOccurrence
                            ToDb.SentimentWordOccurrenceToDb(date, annot.SpanStart, annot.SpanEnd, sentenceNum, blockNum, docId, instUri);
                        }
                    }
                    // Insert into SQL table BlockSentiment
                    if (blockNeg != 0 || blockPoz != 0)
                    {
                        ToDb.BlockSentimentToDb(docId, blockNum, blockPoz, blockNeg, tokensPerBlock);
                    }
                } 
            }
        }

        public new void Dispose()
        {
            base.Dispose();
            try { ToDb.DatabaseConnectionClose(); }
            catch { }
        }
    }
}
