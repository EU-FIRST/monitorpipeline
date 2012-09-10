/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    DocumentCategorizerComponent.cs
 *  Desc:    Document categorizer component
 *  Created: Sep-2012
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Latino;
using Latino.Model;
using Latino.TextMining;
using Latino.Workflows.TextMining;

namespace MonitorPipeline
{
    /* .-----------------------------------------------------------------------
       |
       |  Class DocumentCategorizerComponent
       |
       '-----------------------------------------------------------------------
    */
    class DocumentCategorizerComponent : DocumentProcessor
    {
        private static BowSpace mBowSpace
            = null;
        private static Dictionary<string, IModel<string>> mCategorizer
            = null;

        public DocumentCategorizerComponent() : base(typeof(DocumentCategorizerComponent))
        {
            mBlockSelector = "TextBlock";
        }

        static DocumentCategorizerComponent()
        {
            Logger.GetLogger(typeof(DocumentCategorizerComponent)).Info("CategorizerComponent", "Loading model ...");
            string fileName = Utils.GetConfigValue("CategorizationModel", ".\\CategorizationModel.bin");
            BinarySerializer binReader = new BinarySerializer(fileName, FileMode.Open);
            mBowSpace = new BowSpace(binReader);
            mBowSpace.CutLowWeightsPerc = 0.2;
            mCategorizer = Utils.LoadDictionary<string, IModel<string>>(binReader);
            binReader.Close();
            Logger.GetLogger(typeof(DocumentCategorizerComponent)).Info("CategorizerComponent", "Done.");
        }

        private static void GetPredictedCategories(string prefix, double thresh, SparseVector<double> vec, ArrayList<string> categories)
        {
            if (!mCategorizer.ContainsKey(prefix))
            {
                categories.Add(prefix.TrimEnd('/'));
                return;
            }
            IModel<string> classifier = mCategorizer[prefix];
            Prediction<string> p = classifier.Predict(vec);
            double maxSim = p.Count == 0 ? 0 : p.BestScore;
            foreach (KeyDat<double, string> item in p)
            {
                if (item.Key == 0) { break; }
                double score = item.Key / maxSim;
                if (score < thresh) { break; }
                GetPredictedCategories(prefix + item.Dat + '/', thresh, vec, categories);
            }
        }

        public override void ProcessDocument(Document document)
        {
            string contentType = document.Features.GetFeatureValue("contentType");
            if (contentType != "Text") { return; }
            try
            {
                StringBuilder text = new StringBuilder(document.Name); // *** document title used as part of content (make configurable?)
                TextBlock[] blocks = document.GetAnnotatedBlocks(mBlockSelector);
                foreach (TextBlock block in blocks) { text.AppendLine(block.Text); }
                SparseVector<double> docVec = mBowSpace.ProcessDocument(text.ToString());
                ArrayList<string> categories = new ArrayList<string>();
                GetPredictedCategories(/*prefix=*/"", /*thresh=*/0.9, docVec, categories); // *** threshold hardcoded (make configurable)
                int i = 0;
                foreach (string category in categories)
                {
                    string key = "category" + ++i;
                    document.Features.SetFeatureValue(key, category);
                }
            }
            catch (Exception e)
            {
                mLogger.Error("ProcessDocument", e);
            }
        }
    }
}
