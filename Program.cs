using System;
using Latino;
using Latino.Workflows;
using Latino.Workflows.WebMining;
using Latino.Workflows.TextMining;
using Latino.Workflows.Semantics;
using Latino.Workflows.Persistance;
using System.Configuration;

namespace MonitorPipeline
{
    class Program
    {
        private static string HTML_FOLDER
            = Utils.GetConfigValue("HtmlOutputFolder");
        private static string ONTOLOGY_FOLDER
            = Utils.GetConfigValue("OntologyFolder", ".");
        private static string ONTOLOGY_FOLDER_BYPASS
            = Utils.GetConfigValue("OntologyFolderBypass", ".");
        private static int NUM_PIPES
            = Convert.ToInt32(Utils.GetConfigValue("NumPipes", "1"));
        private static int NUM_PIPES_BYPASS
            = Convert.ToInt32(Utils.GetConfigValue("NumPipesBypass", "1"));
        private static string CONNECTION_STRING_OCCURRENCE
            = Utils.GetConfigValue("ConnectionStringOccurrence");

        private static bool Filter(Document doc, Logger logger)
        {
            int i = 0;
            string category;
            while ((category = doc.Features.GetFeatureValue("category" + ++i)) != null)
            {
                if (category.StartsWith("Business")) { return true; }
            }
            return false;
        }

        static void Main(string[] args)
        {            
            Logger logger = Logger.GetRootLogger();
            ZeroMqReceiverComponent zmqRcv = new ZeroMqReceiverComponent(delegate(string key) {
                if (key == "MessageSendAddress" || key == "ReceiveLoadBalancingAdress" || key == "FinishPublish") { return null; } // ignore these settings
                return ConfigurationManager.AppSettings.Get(key);
            });
            ZeroMqEmitterComponent zmqEmt = new ZeroMqEmitterComponent(delegate(string key) {
                if (key == "MessageReceiveAddress" || key == "SendLoadBalancingAddress" || key == "FinishReceive") { return null; } // ignore these settings
                return ConfigurationManager.AppSettings.Get(key);
            });
            PassOnComponent oldBranch = new PassOnComponent(); // first branch (goes to WP4)            
            oldBranch.DispatchPolicy = DispatchPolicy.BalanceLoadMax;
            PassOnComponent bypass = new PassOnComponent(); // second branch ("bypass", writes to DB)
            bypass.DispatchPolicy = DispatchPolicy.BalanceLoadMax;
            zmqRcv.Subscribe(oldBranch);
            zmqRcv.Subscribe(bypass);
            for (int i = 0; i < NUM_PIPES; i++)
            {
                DocumentFilterComponent rcv = new DocumentFilterComponent();
                rcv.OnFilterDocument += new DocumentFilterComponent.FilterDocumentHandler(delegate(Document doc, Logger log) {
                    Console.WriteLine("RCV " + doc.Name);
                    return true;
                });                
                DocumentCategorizerComponent cc = new DocumentCategorizerComponent();
                cc.BlockSelector = "TextBlock/Content";
                DocumentFilterComponent dfc = new DocumentFilterComponent();
                dfc.OnFilterDocument += new DocumentFilterComponent.FilterDocumentHandler(Filter);                
                EntityRecognitionComponent erc = new EntityRecognitionComponent(ONTOLOGY_FOLDER);
                erc.BlockSelector = "TextBlock/Content";
                DocumentFilterComponent snd = new DocumentFilterComponent();
                snd.OnFilterDocument += new DocumentFilterComponent.FilterDocumentHandler(delegate(Document doc, Logger log) {
                    Console.WriteLine("SND " + doc.Name + " [" + doc.Features.GetFeatureValue("fullId") + "]");
                    return true;
                });
                GenericStreamDataProcessor mkId = new GenericStreamDataProcessor();
                mkId.OnProcessData += new GenericStreamDataProcessor.ProcessDataHandler(delegate(IDataProducer sender, object data) {
                    DocumentCorpus c = (DocumentCorpus)data;
                    string corpusId = c.Features.GetFeatureValue("guid").Replace("-", "");
                    DateTime timeEnd = DateTime.Parse(c.Features.GetFeatureValue("timeEnd"));
                    foreach (Document d in c.Documents)
                    {
                        string docId = d.Features.GetFeatureValue("guid").Replace("-", "");
                        string fullId = timeEnd.ToString("HH_mm_ss_") + corpusId + "_" + docId;
                        d.Features.SetFeatureValue("fullId", fullId);
                    }
                    return data;
                });                
                oldBranch.Subscribe(rcv);
                rcv.Subscribe(cc);
                cc.Subscribe(dfc);
                dfc.Subscribe(erc);
                erc.Subscribe(mkId);
                mkId.Subscribe(snd);
                snd.Subscribe(zmqEmt);
            }
            OccurrenceWriterComponent.Initialize(CONNECTION_STRING_OCCURRENCE);
            for (int i = 0; i < NUM_PIPES_BYPASS; i++)
            {
                // create components
                EntityRecognitionComponent erc = new EntityRecognitionComponent(ONTOLOGY_FOLDER);
                erc.BlockSelector = "TextBlock/Content";
                OntologyCategorizerComponent occ = new OntologyCategorizerComponent();
                OccurrenceWriterComponent owc = new OccurrenceWriterComponent();
                // build branch
                bypass.Subscribe(erc);
                erc.Subscribe(occ);
                occ.Subscribe(owc);
            }
            zmqRcv.Start();
            logger.Info("Main", "The pipeline is running.");
        }
    }
}