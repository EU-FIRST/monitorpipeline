using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading;
using Latino.Workflows;
using Latino.Workflows.TextMining;
using Messaging;

namespace MonitorPipeline
{
    public class ZeroMqReceiverComponent : StreamDataProducer
    {
        private Thread mThread
            = null;
        private bool mStopped
            = true;
        private Messenger mMessenger
            = new Messenger();

        public ZeroMqReceiverComponent() : base(typeof(ZeroMqReceiverComponent))
        {
        }

        public override void Start()
        {
            if (!IsRunning)
            {
                mThread = new Thread(new ThreadStart(
                    delegate() {
                        while (!mStopped && !mMessenger.isMessagingFinished())
                        {
                            string message = mMessenger.getMessage();
                            if (message != null)
                            {
                                try
                                {
                                    XmlReaderSettings settings = new XmlReaderSettings();
                                    settings.CheckCharacters = false;
                                    DocumentCorpus dc = new DocumentCorpus();
                                    XmlReader reader = XmlReader.Create(new StringReader(message), settings);
                                    dc.ReadXml(reader);
                                    reader.Close();
                                    DispatchData(dc);
                                }
                                catch (Exception e)
                                {
                                    mLogger.Error("ZeroMqReceiverComponent", e);
                                    //File.WriteAllText(@"C:\Users\Administrator\Desktop\err\" + Guid.NewGuid().ToString("N") + ".xml", message, Encoding.UTF8);
                                }
                            }
                            Thread.Sleep(1);
                        }
                    }
                ));
                mStopped = false;
                mThread.Start();
            }
        }

        public override void Stop()
        {
            mStopped = true;
        }

        public override bool IsRunning
        {
            get { return mThread != null && mThread.IsAlive; }
        }

        public override void Dispose()
        {
            Stop();
            while (IsRunning) { Thread.Sleep(100); }
        }
    }
}
