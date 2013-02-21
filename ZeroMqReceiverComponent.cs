/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    ZeroMqReceiverComponent.cs
 *  Desc:    ZeroMQ receiver component
 *  Created: Sep-2012
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.IO;
using System.Xml;
using System.Threading;
using Latino.Workflows;
using Latino.Workflows.TextMining;
using Messaging;

namespace MonitorPipeline
{
    /* .-----------------------------------------------------------------------
       |
       |  Class ZeroMqReceiverComponent
       |
       '-----------------------------------------------------------------------
    */
    public class ZeroMqReceiverComponent : StreamDataProducer
    {
        private Thread mThread
            = null;
        private bool mStopped
            = true;
        private Messenger mMessenger;

        public ZeroMqReceiverComponent() : base(typeof(ZeroMqReceiverComponent))
        {
            mMessenger = new Messenger(/*appSettingHandler=*/null);
        }

        public ZeroMqReceiverComponent(Messenger.AppSettingDelegate appSettingHandler) : base(typeof(ZeroMqReceiverComponent))
        {
            mMessenger = new Messenger(appSettingHandler);
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
                                    DocumentCorpus dc = new DocumentCorpus();
                                    XmlReader reader = new XmlTextReader(new StringReader(message));
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
