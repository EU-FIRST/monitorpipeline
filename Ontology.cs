using System;
using Latino.Workflows.Semantics;
using SemWeb;
using System.Data.SqlClient;


namespace SemanticAnotation
{



    public class Ontology : EntityRecognitionEngine
    {

        //
        const string RDF = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
        const string RDFS = "http://www.w3.org/2000/01/rdf-schema#";
        const string OWL = "http://www.w3.org/2002/07/owl#";
        const string XSD = "http://www.w3.org/2001/XMLSchema#";
        const string FIRST = "http://project-first.eu/ontology#";

        // relations
        static readonly Entity rdftype = RDF + "type";
        static readonly Entity rdfSubClassOf = RDFS + "subClassOf";
        static readonly Entity rdfslabel = RDFS + "label";

        //entities
        static readonly Entity hasGazetteer = FIRST + "identifiedBy";
        static readonly Entity hasTerm = FIRST + "term";

        private static Entity hasSettings = FIRST + "settings";


        public Ontology(string rdfFolderName)
        {
            ImportRdfFromFolder(rdfFolderName);
        }


        public void ToScreen(Entity Object, String lag/* = ""*/)
        {

            Console.WriteLine(lag + " " + GetLabel(Object)+" \t\t "+GetSettings(Object));
            foreach (Entity r in mRdfStore.SelectSubjects(rdfSubClassOf, Object))     //get entityUri
            {
                Entity subject = (Entity)r;
                ToScreen(subject, lag + " -");
            }            
        }

        public string GetLabel(Entity myEntity) {
            Literal entityLabel = null;
            foreach (Resource s in mRdfStore.SelectObjects(myEntity, rdfslabel))             //get entityLabel
            {
                entityLabel = (Literal)s;
            }
            if (entityLabel == null)
            {
                return "";
            }
            else
            {
                return entityLabel.Value;
            }
        }

        public string GetSettings(Entity myEntity)
        {
            Literal entitySettings = null;
            foreach (Resource s in mRdfStore.SelectObjects(myEntity, hasSettings))             //get entityLabel
            {
                entitySettings = (Literal)s;
            }
            if (entitySettings == null)
            {
                return "";
            }
            else
            {
                return entitySettings.Value;
            }
        }



        public void ToDb(string connectionLine)
        {
            Console.WriteLine("Ontology to database check/insert");
            //open connection to DB
            SqlConnection connection= new SqlConnection(connectionLine);
            connection.Open();
            
            foreach (Entity r in mRdfStore.SelectSubjects(hasGazetteer, null))     //get entityUri
            {
                Entity entityUri = (Entity)r;

                Literal entityLabel = null;
                foreach (Resource s in mRdfStore.SelectObjects(entityUri, rdfslabel))             //get entityLabel
                {
                    entityLabel = (Literal)s;                
                }
                foreach (Resource s in mRdfStore.SelectObjects(entityUri, rdftype))                 //get entityClass
                {                    
                    Entity classUri = (Entity)s;
                    foreach (Resource classLabel in mRdfStore.SelectObjects(classUri, rdfslabel))             //get entityLabel
                    {                 
                        ClassToDb(connection, classUri.Uri, ((Literal)classLabel).Value);
                    }
                    EntityToDb(connection, entityUri.Uri, entityLabel.Value, classUri.Uri, "");                    
                }

            }
            connection.Close();
        }




        const string InsertEntity = @"IF NOT EXISTS (SELECT * FROM entity WHERE entity_uri = @EntityUri) 
                                     BEGIN
                                        INSERT INTO entity (entity_uri, entity_label, flags, class_id)
                                        VALUES (@EntityUri, @EntityLabel, @Flags, (SELECT id from class where class_uri=@ClassUri));
                                     END;";
    

        public void EntityToDb(SqlConnection connection, string entityUri, string entityLabel, string classUri, string flags)
        {
            
                SqlCommand cmdIE = new SqlCommand(InsertEntity, connection);

                cmdIE.Parameters.Clear();
                cmdIE.Parameters.AddWithValue("@EntityUri", entityUri);
                cmdIE.Parameters.AddWithValue("@EntityLabel", entityLabel);
                cmdIE.Parameters.AddWithValue("@ClassUri", classUri);
                cmdIE.Parameters.AddWithValue("@Flags", flags);


                try
                {
                    cmdIE.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Error inserting entity : {0}", entityUri);
                    throw new Exception(e+ "\nError inserting entity : "+ entityUri);
                }
        }
        

        //const string InsertClass = @"INSERT INTO class(class_uri, class_label)
          //                           VALUES(@ClassUri,@ClassLabel);";


        private const string InsertClass = @"IF NOT EXISTS (SELECT * FROM class WHERE class_uri = @ClassUri) 
                                     BEGIN
                                        INSERT INTO class(class_uri, class_label)
                                        VALUES(@ClassUri,@ClassLabel);
                                     END";


        public void ClassToDb(SqlConnection connection, string classUri, string classLabel)
        {
            
            SqlCommand cmd = new SqlCommand(InsertClass, connection);            
            cmd.Parameters.AddWithValue("@ClassUri", classUri);
            cmd.Parameters.AddWithValue("@ClassLabel", classLabel);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error inserting entity : {0}", classUri);
                throw new Exception(e + "\nError inserting entity : " + classUri);
            }
            
        //    Console.WriteLine("Class inserted into DB:  " + ClassLabel + " \t\t" + ClassUri);
        }

    }

    
}
