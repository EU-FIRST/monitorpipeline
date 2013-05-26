/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    PumpIndexComponent.cs
 *  Desc:    Computes pump index
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
       |  Class PumpIndexComponent
       |
       '-----------------------------------------------------------------------
    */
    class PumpIndexComponent : DocumentProcessor
    {
        private static BowSpace mBowSpace;
        private static SvmBinaryClassifier<int> mClassifier;
        private static double mAvgDistPos;
        private static double mAvgDistNeg;

        public PumpIndexComponent() : base(typeof(PumpIndexComponent))
        {
            mBlockSelector = "TextBlock/Content";
        }

        static PumpIndexComponent()
        {
            Logger.GetLogger(typeof(PumpIndexComponent)).Info("PumpIndexComponent", "Loading model ...");
            string fileName = Utils.GetConfigValue("PumpIndexModel", ".\\PumpIndexModel.bin");
            using (BinarySerializer reader = new BinarySerializer(fileName, FileMode.Open))
            {
                mBowSpace = new BowSpace(reader);
                mClassifier = new SvmBinaryClassifier<int>(reader);
                mAvgDistPos = reader.ReadDouble();
                mAvgDistNeg = reader.ReadDouble();
                //Console.WriteLine(mAvgDistPos);
                //Console.WriteLine(mAvgDistNeg);
            }
            Logger.GetLogger(typeof(PumpIndexComponent)).Info("PumpIndexComponent", "Done.");
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
                Prediction<int> p = mClassifier.Predict(bow);
                double nrmDist = p.BestScore / (2.0 * (p.BestClassLabel > 0.0 ? mAvgDistPos : mAvgDistNeg));
                document.Features.SetFeatureValue("pumpIndex", nrmDist.ToString());
            }
            catch (Exception e)
            {
                mLogger.Error("ProcessDocument", e);
            }
        }
    }
}
