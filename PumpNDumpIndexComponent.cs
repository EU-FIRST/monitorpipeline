/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    PumpNDumpIndexComponent.cs
 *  Desc:    Computes pump'n'dump index
 *  Created: May-2013
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Text;
using System.IO;
using Latino;
using Latino.TextMining;
using Latino.Model;
using Latino.Workflows.TextMining;

namespace MonitorPipeline
{
    /* .-----------------------------------------------------------------------
       |
       |  Class PumpNDumpIndexComponent
       |
       '-----------------------------------------------------------------------
    */
    class PumpNDumpIndexComponent : DocumentProcessor
    {
        private static BowSpace mBowSpace;
        private static SvmBinaryClassifier<bool> mClassifier;

        public PumpNDumpIndexComponent() : base(typeof(PumpNDumpIndexComponent))
        {
            mBlockSelector = "TextBlock/Content";
        }

        static PumpNDumpIndexComponent()
        {
            Logger.GetLogger(typeof(PumpNDumpIndexComponent)).Info("PumpNDumpIndexComponent", "Loading model ...");
            string fileName = Utils.GetConfigValue("PumpNDumpModel", ".\\PumpNDumpModel.bin");
            using (BinarySerializer reader = new BinarySerializer(fileName, FileMode.Open))
            {
                mBowSpace = new BowSpace(reader);
                mClassifier = new SvmBinaryClassifier<bool>(reader);
            }
            Logger.GetLogger(typeof(PumpNDumpIndexComponent)).Info("PumpNDumpIndexComponent", "Done.");
        }

        public override void ProcessDocument(Document document)
        {
            string contentType = document.Features.GetFeatureValue("contentType");
            if (contentType != "Text") { return; }
            try
            {
                StringBuilder text = new StringBuilder(document.Name); 
                TextBlock[] blocks = document.GetAnnotatedBlocks(mBlockSelector);
                foreach (TextBlock block in blocks) { text.AppendLine(block.Text); }
                SparseVector<double> bow = mBowSpace.ProcessDocument(text.ToString());
                Prediction<bool> pred = mClassifier.Predict(bow);

            }
            catch (Exception e)
            {
                mLogger.Error("ProcessDocument", e);
            }
        }
    }
}
