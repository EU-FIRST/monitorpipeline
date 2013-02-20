using Latino;
using Latino.Workflows;
using Latino.Workflows.WebMining;
using Latino.Workflows.TextMining;
using Latino.Workflows.Semantics;
using Latino.Workflows.Persistance;

namespace MonitorPipeline
{
    class Program
    {
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
            const int NUM_PIPES = 4;
            ZeroMqReceiverComponent zmqRcv = new ZeroMqReceiverComponent();
            zmqRcv.DispatchPolicy = DispatchPolicy.BalanceLoadMax;
            for (int i = 0; i < NUM_PIPES; i++)
            {
                DocumentCategorizerComponent cc = new DocumentCategorizerComponent();
                cc.BlockSelector = "TextBlock/Content";
                DocumentFilterComponent dfc = new DocumentFilterComponent();
                dfc.OnFilterDocument += new DocumentFilterComponent.FilterDocumentHandler(Filter);                
                EntityRecognitionComponent erc = new EntityRecognitionComponent(Utils.GetConfigValue("EntityRecognitionOntologies", "."));
                erc.BlockSelector = "TextBlock/Content";
                //DocumentCorpusWriterComponent dcwc = new DocumentCorpusWriterComponent(null, null, @"C:\Work\MonitorPipeline\Html");
                BowComponent bowc = new BowComponent();
                zmqRcv.Subscribe(cc);
                cc.Subscribe(dfc);
                dfc.Subscribe(erc);
                //erc.Subscribe(dcwc);                
                erc.Subscribe(bowc);
            }
            zmqRcv.Start();
        }
    }
}