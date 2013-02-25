/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    ZeroMqEmitterComponent.cs
 *  Desc:    ZeroMQ emitter component
 *  Created: Sep-2011
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System.Xml;
using System.IO;
using Latino.Workflows.TextMining;
using Messaging;
using System.Text;
using Latino;
using Latino.Workflows;

namespace MonitorPipeline
{
    /* .-----------------------------------------------------------------------
       |
       |  Class ZeroMqEmitter
       |
       '-----------------------------------------------------------------------
    */
    public class ZeroMqEmitterComponent : StreamDataConsumer
    {
        private Messenger mMessenger;

        public ZeroMqEmitterComponent() : base(typeof(ZeroMqEmitterComponent))
        {
            mMessenger = new Messenger(/*appSettingHandler=*/null);
        }

        public ZeroMqEmitterComponent(Messenger.AppSettingDelegate appSettingHandler) : base(typeof(ZeroMqEmitterComponent))
        {
            mMessenger = new Messenger(appSettingHandler);
        }

        protected override void ConsumeData(IDataProducer sender, object data)
        {
            Utils.ThrowException(!(data is DocumentCorpus) ? new ArgumentTypeException("data") : null);
            foreach (Document doc in ((DocumentCorpus)data).Documents)
            {
                StringWriter stringWriter;
                XmlWriterSettings xmlSettings = new XmlWriterSettings();
                xmlSettings.Indent = true;
                xmlSettings.NewLineOnAttributes = true;
                xmlSettings.CheckCharacters = false;
                XmlWriter writer = XmlWriter.Create(stringWriter = new StringWriter(), xmlSettings);
                doc.WriteGateXml(writer, /*writeTopElement=*/true, /*removeBoilerplate=*/true);
                writer.Close();
                // send message
                mMessenger.sendMessage(stringWriter.ToString());
            }
        }

        // *** IDisposable interface implementation ***

        public new void Dispose()
        {
            try
            {
                mMessenger.stopMessaging();
            }
            catch
            {
            }
        }
    }
}