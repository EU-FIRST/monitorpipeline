using System;
using Latino;
using Latino.Workflows;
using Latino.Workflows.TextMining;

namespace MonitorPipeline
{
    public class TestComponent : StreamDataConsumer
    {
        public TestComponent() : base(typeof(TestComponent))
        {
        }

        protected override void ConsumeData(IDataProducer sender, object data)
        {
            Utils.ThrowException(!(data is DocumentCorpus) ? new ArgumentTypeException("data") : null);
            DocumentCorpus corpus = (DocumentCorpus)data;
            foreach (Document doc in corpus.Documents)
            {
                Console.WriteLine(doc.Name);
                int i = 0;
                string category;
                while ((category = doc.Features.GetFeatureValue("category" + ++i)) != null)
                {
                    Console.WriteLine(category);
                }
                Console.WriteLine();              
            }
        }
    }
}
