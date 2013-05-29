using System;
using System.Data.SqlClient;


namespace SemanticAnotation
{

    public class ToDb
    {
        const string ClDocument = @"INSERT INTO document(title,  date,  pub_date,  time_get,  response_url,  url_key,  domain_name, is_financial, pump_dump_index, document_guid )
                                        VALUES ( @title,  @date,  @pubDate,  @timeGet,  @responseUrl,  @urlKey,  @domainName, @isFinancial, @pumpDumpIndex, @documentGuid );
                                        SELECT SCOPE_IDENTITY();";

        const string ClOccurrence = @"INSERT INTO occurrence(date, location_start, location_end, sentence_num, block_num, document_id, entity_id)
                                        VALUES (@date,@locationStart,@locationEnd,@sentenceNum,@blockNum,@document_id,(SELECT id from entity where entity_uri=@entityUri));
                                        SELECT SCOPE_IDENTITY();";

        const string ClSentimentWordOccurrence = @"INSERT INTO sentiment_word_occurrence(date, location_start, location_end, sentence_num, block_num, document_id, entity_id)
                                        VALUES (@date,@locationStart,@locationEnd,@sentenceNum,@blockNum,@document_id,(SELECT id from entity where entity_uri=@entityUri));
                                        SELECT SCOPE_IDENTITY();";

        const  string ClDocumentSentiment = @"INSERT INTO document_sentiment(document_id, positives, negatives, polarity, tokens)
                                                        VALUES (@document_id, @positives, @negatives, @polarity, @tokens);";

        const string ClBlockSentiment = @"INSERT INTO block_sentiment(document_id, block_num, positives, negatives, polarity, tokens)
                                                       VALUES (@document_id, @block_id, @positives, @negatives, @polarity, @tokens);";

        const string ClSentenceSentiment = @"INSERT INTO sentence_sentiment(document_id, block_num, sentence_num, positives, negatives, polarity, tokens)
                                                        VALUES (@document_id, @block_id, @sentence_id, @positives, @negatives, @polarity, @tokens);";

        const string ClTerm = @"INSERT INTO term(occurrence_id,  term)
                                     VALUES (@occurrence_id,  @term);";



        /// <summary>
        /// Inserts the data about the document document table in the semantics database.
        /// </summary>
        public static long DocumentToDb(SqlConnection connection, string title, string date, string pubDate, string timeGet, string responseUrl, string urlKey, string domainName, bool isFinancial, double pumpDumpIndex, Guid documentGuid)
        {
            using (SqlCommand cmd = new SqlCommand(null, connection))
            {
                cmd.CommandTimeout = 300;
                cmd.CommandText = ClDocument;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@title", Shorten(title, 1000));
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@pubDate", Shorten(pubDate, 100));
                cmd.Parameters.AddWithValue("@timeGet", timeGet);
                cmd.Parameters.AddWithValue("@responseUrl", Shorten(responseUrl, 1000));
                cmd.Parameters.AddWithValue("@urlKey", Shorten(urlKey, 1000));
                cmd.Parameters.AddWithValue("@domainName", Shorten(domainName, 255));

                cmd.Parameters.AddWithValue("@isFinancial", isFinancial);
                cmd.Parameters.AddWithValue("@pumpDumpIndex", pumpDumpIndex);
                cmd.Parameters.AddWithValue("@documentGuid", documentGuid);

                try
                {
                    long id = Decimal.ToInt32((Decimal)cmd.ExecuteScalar());  //Execute the command, get id of the inserted document
                    return id;
                }
                catch (SqlException ex)
                {
                    string errorMessage = String.Format("\nError inserting document: \r\nThe document: \n\t\tTitle:{0} \n\t\tDate:{1} \n\t\tUrlKey:{2}", title, date, urlKey);
                    throw new Exception(ex + errorMessage);
                }
            }
        }

        public static long OccurrenceToDb(SqlConnection connection, string date, int locationStart, int locationEnd, int sentenceNum, int blockNum, long document_id, string entityUri)
        {
            using (SqlCommand cmd = new SqlCommand(null, connection))
            {
                cmd.CommandTimeout = 300;
                cmd.CommandText = ClOccurrence;
                //  Console.WriteLine("\n -------------------"+entityUri);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@locationStart", locationStart);
                cmd.Parameters.AddWithValue("@locationEnd", locationEnd);
                cmd.Parameters.AddWithValue("@sentenceNum", sentenceNum);
                cmd.Parameters.AddWithValue("@blockNum", blockNum);
                cmd.Parameters.AddWithValue("@document_id", document_id);
                cmd.Parameters.AddWithValue("@entityUri", entityUri);

                try
                {
                    Decimal tmp = (Decimal)cmd.ExecuteScalar();
                    long id = Decimal.ToInt32(tmp);  //Execute the command, get id of the inserted document
                    return id;
                }
                catch (Exception ex)
                {
                    string errorMessage = String.Format("\nError inserting occurrence: \n\t\t DocumentId:{0} \n\t\tEntityUri:{1}", document_id, entityUri);
                    throw new Exception(ex + errorMessage);
                }
            }
        }


