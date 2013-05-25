/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    OntologyCategorizerComponent.cs
 *  Desc:    Ontology-based document categorizer component
 *  Created: May-2013
 *
 *  Author:  Petra Kralj Novak
 *
 ***************************************************************************/

using System;
using Latino.Workflows.TextMining;

namespace MonitorPipeline
{
    /* .-----------------------------------------------------------------------
       |
       |  Class OntologyCategorizerComponent
       |
       '-----------------------------------------------------------------------
    */
    class OntologyCategorizerComponent : DocumentProcessor
    {
        public OntologyCategorizerComponent() : base(typeof(OntologyCategorizerComponent))
        {
        }

        public override void ProcessDocument(Document document)
        {
            string contentType = document.Features.GetFeatureValue("contentType");
            if (contentType != "Text") { return; }
            try
            {
                //******************* Ontology-based check for financial documents
                Boolean isFinancial = false;                                     // for setting the feature isFinancial in of the document
                foreach (TextBlock s in document.GetAnnotatedBlocks("SentimentObject"))
                {
                    if (!s.Annotation.Type.StartsWith("SentimentObject/GeographicalRegion"))
                    {
                        isFinancial = true;
                        //Console.WriteLine("\nFinancial document: " + s.Annotation.Features.GetFeatureValue("instanceUri"));
                        break;
                    }
                }
                document.Features.SetFeatureValue("isFinancial", isFinancial.ToString());    //add feature isFinancial
            }
            catch (Exception e)
            {
                mLogger.Error("ProcessDocument", e);
            }
        }
    }
}
