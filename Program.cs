﻿using System;
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
            const int NUM_PIPES = 4;
            ZeroMqReceiverComponent zmqRcv = new ZeroMqReceiverComponent(delegate(string key) {
                if (key == "MessageSendAddress" || key == "ReceiveLoadBalancingAdress" || key == "FinishPublish") { return null; } // ignore these settings
                return ConfigurationManager.AppSettings.Get(key);
            });
            zmqRcv.DispatchPolicy = DispatchPolicy.BalanceLoadMax;
            ZeroMqEmitterComponent zmqEmt = new ZeroMqEmitterComponent(delegate(string key) {
                if (key == "MessageReceiveAddress" || key == "SendLoadBalancingAddress" || key == "FinishReceive") { return null; } // ignore these settings
                return ConfigurationManager.AppSettings.Get(key);
            });
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
                EntityRecognitionComponent erc = new EntityRecognitionComponent(Utils.GetConfigValue("EntityRecognitionOntologies", "."));
                erc.BlockSelector = "TextBlock/Content";
                //DocumentCorpusWriterComponent dcwc = new DocumentCorpusWriterComponent(null, null, HTML_FOLDER);
                DocumentFilterComponent snd = new DocumentFilterComponent();
                rcv.OnFilterDocument += new DocumentFilterComponent.FilterDocumentHandler(delegate(Document doc, Logger log) {
                    Console.WriteLine("SND " + doc.Name);
                    return true;
                });                
                zmqRcv.Subscribe(rcv);
                rcv.Subscribe(cc);
                cc.Subscribe(dfc);
                dfc.Subscribe(erc);
                //erc.Subscribe(dcwc);
                erc.Subscribe(snd);
                snd.Subscribe(zmqEmt);
            }
            zmqRcv.Start();
            logger.Info("Main", "The pipeline is running.");
        }
    }
}