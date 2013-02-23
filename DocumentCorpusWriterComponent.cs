/*==========================================================================;
 *
 *  This file is part of ???
 *
 *  File:    DocumentCorpusWriterComponent.cs
 *  Desc:    Writes document metadata into database
 *  Created: Feb-2013
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Data.SqlClient;
using System.Data;
using Latino;
using Latino.Workflows;
using Latino.Workflows.TextMining;

namespace MonitorPipeline
{
    /* .-----------------------------------------------------------------------
       |
       |  Class DocumentCorpusWriterComponent
       |
       '-----------------------------------------------------------------------
    */
    public class DocumentCorpusWriterComponent : StreamDataConsumer
    {
        private SqlConnection mConnection;
        private static int BULK_COPY_TIMEOUT
            = Convert.ToInt32(Utils.GetConfigValue("DatabaseBulkCopyTimeout", "0"));

        public DocumentCorpusWriterComponent() : this(Utils.GetConfigValue("DocumentCorpusWriterConnectionString"))
        {
        }

        public DocumentCorpusWriterComponent(string dbConnectionString) : base(typeof(DocumentCorpusWriterComponent))
        {
            mConnection = new SqlConnection(dbConnectionString);
            mConnection.Open();
        }

        private DataTable CreateMetadataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("corpusId", typeof(Guid));
            table.Columns.Add("docId", typeof(Guid));
            table.Columns.Add("name", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("url", typeof(string));
            table.Columns.Add("time", typeof(string));
            table.Columns.Add("pubDate", typeof(string));
            table.Columns.Add("domain", typeof(string));
            table.Columns.Add("rev", typeof(int));
            table.Columns.Add("category", typeof(string));
            return table;
        }

        private static string GetCategoryFeature(Document doc)
        {
            int i = 0;
            string val = "";
            string category;
            while ((category = doc.Features.GetFeatureValue("category" + ++i)) != null)
            {
                val += category + ";";
            }
            return val.TrimEnd(';');
        }

        protected override void ConsumeData(IDataProducer sender, object data)
        {
            DocumentCorpus corpus = (DocumentCorpus)data;
            Guid corpusId = new Guid(corpus.Features.GetFeatureValue("guid"));
            DataTable docsTable = CreateMetadataTable();
            foreach (Document doc in corpus.Documents)
            {
                docsTable.Rows.Add(
                    corpusId,
                    new Guid(doc.Features.GetFeatureValue("guid")),
                    Utils.Truncate(doc.Name, 400),
                    Utils.Truncate(doc.Features.GetFeatureValue("description"), 400),
                    Utils.Truncate(doc.Features.GetFeatureValue("responseUrl"), 400),
                    Utils.Truncate(doc.Features.GetFeatureValue("time"), 26),
                    Utils.Truncate(doc.Features.GetFeatureValue("pubDate"), 100),
                    Utils.Truncate(doc.Features.GetFeatureValue("domainName"), 100),
                    Convert.ToInt32(doc.Features.GetFeatureValue("rev")),
                    GetCategoryFeature(doc)
                    );
            }
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(mConnection, SqlBulkCopyOptions.CheckConstraints, /*externalTransaction=*/null))
            {
                bulkCopy.BulkCopyTimeout = BULK_COPY_TIMEOUT;
                bulkCopy.DestinationTableName = "Documents";
                bulkCopy.WriteToServer(docsTable);
            }
        }

        // *** IDisposable interface implementation ***

        public new void Dispose()
        {
            base.Dispose();
            try { mConnection.Close(); }
            catch { }
        }
    }
}