        public static long SentimentWordOccurrenceToDb(SqlConnection connection, string date, int locationStart, int locationEnd, int sentenceNum, int blockNum, long document_id, string entityUri)
        {
            using (SqlCommand cmd = new SqlCommand(null, connection))
            {
                cmd.CommandTimeout = 300;
                cmd.CommandText = ClSentimentWordOccurrence;
                //  Console.WriteLine("\n -------------------"+entityUri);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@locationStart", locationStart);
                cmd.Parameters.AddWithValue("@locationEnd", locationEnd);
                cmd.Parameters.AddWithValue("@sentenceNum", sentenceNum);
                cmd.Parameters.AddWithValue("@blockNum", blockNum);
                cmd.Parameters.AddWithValue("@document_id", document_id);
                cmd.Parameters.AddWithValue("@entityUri", entityUri);

                try
                {
                    Decimal tmp = (Decimal)cmd.ExecuteScalar();
                    long id = Decimal.ToInt32(tmp);  //Execute the command, get id of the inserted document
                    return id;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nError inserting sentiment word occurrence " + entityUri + "\n" + ex);
                    return 0;
                }
            }
        }


        /// <summary>
        /// Inserts the EntityOccurrence data into the Occurrence table in the Semantics database.
        /// </summary>
        public static void DocumentSentimentToDb(SqlConnection connection, long docId, int positives, int negatives, int noTokens)
        {
            using (SqlCommand cmd = new SqlCommand(null, connection))
            {
                cmd.CommandTimeout = 300;
                cmd.CommandText = ClDocumentSentiment;

                double polarity = 0;
                if (positives != 0 || negatives != 0)
                {
                    polarity = 1.0 * (positives - negatives) / (positives + negatives);
                }
                //  Console.WriteLine("\n -------------------"+entityUri);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@document_id", docId);
                cmd.Parameters.AddWithValue("@positives", positives);
                cmd.Parameters.AddWithValue("@negatives", negatives);
                cmd.Parameters.AddWithValue("@polarity", polarity);
                cmd.Parameters.AddWithValue("@tokens", noTokens);

                try
                {
                    cmd.ExecuteNonQuery(); //Execute the command
                }
                catch (Exception ex)
                {
                    string errorMessage = String.Format("\nError inserting document sentiment: \n\t\t DocumentId:{0}", docId);
                    throw new Exception(ex + errorMessage);
                }
            }
        }


        public static void BlockSentimentToDb(SqlConnection connection, long docId, short blockNum, int positives, int negatives, int noTokens)
        {
            using (SqlCommand cmd = new SqlCommand(null, connection))
            {
                cmd.CommandTimeout = 300;
                cmd.CommandText = ClBlockSentiment;

                double polarity = 1.0 * (positives - negatives) / (positives + negatives);
                //  Console.WriteLine("\n -------------------"+entityUri);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@document_id", docId);
                cmd.Parameters.AddWithValue("@block_id", blockNum);
                cmd.Parameters.AddWithValue("@positives", positives);
                cmd.Parameters.AddWithValue("@negatives", negatives);
                cmd.Parameters.AddWithValue("@polarity", polarity);
                cmd.Parameters.AddWithValue("@tokens", noTokens);

                try
                {
                    cmd.ExecuteNonQuery(); //Execute the command
                }
                catch (Exception ex)
                {
                    string errorMessage = String.Format("\nError inserting block sentiment: \n\t\t DocumentId:{0} \n\t\t BlockNum{1}", docId, blockNum);
                    throw new Exception(ex + errorMessage);
                }
            }
        }


        public static void SentenceSentimentToDb(SqlConnection connection, long docId, short blockNum, short sentenceNum, int positives, int negatives, int noTokens)
        {
            using (SqlCommand cmd = new SqlCommand(null, connection))
            {
                cmd.CommandTimeout = 300;
                cmd.CommandText = ClSentenceSentiment;

                double polarity = 1.0 * (positives - negatives) / (positives + negatives);
                //  Console.WriteLine("\n -------------------"+entityUri);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@document_id", docId);
                cmd.Parameters.AddWithValue("@block_id", blockNum);
                cmd.Parameters.AddWithValue("@sentence_id", sentenceNum);
                cmd.Parameters.AddWithValue("@positives", positives);
                cmd.Parameters.AddWithValue("@negatives", negatives);
                cmd.Parameters.AddWithValue("@polarity", polarity);
                cmd.Parameters.AddWithValue("@tokens", noTokens);

                cmd.ExecuteNonQuery();  //Execute the command
            }
        }


        /// <summary>
        /// Inserts the data about the actual term (string) from teh document
        /// </summary>
        public static void TermToDb(SqlConnection connection, long occurrenceId, string term)
        {
            using (SqlCommand cmd = new SqlCommand(null, connection))
            {
                cmd.CommandTimeout = 300;
                cmd.CommandText = ClTerm;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@occurrence_id", occurrenceId);
                cmd.Parameters.AddWithValue("@term", Shorten(term, 400));
                try
                {
                    cmd.ExecuteNonQuery();  //Execute the command
                }
                catch (Exception)
                {
                    Console.WriteLine("\nTerm insert exception : occurrence_id \t" + occurrenceId + " \t" + term);
                }
            }
        }

        //*****************************************************************************************************************************
        //*****************************************************************************************************************************
        //*****************************************  SELECT *
        /// <summary>
        /// Selects * from the table tableName
        /// </summary>
        public static void PrintSelectAll(SqlConnection connect, string tableName)
        {
            SqlCommand command = connect.CreateCommand();
            command.CommandText = "SELECT * from "+tableName;
            Console.WriteLine("\nSELECT * FROM {0}: \n",tableName);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string thisrow = "";
                for (int i = 0; i < reader.FieldCount; i++)
                    thisrow += reader.GetValue(i).ToString() + ",";
                Console.WriteLine(thisrow);
            }
        }

        //******************************************* shorten string to desired number of characters
        private static string Shorten(string value, int length)
        {
            if (value.Length > length)
            {
                return value.Substring(0, length - 5) + "...";
            }
            return value;
        }

    }
}
