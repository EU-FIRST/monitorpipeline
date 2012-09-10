using Latino;
using Latino.Workflows.WebMining;
using Latino.Workflows.TextMining;
using Latino.Workflows.Semantics;
using Latino.Workflows.Persistance;

namespace MonitorPipeline
{
    class Program
    {
        static void Main(string[] args)
        {
            ZeroMqReceiverComponent zmqRcv = new ZeroMqReceiverComponent();
            DocumentCategorizerComponent cc = new DocumentCategorizerComponent();
            DocumentFilterComponent dfc = new DocumentFilterComponent();
            dfc.OnFilterDocument += new DocumentFilterComponent.FilterDocumentHandler(delegate(Document doc, Logger logger) {
                int i = 0;
                string category;
                while ((category = doc.Features.GetFeatureValue("category" + ++i)) != null)
                {
                    if (category.StartsWith("Business")) { return true; }
                }
                return false;
            });
            cc.BlockSelector = "TextBlock/Content";
            //TestComponent tc = new TestComponent();
            EntityRecognitionComponent erc = new EntityRecognitionComponent(@"C:\Work\MonitorPipeline\Ontology");
            DocumentCorpusWriterComponent dcwc = new DocumentCorpusWriterComponent(null, null, @"C:\Work\MonitorPipeline\Html");
            zmqRcv.Subscribe(cc);
            cc.Subscribe(dfc);
            dfc.Subscribe(erc);
            erc.Subscribe(dcwc);
            zmqRcv.Start();
        }
    }
}