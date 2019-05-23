using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Win32;



/// <summary>
/// This is the C# script of the Multiterm to TBX converter. It accepts a JSON mapping file and an XML TBX file and returns a properly formatted TBX file as its output.
/// 
/// Currently the program is functional, and has no known current errors 
/// 
/// Known limitations:
/// 
/// ** Should any elements changed by the mapping file require being placed as a child of a new element, the program only knows how to place adminNotes in adminGrps
/// 
/// </summary>



namespace MultiTermTBXMapper
{
    // This holds teasps that have one or more value groups. 

    public class ExtendedTeaspStorageManager
    {
        public List<string[]> ValueGroupCollection { get; set; } // Each string[] will correspond with the teasp in the same position
        public List<object> CorrespondingValGrpTeasps { get; set; } // Every one of these will have substitutions, and therefore the default will not be in this list
        public TeaspNoSubstitution DefaultTeaspSub { get; set; }

        public ExtendedTeaspStorageManager(List<string[]> ls, List<object> lt, TeaspNoSubstitution dt)
        {
            ValueGroupCollection = ls;
            CorrespondingValGrpTeasps = lt;
            DefaultTeaspSub = dt;
        }

        public List<string[]> GetValueGroupCollection()
        {
            return ValueGroupCollection;
        }

        public List<object> GetCorrespondingValGrpTeasps()
        {
            return CorrespondingValGrpTeasps;
        }

        public TeaspNoSubstitution GetDefaultTeaspSub()
        {
            return DefaultTeaspSub;
        }
    }

    // This is the vanilla teasp template. It takes an array of strings as its constructor and stores the appropriate values, although it currently does not account for the possibility of a 
    // dictionary-like object for a substitution.

    public class TeaspNoSubstitution
    {
        public string Target { get; set; }
        public string ElementOrAttributes { get; set; }
        public string Substitution { get; set; }
        public string Placement { get; set; }

        public TeaspNoSubstitution(string t, string ea, string s, string p)
        {
            Target = t;
            ElementOrAttributes = ea;
            Substitution = s;
            Placement = p;
        }

        public string GetTarget()
        {
            return Target;
        }

        public string GetElementOrAttribute()
        {
            return ElementOrAttributes;
        }

        public string GetSubstitution()
        {
            return Substitution;
        }

        public string GetPlacement()
        {
            return Placement;
        }

    }

    // This holds teasps that have no value groups, but do have substitutions

    public class TeaspWithSubstitution
    {
        public string Target { get; set; }
        public string ElementOrAttributes { get; set; }
        public Dictionary<string, string> Substitution { get; set; } = new Dictionary<string, string>();
        public string Placement { get; set; }

        public TeaspWithSubstitution(string t, string ea, Dictionary<string, string> s, string p)
        {
            Target = t;
            ElementOrAttributes = ea;
            foreach (KeyValuePair<string, string> entry in s)
            {
                Substitution.Add(entry.Key, entry.Value);
            }
            Placement = p;
        }

        public string GetTarget()
        {
            return Target;
        }

        public string GetElementOrAttribute()
        {
            return ElementOrAttributes;
        }

        public Dictionary<string, string> GetSubstitution()
        {
            return Substitution;
        }

        public string GetPlacement()
        {
            return Placement;
        }

    }

    // The template set is constructed by seperating all of the template-set jObjects from the dictionaries, and stored in individual Lists. Each list is then parsed and seperated into 
    // the default teasp and value-groups, followed by the subsequent teasps.

    public class TemplateSetApp
    {
        public List<object> ConceptMappingTemplates { get; set; } = new List<object>();
        public List<string> ConceptMappingTemplatesKeys { get; set; } = new List<string>();
        public List<object> LanguageMappingTemplates { get; set; } = new List<object>();
        public List<string> LanguageMappingTemplatesKeys { get; set; } = new List<string>();
        public List<object> TermMappingTemplates { get; set; } = new List<object>();
        public List<string> TermMappingTemplatesKeys { get; set; } = new List<string>();
        public object[] CastObjArray { get; set; }
        public string Key { get; set; }
        public TeaspNoSubstitution TeaspNS { get; set; }
        public TeaspWithSubstitution TeaspWS { get; set; }
        public int Handler { get; set; } = 0;
        public int KeyCounter { get; set; } = 0;
        public int TemplateKeyTracker { get; set; } = 0;
        public string[] ValGrp { get; set; }

        // // // //

        // This is the Dicitonary that will contain the Mapping Templates Strings and an object (Either a plain teasp or a extendedTeaspStorageManager object).
        // Regardless of what kind of object each Key-Value pair has, type will be determined at runtime and processing will be done then.

        public Dictionary<string, object> GrandMasterDictionary { get; set; } = new Dictionary<string, object>();

        // // // //

        public string T { get; set; }
        public string EA { get; set; }
        // Declare s at runtime
        public string P { get; set; }

        public TemplateSetApp(Dictionary<string, object> c, Dictionary<string, object> l, Dictionary<string, object> t)
        {
            foreach (var entry in c)
            {
                object tempUNK1 = entry.Value;
                ConceptMappingTemplates.Add(tempUNK1);

                Key = entry.Key;
                ConceptMappingTemplatesKeys.Add(Key);
            }
            foreach (var entry2 in l)
            {
                object tempUNK2 = entry2.Value;
                LanguageMappingTemplates.Add(tempUNK2);

                Key = entry2.Key;
                LanguageMappingTemplatesKeys.Add(Key);
            }
            foreach (var entry3 in t)
            {
                object tempUNK3 = entry3.Value;
                TermMappingTemplates.Add(tempUNK3);

                Key = entry3.Key;
                TermMappingTemplatesKeys.Add(Key);
            }

            ConvertTemplateSets();
        }

        public Dictionary<string, object> GetGrandMasterDictionary()
        {
            return GrandMasterDictionary;
        }

        private string KeySelector()
        {
            string returnKey = "";

            if (TemplateKeyTracker == 1)
            {
                returnKey = ConceptMappingTemplatesKeys[KeyCounter];
            }
            else if (TemplateKeyTracker == 2)
            {
                returnKey = LanguageMappingTemplatesKeys[KeyCounter];
            }
            else if (TemplateKeyTracker == 3)
            {
                returnKey = TermMappingTemplatesKeys[KeyCounter];
            }
            else if (TemplateKeyTracker == -1)
            {
                throw new Exception("ERROR! Invalid Dictionary Key");
            }

            return returnKey;
        }

        private void AddToMasterDictionaryAfterHandle(JArray plainTeasp)
        {
            if (Handler == 0)
            {
                Dictionary<string, string> exceptionSub = (Dictionary<string, string>)plainTeasp[2].ToObject(typeof(Dictionary<string, string>));
                var teaspMy = new TeaspWithSubstitution(T, EA, exceptionSub, P);

                GrandMasterDictionary.Add(KeySelector(), teaspMy);
            }
            else if (Handler == 1)
            {
                string s = (string)plainTeasp[2].ToObject(typeof(string));
                var teaspMy = new TeaspNoSubstitution(T, EA, s, P);

                GrandMasterDictionary.Add(KeySelector(), teaspMy);
            }
        }

        private void HandlePlainTemplateSet()
        {
            JArray tempJA = (JArray)CastObjArray[0];
            object[] teasp = (object[])tempJA.ToObject(typeof(object[]));
            JArray plainTeasp = (JArray)teasp[0];

            T = (string)plainTeasp[0].ToObject(typeof(string));
            EA = (string)plainTeasp[1].ToObject(typeof(string));
            P = (string)plainTeasp[3].ToObject(typeof(string));

            // // //
            JToken castTest = (JToken)plainTeasp[2].ToObject(typeof(JToken));
            string castTestString = "";

            try
            {
                castTestString = (string)castTest.ToObject(typeof(string));
            }
            catch (Exception)
            {
                Handler = 0;
            }

            try
            {
                Dictionary<string, string> castTestDictionary = (Dictionary<string, string>)castTest.ToObject(typeof(Dictionary<string, string>));
            }
            catch (Exception)
            {
                Handler = 1;
            }

            AddToMasterDictionaryAfterHandle(plainTeasp);
        }

        private List<Object> AddToLocalList(List<Object> lt0, JArray tsp, string s)
        {
            if (Handler == 0)
            {
                Dictionary<string, string> exceptionSub = (Dictionary<string, string>)tsp[2].ToObject(typeof(Dictionary<string, string>));
                var teaspMy = new TeaspWithSubstitution(T, EA, exceptionSub, P);

                lt0.Add(teaspMy);
                return lt0;
            }
            else if (Handler == 1)
            {
                string str = (string)tsp[2].ToObject(typeof(string));
                var teaspMy = new TeaspNoSubstitution(T, EA, s, P);

                lt0.Add(teaspMy);
                return lt0;
            }

            return null;
        }

        private void PrepareContentForTemplateWithValueGroups(int identifierForLevel)
        {
            List<string[]> ls0 = new List<string[]>();
            List<object> lt0 = new List<object>();
            TeaspNoSubstitution defTeasp0;

            JArray temp = (JArray)CastObjArray[0]; // This will have the default Teasp and subsequent value groups
            object[] deftsp = (object[])temp.ToObject(typeof(object[])); // Grab the default teasp
            JArray defaultTsp = (JArray)deftsp[0];

            T = (string)defaultTsp[0].ToObject(typeof(string));
            EA = (string)defaultTsp[1].ToObject(typeof(string));
            string s = (string)defaultTsp[2].ToObject(typeof(string));
            P = (string)defaultTsp[3].ToObject(typeof(string));

            TeaspNS = new TeaspNoSubstitution(T, EA, s, P); // This is now ready to give to the extendedTeaspStorageManager
            defTeasp0 = TeaspNS;

            deftsp = deftsp.Skip(1).ToArray(); // We dont want the first array, it is its own teasp, this array now just has value groups
            foreach (JArray st in deftsp)
            {
                string[] singleValGrp = (string[])st.ToObject(typeof(string[]));
                ls0.Add(singleValGrp); // This populates the list of string[] for the extendedTeaspStorageManager with all value-groups
            }

            CastObjArray = CastObjArray.Skip(1).ToArray(); // We dont want the first array, because it will be handled seperately (above)
            foreach (JArray tsp in CastObjArray) // This handles the teasps that correspond with each value-group
            {
                T = (string)tsp[0].ToObject(typeof(string));
                EA = (string)tsp[1].ToObject(typeof(string));
                P = (string)tsp[3].ToObject(typeof(string));

                // // //
                JToken castTest = (JToken)tsp[2].ToObject(typeof(JToken));
                string castTestString = "";

                try
                {
                    castTestString = (string)castTest.ToObject(typeof(string));
                }
                catch (Exception)
                {
                    Handler = 0;
                }

                try
                {
                    Dictionary<string, string> castTestDictionary = (Dictionary<string, string>)castTest.ToObject(typeof(Dictionary<string, string>));
                }
                catch (Exception)
                {
                    Handler = 1;
                }

                // // // Check casting here

                lt0 = AddToLocalList(lt0, tsp, s);

            }

            // When this is finished, lt will now have all the teasps that correspond to each value group ready

            if (identifierForLevel == 0)
            {
                HandleConceptTemplateSetWithValueGroups(ls0, lt0, defTeasp0);
            }
            else if (identifierForLevel == 1)
            {
                HandleLanguageTemplateSetWithValueGroups(ls0, lt0, defTeasp0);
            }
            else if (identifierForLevel == 2)
            {
                HandleTermTemplateSetWithValueGroups(ls0, lt0, defTeasp0);
            }
        }

        private void HandleConceptTemplateSetWithValueGroups(List<string[]> ls0, List<Object> lt0, TeaspNoSubstitution defTeasp0)
        {
            // We are now ready to build the extendedTeaspStorageManager
            ExtendedTeaspStorageManager ETSM1 = new ExtendedTeaspStorageManager(ls0, lt0, defTeasp0);

            // Add it to the dictionary
            GrandMasterDictionary.Add(ConceptMappingTemplatesKeys[KeyCounter], ETSM1);
        }

        private void HandleLanguageTemplateSetWithValueGroups(List<string[]> ls2, List<Object> lt2, TeaspNoSubstitution defTeasp2)
        {
            // We are now ready to build the extendedTeaspStorageManager

            ExtendedTeaspStorageManager ETSM2 = new ExtendedTeaspStorageManager(ls2, lt2, defTeasp2);

            // Add it to the dictionary
            GrandMasterDictionary.Add(LanguageMappingTemplatesKeys[KeyCounter], ETSM2);
        }

        private void HandleTermTemplateSetWithValueGroups(List<string[]> ls3, List<Object> lt3, TeaspNoSubstitution defTeasp3)
        {
            // We are now ready to build the extendedTeaspStorageManager

            ExtendedTeaspStorageManager ETSM3 = new ExtendedTeaspStorageManager(ls3, lt3, defTeasp3);

            // Add it to the dictionary
            GrandMasterDictionary.Add(TermMappingTemplatesKeys[KeyCounter], ETSM3);
        }

        public void ConvertTemplateSets()
        {
            // Logic: A plain template-set will have only 1 internal array, where those that have value groups will have multiple internal arrays, and the first array will hold value groups
            KeyCounter = 0;
            TemplateKeyTracker = 1;

            foreach (JArray j in ConceptMappingTemplates)
            {
                CastObjArray = (object[])j.ToObject(typeof(object[]));

                if (CastObjArray.Length == 1) // This is "plain" template set
                {
                    HandlePlainTemplateSet();
                    KeyCounter++;
                }
                else if (CastObjArray.Length != 0 && CastObjArray.Length > 1) // This is a template set with Value groups 
                {
                    PrepareContentForTemplateWithValueGroups(0);
                    KeyCounter++;
                }
            }

            KeyCounter = 0;
            TemplateKeyTracker = 2;

            foreach (JArray j in LanguageMappingTemplates)
            {
                CastObjArray = (object[])j.ToObject(typeof(object[]));

                if (CastObjArray.Length == 1)
                {
                    HandlePlainTemplateSet();
                    KeyCounter++;
                }
                else if (CastObjArray.Length != 0 && CastObjArray.Length > 1)
                {
                    PrepareContentForTemplateWithValueGroups(1);
                    KeyCounter++;
                }

            }

            KeyCounter = 0;
            TemplateKeyTracker = 3;

            foreach (JArray j in TermMappingTemplates)
            {
                CastObjArray = (object[])j.ToObject(typeof(object[]));

                if (CastObjArray.Length == 1)
                {
                    HandlePlainTemplateSet();
                    KeyCounter++;

                }
                else if (CastObjArray.Length != 0 && CastObjArray.Length > 1)
                {
                    PrepareContentForTemplateWithValueGroups(2);
                    KeyCounter++;
                }

            }

            KeyCounter = 0;
            TemplateKeyTracker = -1;

        }

    }

    // The One-level mappings are broken down into 3 dictionaries, each belonging to one of the original concept levels, and then sent to parse the template-sets that are still JObjects at this point;

    public class OneLevelMapping
    {
        public Dictionary<string, object> ConceptOLvlDictionary { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> LanguageOLvlDictionary { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> TermOLvlDictionary { get; set; } = new Dictionary<string, object>();
        public TemplateSetApp TS { get; set; }

        public OneLevelMapping(Dictionary<string, JObject> d)
        {
            JObject tempC = d["concept"];
            ConceptOLvlDictionary = tempC.ToObject<Dictionary<string, object>>();

            JObject tempL = d["language"];
            LanguageOLvlDictionary = tempL.ToObject<Dictionary<string, object>>();

            JObject tempT = d["term"];
            TermOLvlDictionary = tempT.ToObject<Dictionary<string, object>>();
        }

        public Dictionary<string, object> BeginTemplate()
        {
            TS = new TemplateSetApp(ConceptOLvlDictionary, LanguageOLvlDictionary, TermOLvlDictionary);
            return TS.GetGrandMasterDictionary();
        }
    }

    // A dictionary is created for the 3 possible categorical mappings: Concept, Language and Term. Their values are still JObjects that are handed off to the next function.

    public class CMapClass
    {
        public Dictionary<string, JObject> ConceptDefault { get; set; } = new Dictionary<string, JObject>();
        public OneLevelMapping PassDictionary { get; set; }
        public Dictionary<string, object> DS { get; set; } = new Dictionary<string, object>();

        public CMapClass(JObject c, JObject l, JObject t)
        {
            ConceptDefault.Add("concept", c);
            ConceptDefault.Add("language", l);
            ConceptDefault.Add("term", t);
        }

        public Dictionary<string, object> ParseOLvl() //Hand over dictionary to oneLevelMapping
        {
            PassDictionary = new OneLevelMapping(ConceptDefault);
            DS = PassDictionary.BeginTemplate();
            return DS;
        }

    }

    // The orders are seperated into lists for each key that exists. This is the end of handling the Queue-draining orders

    public class ListOfOrders
    {
        public List<string[]> Concept { get; set; } = new List<string[]>();
        public List<string[]> Language { get; set; } = new List<string[]>();
        public List<string[]> Term { get; set; } = new List<string[]>();

        public ListOfOrders(Dictionary<string, JArray> k)
        {
            JArray c = (JArray)k["conceptGrp"];
            if (c != null)
            {
                object[] sArray1 = (object[])c.ToObject(typeof(object[]));
                string[][] s = c.ToObject<string[][]>();
                foreach (string[] a in s)
                {
                    Concept.Add(a);
                }
            }

            JArray l = (JArray)k["languageGrp"];
            if (l != null)
            {
                object[] sArray2 = (object[])l.ToObject(typeof(object[]));
                string[][] s1 = l.ToObject<string[][]>();
                foreach (string[] a in s1)
                {
                    Language.Add(a);
                }
            }
            
            JArray t = (JArray)k["termGrp"];
            if (t != null)
            {
                object[] sArray3 = (object[])t.ToObject(typeof(object[]));
                string[][] s2 = t.ToObject<string[][]>();
                foreach (string[] a in s2)
                {
                    Term.Add(a);
                }
            }
        }

        public List<string[]> GetConcept()
        {
            return Concept;
        }

        public List<string[]> GetLanguage()
        {
            return Language;
        }

        public List<string[]> GetTerm()
        {
            return Term;
        }
    }

    // The beginning of the Queue-drainind orders method. The object is constructed with the JObject[3] sent from the original JObject. A dictionary is created and passed for parsing the orders

    public class QueueOrders
    {
        public Dictionary<string, JArray> QueueBOrders { get; set; } = new Dictionary<string, JArray>(); //Or just a regular object?? 
        public ListOfOrders LoO { get; set; }

        public QueueOrders(JObject j)
        {
            JArray cGStrings = (JArray)j["conceptGrp"];
            JArray lGStrings = (JArray)j["languageGrp"];
            JArray tGStrings = (JArray)j["termGrp"];

            if (cGStrings == null && lGStrings == null && tGStrings == null)
            {
                LoO = null;
            }
            else
            {
                QueueBOrders.Add("conceptGrp", cGStrings);
                QueueBOrders.Add("languageGrp", lGStrings);
                QueueBOrders.Add("termGrp", tGStrings);
                LoO = new ListOfOrders(QueueBOrders);
            }
        }

        public Dictionary<string, string[]> GetOrders()
        {
            if (LoO == null) { return null; }
            Dictionary<string, string[]> combinedOrders = new Dictionary<string, string[]>();
            List<string[]> c = LoO.GetConcept();
            List<string[]> l = LoO.GetLanguage();
            List<string[]> t = LoO.GetTerm();

            for (int i = 0; i < (c.Count()); i++)
            {
                // for each value in a string array, there needs to be a key with that string value, and a value of the array. In an array of 3 strings, you will have 3 keys and each will have the same value
                for (int j = 0; j < 2; j++)
                {
                    if (combinedOrders.ContainsKey(c[i][j]))
                    {
                        continue;
                    }
                    combinedOrders.Add(c[i][j], c[i]); // Make sure this isnt nonsense
                }
            }
            for (int i = 0; i < (l.Count()); i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (combinedOrders.ContainsKey(l[i][j]))
                    {
                        continue;
                    }
                    combinedOrders.Add(l[i][j], l[i]);
                }
            }
            for (int i = 0; i < (t.Count()); i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (combinedOrders.ContainsKey(t[i][j]))
                    {
                        continue;
                    }
                    combinedOrders.Add(t[i][j], t[i]);
                }
            }

            return combinedOrders;

        }
    }

    // This is where the surface level JSON file is stored. The categorical mapping is parsed though the parseCMap method and the Queue-draining orders are parsed through the startQueue method

    public class LevelOneClass
    {
        public string Dialect { get; set; }
        public string XcsElement { get; set; }
        public JArray ObjectStorage { get; set; }
        public CMapClass ParseCMP { get; set; }
        public QueueOrders QDO { get; set; }
        public Dictionary<string, object> DictionaryStorage { get; set; } = new Dictionary<string, object>();

        public LevelOneClass(string d, string x, JArray cmp)
        {
            Dialect = d;
            XcsElement = x;
            ObjectStorage = cmp;
        }

        public string GetDialect()
        {
            return Dialect;
        }

        public string GetXCS()
        {
            return XcsElement;
        }

        public void ParseCMap()
        {
            JObject conceptLvl = (JObject)ObjectStorage[2]["concept"];
            JObject languageLvl = (JObject)ObjectStorage[2]["language"];
            JObject termLvl = (JObject)ObjectStorage[2]["term"];

            ParseCMP = new CMapClass(conceptLvl, languageLvl, termLvl);
            DictionaryStorage = ParseCMP.ParseOLvl();
            StartQueue();
        }

        public void StartQueue()
        {
            JObject j = (JObject)ObjectStorage[3];
            QDO = new QueueOrders(j);
        }

        public Dictionary<string, object> GetMasterDictionary()
        {
            return DictionaryStorage;
        }

        public Dictionary<string, string[]> GetQueueOrders()
        {
            return QDO.GetOrders();
        }


    }

    public class ConverterApp
    {

        private int ConceptCounter { get; set; } = 1;
        private XmlDocument MultiTermDoc { get; set; }

        // Converter Utilities

        private void PlacePreviousAttributes(XmlNode oldRoot, XmlElement newRoot)
        {
            if (oldRoot.Attributes != null)
            {
                if (oldRoot.Attributes["type"] != null)
                {
                    newRoot.SetAttribute("type", oldRoot.Attributes["type"].Value);
                }

                if (oldRoot.Attributes["lang"] != null)
                {
                    newRoot.SetAttribute("lang", oldRoot.Attributes["lang"].Value);
                }

                if (oldRoot.Attributes["id"] != null)
                {
                    newRoot.SetAttribute("id", oldRoot.Attributes["id"].Value);
                }

                if (oldRoot.Attributes["xml:lang"] != null)
                {
                    newRoot.SetAttribute("xml:lang", oldRoot.Attributes["xml:lang"].Value);
                }
            }
        }

        public void RenameXMLNode(XmlNode oldRoot, string newname)
        {
            XmlElement newRootElement = MultiTermDoc.CreateElement(newname);

            PlacePreviousAttributes(oldRoot, newRootElement);

            XmlNode newRoot = newRootElement;

            foreach (XmlNode childNode in oldRoot.ChildNodes)
            {
                newRoot.AppendChild(childNode.CloneNode(true));
            }
            XmlNode parent = oldRoot.ParentNode;
            if (parent != null)
            {
                parent.ReplaceChild(newRoot, oldRoot);
            }
            else
            {
                MultiTermDoc.ReplaceChild(newRoot, MultiTermDoc.DocumentElement);
            }
        }

        // Queue-Bundling Orders

        private void ExecuteQueueBundlingOrders(Dictionary<string, string[]> queueOrders)
        {
            if (queueOrders == null) { return; }
            foreach (KeyValuePair<string, string[]> pair in queueOrders)
            {
                string queryItemOne = "//descrip[@type='";
                queryItemOne = queryItemOne + pair.Value[0] + "']";

                XmlNodeList pairItemOne = MultiTermDoc.SelectNodes(queryItemOne);

                for (int i = 0; i < pairItemOne.Count; i++)
                {
                    XmlNode tempNode = pairItemOne[i];
                    XmlNode directParent = tempNode.ParentNode;
                    XmlNode grandparent = directParent.ParentNode;

                    string queryItemTwo = "//descrip[@type='";
                    queryItemTwo = queryItemTwo + pair.Value[0] + "']";

                    XmlNodeList limitedScopeQuery = grandparent.SelectNodes(queryItemTwo);

                    foreach (XmlNode node in limitedScopeQuery)
                    {
                        directParent.InsertAfter(node, tempNode);
                    }
                }
            }
        }

        // TBXHeader Addition

        private void ModifyTBXHeader(string xcs, XmlNode root, string tbxDialect)
        {
       
            // Surround the body in the <text> and <body> tags
            XmlNode text = MultiTermDoc.CreateElement("text");
            RenameXMLNode(root, "body");
            root = MultiTermDoc.SelectSingleNode("body");

            // Whitespace cleanup
            MultiTermDoc.RemoveChild(root);
            MultiTermDoc.RemoveChild(MultiTermDoc.LastChild);

            text.AppendChild(root);
            MultiTermDoc.AppendChild(text);

            XmlElement tbxRoot = MultiTermDoc.CreateElement("tbx");
            tbxRoot.SetAttribute("style", "dca");
            tbxRoot.SetAttribute("type", tbxDialect);
            tbxRoot.SetAttribute("xml:lang", "en");
            //tbxRoot.SetAttribute("xmlns", "urn:iso:std:iso:30042:ed:3.0");
            XmlNode tbx = tbxRoot;

            XmlNode tbxHeader = MultiTermDoc.CreateElement("tbxHeader");

            XmlNode fileDesc = MultiTermDoc.CreateElement("fileDesc");

            XmlNode titleStmt = MultiTermDoc.CreateElement("titleStmt");

            XmlNode title = MultiTermDoc.CreateElement("title");
            title.InnerText = "MultiTerm Termbase TBX File";

            titleStmt.AppendChild(title);

            XmlNode sourceDesc = MultiTermDoc.CreateElement("sourceDesc");

            XmlNode p = MultiTermDoc.CreateElement("p");
            p.InnerText = "Converted from MultiTerm XML";

            sourceDesc.AppendChild(p);

            fileDesc.AppendChild(titleStmt);
            fileDesc.AppendChild(sourceDesc);

            /*

            XmlNode encodingDesc = multiTermDoc.CreateElement("encodingDesc");

            XmlElement p2 = multiTermDoc.CreateElement("p");
            p2.SetAttribute("type", "DCSName");
            p2.InnerText = xcs;
            XmlNode secondP = p2;

            encodingDesc.AppendChild(secondP);

            */

            tbxHeader.AppendChild(fileDesc);

            // tbxHeader.AppendChild(encodingDesc);

            // Append the header info
            tbx.AppendChild(tbxHeader);

            // Append text, which has the rest of the body
            tbx.AppendChild(text);
            
            MultiTermDoc.AppendChild(tbx);
        }

        // Reorder Internal Nodes
        private void CorrectOrdering()
        {
            ScanEntries(new string[] { "//termSec", "//langSec", "//conceptEntry"});
        }

        private void ScanEntries(string[] XPaths)
        {
            foreach (string xPath in XPaths)
            {
                ScanEntry(xPath);
            }
        }

        private void ScanEntry(string XPath)
        {
            XmlNodeList items = MultiTermDoc.SelectNodes(XPath);
            foreach (XmlNode item in items.AsParallel())
            {
                ReorderLevelChildren(item);
            }
        }

        private void ReorderLevelChildren(XmlNode node)
        {
            string keyChildName = (node.Name == "conceptEntry") ? "langSec" : (node.Name == "langSec") ? "termSec" : "term";

            if (keyChildName != "term")
            {
                XmlNodeList keyChildren = node.SelectNodes($"{keyChildName}");
                foreach (XmlNode keyChild in keyChildren)
                {
                    node.RemoveChild(keyChild);
                }

                foreach (XmlNode keyChild in keyChildren)
                {
                    node.AppendChild(keyChild);
                }
            }
            else
            {
                XmlNode keyChild = node.SelectSingleNode($"{keyChildName}");
                node.RemoveChild(keyChild);
                node.PrependChild(keyChild);
            }
        }

        private void PlaceOriginationElements()
        {
            XmlNodeList origination = MultiTermDoc.SelectNodes("//transac[@type='origination']");

            foreach (XmlNode node in origination)
            {
                string textValue = node.InnerText;
                XmlElement responsibility = MultiTermDoc.CreateElement("transacNote");
                responsibility.SetAttribute("type", "responsibility");
                responsibility.InnerText = node.InnerText;
                node.ParentNode.InsertAfter(responsibility, node);
                node.InnerText = "origination";
            }
        }

        private void PlaceModificationElements()
        {
            XmlNodeList modification = MultiTermDoc.SelectNodes("//transac[@type='modification']");

            foreach (XmlNode node in modification)
            {
                string textValue = node.InnerText;
                XmlElement responsibility = MultiTermDoc.CreateElement("transacNote");
                responsibility.SetAttribute("type", "responsibility");
                responsibility.InnerText = node.InnerText;
                node.ParentNode.InsertAfter(responsibility, node);
                node.InnerText = "modification";
            }
        }

        private void CreateTransacGrpPairs()
        {
            PlaceOriginationElements();
            PlaceModificationElements();
        }

        private void ExtractLanguageInfo()
        {
            XmlNodeList languageNodeList = MultiTermDoc.SelectNodes("//language");
            foreach (XmlNode node in languageNodeList)
            {
                XmlNode parent = node.ParentNode;
                string languageCode = node.Attributes["lang"].Value;
                parent.RemoveChild(node);
                XmlAttribute xmlLang = MultiTermDoc.CreateAttribute("xml:lang");
                xmlLang.Value = languageCode;
                parent.Attributes.Append(xmlLang);
                RenameXMLNode(parent, "langSec");
            }
        }

        private void ExtractConceptInfo()
        {
            XmlNodeList conceptNodeList = MultiTermDoc.SelectNodes("//conceptGrp");
            foreach (XmlNode node in conceptNodeList)
            {
                XmlNode conceptNode = node.SelectSingleNode("concept");
                if (conceptNode != null)
                {
                    string idValue = conceptNode.InnerText;
                    node.RemoveChild(conceptNode);
                    XmlAttribute id = MultiTermDoc.CreateAttribute("id");
                    id.Value = "_" + idValue;
                    node.Attributes.Append(id);
                }
                else
                {
                    XmlAttribute id = MultiTermDoc.CreateAttribute("id");
                    id.Value = "c" + ConceptCounter;
                    node.Attributes.Append(id);
                    ConceptCounter++;
                }
                RenameXMLNode(node, "conceptEntry");
            }
        }

        private void ExtractSingleChildDescripGrp()
        {
            XmlNodeList descripGrpList = MultiTermDoc.SelectNodes("//descripGrp");

            foreach (XmlNode node in descripGrpList)
            {
                // whitespace, descip, whitespace - or just the descrip
                if (node.ChildNodes.Count == 3)
                {
                    XmlNode extractedChild = node.ChildNodes[1];
                    node.ParentNode.ReplaceChild(extractedChild, node);
                }
                else if (node.ChildNodes.Count == 1)
                {
                    XmlNode extractedChild = node.ChildNodes[0];
                    node.ParentNode.ReplaceChild(extractedChild, node);
                }
            }
        }

        private void ReplaceTermNoteLocations(XmlNode termGrp)
        {
            foreach (XmlNode child in termGrp.ChildNodes)
            {
                if (child.Name != "termNote") { continue; }

                XmlNode lastReferencedNode = termGrp.SelectSingleNode("term");
                // After terms
                termGrp.InsertAfter(child, lastReferencedNode);
                lastReferencedNode = child;
            }
        }

        private void ReplaceDescripGrpLocations(XmlNode termGrp)
        {
            XmlNodeList lastReferencedNode = termGrp.SelectNodes("termNote");
            if (lastReferencedNode == null || lastReferencedNode.Count == 0)
            {
                lastReferencedNode = termGrp.SelectNodes("term");
            }

            XmlNode refNode = lastReferencedNode[0];

            foreach (XmlNode child in termGrp.ChildNodes)
            {
                if (child.Name != "descripGrp") { continue; }
                // After termNotes
                termGrp.InsertAfter(child, refNode);
                refNode = child;
            }
        }

        private void ReplaceTransacGrpLocations(XmlNode termGrp)
        {
            XmlNodeList lastReferencedNode = termGrp.SelectNodes("transacGrp");
            if (lastReferencedNode == null || lastReferencedNode.Count == 0)
            {
                lastReferencedNode = termGrp.SelectNodes("termNote");
            }

            if (lastReferencedNode == null || lastReferencedNode.Count == 0)
            {
                lastReferencedNode = termGrp.SelectNodes("term");
            }

            XmlNode refNode = lastReferencedNode[0];

            foreach (XmlNode child in termGrp.ChildNodes)
            {
                if (child.Name != "transacGrp") { continue; }
                // After descripGrps
                termGrp.InsertAfter(child, refNode);
                refNode = child;
            }
        }

        private void TermGrpReordering()
        {
            XmlNodeList termGrpList = MultiTermDoc.SelectNodes("tbx/text/body/conceptGrp/languageGrp/termGrp");

            foreach (XmlNode termGrp in termGrpList)
            {

                ReplaceTermNoteLocations(termGrp);

                ReplaceDescripGrpLocations(termGrp);

                ReplaceTransacGrpLocations(termGrp);

                RenameXMLNode(termGrp, "termSec");

            }
        }

        private void ReorderXML(XmlNode root)
        {
            // Process termGrp elements
            TermGrpReordering();

            // Extract single child descripGrps
            ExtractSingleChildDescripGrp();

            // Extract language information
            ExtractLanguageInfo();

            // Extract concept information
            ExtractConceptInfo();

            // Create transacGrp pairs
            CreateTransacGrpPairs();
        }

        // Node Removal

        private void RemoveXref(XmlNode root)
        {
            XmlNodeList xref = MultiTermDoc.SelectNodes("//xref");
            foreach (XmlNode node in xref)
            {
                if (node.ParentNode != null)
                {
                    XmlNode parent = node.ParentNode;
                    string textBeforeRemoval = parent.InnerText;
                    parent.RemoveChild(node);
                    parent.InnerText = textBeforeRemoval;
                }
            }
        }

        // Data Category Replacement Through Mapping

        private int FindTeaspIndex(List<string[]> ValGrpTemp, string currentContent)
        {
            for (int i = 0; i < ValGrpTemp.Count(); i++)
            {
                string[] currentStringArray = ValGrpTemp[i];
                for (int k = 0; k < currentStringArray.Count(); k++)
                {
                    if (currentStringArray[k] == currentContent)
                    {
                        // Remember this spot and break, dont store k because order may be different in teasp's substitution rule
                        return i; // This will indicate which index in the correspondingTemp List has our teasp    
                    }
                }
            }
            return -1; // Did not find content in Value-Groups, indicate that default must be used 
        }

        private void HandleTeasp(object teasp, XmlNode node)
        {
            // Shared members between both classes

            string target = "";
            string element = "";
            string placement = "";

            if (teasp.GetType() == typeof(TeaspNoSubstitution))
            {
                target = ((TeaspNoSubstitution)teasp).GetTarget();
                element = ((TeaspNoSubstitution)teasp).GetElementOrAttribute();
                placement = ((TeaspNoSubstitution)teasp).GetPlacement();
            }
            else if (teasp.GetType() == typeof(TeaspWithSubstitution))
            {
                target = ((TeaspWithSubstitution)teasp).GetTarget();
                element = ((TeaspWithSubstitution)teasp).GetElementOrAttribute();
                placement = ((TeaspWithSubstitution)teasp).GetPlacement();
            }

            string currentNodeName = node.Name;

            // Select target element name
            Match match = Regex.Match(element, @"<\w([^\s]+)");
            string retirevedElementName = match.Groups[0].Value;
            retirevedElementName = retirevedElementName.Substring(1);

            if (currentNodeName != retirevedElementName)
            {
                currentNodeName = retirevedElementName;
            }

            // Select the new Attribute
            Match matchAtt = Regex.Match(element, @"'([^']*)");
            string attribute = matchAtt.Groups[1].Value;

            if ((node.Attributes == null || node.Attributes["type"] == null) && attribute != "")
            {
                XmlAttribute type = MultiTermDoc.CreateAttribute("type");
                type.Value = attribute;
                node.Attributes.Append(type);
            }
            else
            {
                if (attribute != "")
                {
                    node.Attributes["type"].Value = attribute;
                }
                else
                {
                    node.Attributes.RemoveAll();
                }
            }

            RenameXMLNode(node, currentNodeName);

        }

        private void HandleTeaspNoSubstitution(TeaspNoSubstitution teasp, XmlNode node)
        {
            HandleTeasp(teasp, node);
        }

        private void HandleTeaspWithSubstitution(TeaspWithSubstitution teasp, XmlNode node)
        {
            // Replace Substitution String
            Dictionary<string, string> substitution = teasp.GetSubstitution();
            node.InnerText = substitution[node.InnerText];

            HandleTeasp(teasp, node);
        }

        private void HandleIndexedTeasp(ExtendedTeaspStorageManager teasp, XmlNode node, List<object> valueGroupCorrespondingTeasps, int index)
        {
            object neutralTeasp = valueGroupCorrespondingTeasps[index];
            if (neutralTeasp is TeaspNoSubstitution castedTeaspNoSub)
            {
                HandleTeaspNoSubstitution(castedTeaspNoSub, node);
            }
            else if (neutralTeasp is TeaspWithSubstitution castedTeaspWithSub)
            {
                HandleTeaspWithSubstitution(castedTeaspWithSub, node);
            }
        }

        private void HandleDefaultTeasp(ExtendedTeaspStorageManager teasp, XmlNode node)
        {
            TeaspNoSubstitution defaultTeasp = teasp.GetDefaultTeaspSub();
            HandleTeasp(defaultTeasp, node);
        }

        private void HandleExtendedTeaspStorageManager(ExtendedTeaspStorageManager teasp, XmlNode node)
        {
            List<string[]> valueGroupList = teasp.GetValueGroupCollection();
            List<object> valueGroupCorrespondingTeasps = teasp.GetCorrespondingValGrpTeasps();

            int relativeTeaspIndex = FindTeaspIndex(valueGroupList, node.InnerText);

            if (relativeTeaspIndex >= 0)
            {
                HandleIndexedTeasp(teasp, node, valueGroupCorrespondingTeasps, relativeTeaspIndex);
            }
            else if (relativeTeaspIndex == -1)
            {
                HandleDefaultTeasp(teasp, node);
            }
        }

        private void HandleExecutionOfTeasps(XmlNode node, Dictionary<string, object> highLevelDictionaryStorage)
        {
            if (node.Attributes != null && node.Attributes["type"] != null)
            {
                string currentAttributeValue = node.Attributes["type"].Value;
                if (highLevelDictionaryStorage.ContainsKey(currentAttributeValue))
                {
                    Object teaspObj = highLevelDictionaryStorage[currentAttributeValue];
                    if (teaspObj.GetType() == typeof(TeaspNoSubstitution))
                    {
                        HandleTeaspNoSubstitution((TeaspNoSubstitution)teaspObj, node);
                    }
                    else if (teaspObj.GetType() == typeof(TeaspWithSubstitution))
                    {
                        HandleTeaspWithSubstitution((TeaspWithSubstitution)teaspObj, node);
                    }
                    else if (teaspObj.GetType() == typeof(ExtendedTeaspStorageManager))
                    {
                        HandleExtendedTeaspStorageManager((ExtendedTeaspStorageManager)teaspObj, node);
                    }
                }
            }
        }

        private void ParseForDataCategories(XmlNode root, Dictionary<string, object> highLevelDictionaryStorage)
        {
            HandleExecutionOfTeasps(root, highLevelDictionaryStorage);
            XmlNodeList children = root.ChildNodes;
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].NodeType == XmlNodeType.Whitespace) { continue; }
                ParseForDataCategories(children[i], highLevelDictionaryStorage);
            }
        }

        // Node Pairing

        private void PairNodes()
        {
            XmlNodeList adminSource = MultiTermDoc.SelectNodes("//admin[@type='source']");
            foreach (XmlNode node in adminSource)
            {
                if (node.ParentNode.Name == "termGrp")
                {
                    XmlNode pairingSibling = node.PreviousSibling;
                    XmlNode staticSibling = pairingSibling.PreviousSibling;

                    XmlNode descripGrouper = MultiTermDoc.CreateElement("descripGrp");
                    descripGrouper.AppendChild(pairingSibling);
                    descripGrouper.AppendChild(node);

                    staticSibling.ParentNode.InsertAfter(descripGrouper, staticSibling);
                }
            }
        }

        // TBX Dialect processing

        private string FindConceptId(XmlNode node)
        {
            if (node.Name == "tbx")
            {
                return null;
            }


            if (node.Name != "conceptEntry")
            {
                FindConceptId(node.ParentNode);
            }
            else
            {
                return node.Attributes["id"].Value;
            }

            return null;
        }

        private void LogStrippedInfo(List<XmlNode> nodesToRemove, StreamWriter stream, Dictionary<string, int> nodeOccuranceTracker, string dialect)
        {
            foreach (XmlNode node in nodesToRemove)
            {
                XmlNode parent = node.ParentNode;
                parent.RemoveChild(node);

                string idNumber = FindConceptId(parent);

                string currentLine = node.Attributes["type"].Value + " discovered!";
                stream.WriteLine(currentLine);

                currentLine = "This data category must be removed for TBX-" + dialect + " compliance.";
                stream.WriteLine(currentLine);

                currentLine = "The following information may be useful if you wish to replace the information:";
                stream.WriteLine(currentLine);
                stream.WriteLine();


                if (idNumber != null)
                {
                    currentLine = "conceptEntry id: " + idNumber;
                    stream.WriteLine(currentLine);
                }

                currentLine = "Parent element: <" + parent.Name + ">";
                stream.WriteLine(currentLine);
                stream.WriteLine();


                if (nodeOccuranceTracker.ContainsKey(node.Attributes["type"].Value))
                {
                    int currentIndex = nodeOccuranceTracker[node.Attributes["type"].Value];
                    nodeOccuranceTracker[node.Attributes["type"].Value] = ++currentIndex;
                }
                else
                {
                    nodeOccuranceTracker[node.Attributes["type"].Value] = 1;
                }
            }
        }

        private void StripForCore(string errorPath)
        {
            XmlProcessingInstruction rng = MultiTermDoc.CreateProcessingInstruction("xml-model", "href=\"https://raw.githubusercontent.com/LTAC-Global/TBX-Core_dialect/master/Schemas/TBXcoreStructV03_TBX-Core_integrated.rng\" " +
             "type=\"application/xml\" schematypens=\"http://relaxng.org/ns/structure/1.0\"");
            XmlProcessingInstruction sch = MultiTermDoc.CreateProcessingInstruction("xml-model", "href=\"https://raw.githubusercontent.com/LTAC-Global/TBX-Core_dialect/master/Schemas/TBX-Core.sch\" " +
                "type=\"application/xml\" schematypens=\"http://purl.oclc.org/dsdl/schematron\"");

            MultiTermDoc.InsertBefore(sch, MultiTermDoc.DocumentElement);
            MultiTermDoc.InsertBefore(rng, MultiTermDoc.DocumentElement);

            XmlNodeList allNodes = MultiTermDoc.SelectNodes("//*");
            List<XmlNode> nodesToRemove = new List<XmlNode>();

            for (int i = 0; i < allNodes.Count; i++)
            {
                if (allNodes[i].Attributes != null && allNodes[i].Attributes["type"] != null && allNodes[i].Attributes["type"].Value != "TBX-Core" && allNodes[i].Attributes["type"].Value != "DCSName")
                {
                    nodesToRemove.Add(allNodes[i]);
                }
            }

            if (nodesToRemove.Count > 0)
            {
                Dictionary<string, int> nodeOccuranceTracker = new Dictionary<string, int>();
                //string outputPath = errorPath.Replace("ConvertedTBX.tbx", "ExceptionLog.txt");
                StreamWriter stream = new StreamWriter(errorPath, false);

                stream.WriteLine("Begin document analysis:");
                stream.WriteLine();
                stream.WriteLine();

                LogStrippedInfo(nodesToRemove, stream, nodeOccuranceTracker, "Core");

                XmlNodeList emptyTransacGrp = MultiTermDoc.SelectNodes("//transacGrp");
                foreach (XmlNode transacGrp in emptyTransacGrp)
                {
                    transacGrp.ParentNode.RemoveChild(transacGrp);
                }

                XmlNodeList emptyDescripGrp = MultiTermDoc.SelectNodes("//descripGrp");
                foreach (XmlNode descripGrp in emptyDescripGrp)
                {
                    descripGrp.ParentNode.RemoveChild(descripGrp);
                }

                stream.WriteLine();
                stream.WriteLine("Finished. Printing report:");
                stream.WriteLine();
                stream.WriteLine();

                foreach (KeyValuePair<string, int> pair in nodeOccuranceTracker)
                {
                    string output = pair.Key + " Data Category: " + pair.Value;
                    stream.WriteLine(output);
                }

                stream.Close();
            }
        }

        private void StripForMin(string errorPath)
        {
            XmlProcessingInstruction rng = MultiTermDoc.CreateProcessingInstruction("xml-model", "href=\"https://raw.githubusercontent.com/LTAC-Global/TBX-Min_dialect/master/DCA/TBXcoreStructV03_TBX-Min_integrated.rng\" " +
             "type=\"application/xml\" schematypens=\"http://relaxng.org/ns/structure/1.0\"");
            XmlProcessingInstruction sch = MultiTermDoc.CreateProcessingInstruction("xml-model", "href=\"https://raw.githubusercontent.com/LTAC-Global/TBX-Min_dialect/master/DCA/TBX-Min_DCA.sch\" " +
                "type=\"application/xml\" schematypens=\"http://purl.oclc.org/dsdl/schematron\"");

            MultiTermDoc.InsertBefore(sch, MultiTermDoc.DocumentElement);
            MultiTermDoc.InsertBefore(rng, MultiTermDoc.DocumentElement);

            XmlNodeList allNodes = MultiTermDoc.SelectNodes("//*");
            List<XmlNode> nodesToRemove = new List<XmlNode>();

            for (int i = 0; i < allNodes.Count; i++)
            {
                if (allNodes[i].Attributes != null && allNodes[i].Attributes["type"] != null &&
                    allNodes[i].Attributes["type"].Value != "TBX-Min" && allNodes[i].Attributes["type"].Value != "DCSName" &&
                    allNodes[i].Attributes["type"].Value != "administrativeStatus" && allNodes[i].Attributes["type"].Value != "customerSubset" &&
                    allNodes[i].Attributes["type"].Value != "partOfSpeech" && allNodes[i].Attributes["type"].Value != "subjectField")
                {
                    nodesToRemove.Add(allNodes[i]);
                }
            }

            if (nodesToRemove.Count > 0)
            {
                Dictionary<string, int> nodeOccuranceTracker = new Dictionary<string, int>();
                //string outputPath = errorPath.Replace("ConvertedTBX.tbx", "ExceptionLog.txt");
                StreamWriter stream = new StreamWriter(errorPath, false);

                stream.WriteLine("Begin document analysis:");
                stream.WriteLine();
                stream.WriteLine();

                LogStrippedInfo(nodesToRemove, stream, nodeOccuranceTracker, "Min");

                XmlNodeList emptyTransacGrp = MultiTermDoc.SelectNodes("//transacGrp");
                foreach (XmlNode transacGrp in emptyTransacGrp)
                {
                    transacGrp.ParentNode.RemoveChild(transacGrp);
                }

                XmlNodeList emptyDescripGrp = MultiTermDoc.SelectNodes("//descripGrp");
                foreach (XmlNode descripGrp in emptyDescripGrp)
                {
                    descripGrp.ParentNode.RemoveChild(descripGrp);
                }

                stream.WriteLine();
                stream.WriteLine("Finished. Printing report:");
                stream.WriteLine();
                stream.WriteLine();

                foreach (KeyValuePair<string, int> pair in nodeOccuranceTracker)
                {
                    string output = pair.Key + " Data Category: " + pair.Value;
                    stream.WriteLine(output);
                }

                stream.Close();
            }
        }

        private void StripForBasic(string errorPath)
        {
            XmlProcessingInstruction rng = MultiTermDoc.CreateProcessingInstruction("xml-model", "href=\"https://raw.githubusercontent.com/LTAC-Global/TBX-Basic_dialect/master/DCA/TBXcoreStructV03_TBX-Basic_integrated.rng\" " +
                "type=\"application/xml\" schematypens=\"http://relaxng.org/ns/structure/1.0\"");
            XmlProcessingInstruction sch = MultiTermDoc.CreateProcessingInstruction("xml-model", "href=\"https://raw.githubusercontent.com/LTAC-Global/TBX-Basic_dialect/master/DCA/TBX-Basic_DCA.sch\" " +
                "type=\"application/xml\" schematypens=\"http://purl.oclc.org/dsdl/schematron\"");

            MultiTermDoc.InsertBefore(sch, MultiTermDoc.DocumentElement);
            MultiTermDoc.InsertBefore(rng, MultiTermDoc.DocumentElement);

            XmlNodeList allNodes = MultiTermDoc.SelectNodes("//*");
            List<XmlNode> nodesToRemove = new List<XmlNode>();

            // Important for TBX-Basic: We need to reposition "definition" data categories from inside the termSec to the LangSec level for valdiation
            // We will lose any idea of which term the definition pertained to, so a descrip group will be added with a note

            XmlNodeList allDefinitionDatCat = MultiTermDoc.SelectNodes("//descrip[@type='definition']");  
            foreach (XmlNode node in allDefinitionDatCat)
            {
                XmlNode parent = node.ParentNode;
                XmlNode termSibling = parent.SelectSingleNode("termSec");
                string termValue = termSibling.InnerText;

                XmlElement descripParent = MultiTermDoc.CreateElement("descripGrp");
                XmlNode grp = descripParent;

                XmlElement note = MultiTermDoc.CreateElement("note");
                note.InnerText = "This definition pertains to the following term: " + termValue;
                XmlNode grpNote = note;

                grp.AppendChild(grpNote);
                grp.AppendChild(node);

            }


            for (int i = 0; i < allNodes.Count; i++)
            {
                if (allNodes[i].Attributes != null && allNodes[i].Attributes["type"] != null && 
                    allNodes[i].Attributes["type"].Value != "TBX-Basic" && allNodes[i].Attributes["type"].Value != "DCSName" &&
                    allNodes[i].Attributes["type"].Value != "administrativeStatus" && allNodes[i].Attributes["type"].Value != "customerSubset" &&
                    allNodes[i].Attributes["type"].Value != "partOfSpeech" && allNodes[i].Attributes["type"].Value != "subjectField" &&
                    allNodes[i].Attributes["type"].Value != "context" && allNodes[i].Attributes["type"].Value != "definition" &&
                    allNodes[i].Attributes["type"].Value != "externalCrossReference" && allNodes[i].Attributes["type"].Value != "grammaticalGender" &&
                    allNodes[i].Attributes["type"].Value != "geographicalUsage" && allNodes[i].Attributes["type"].Value != "projectSubset" &&
                    allNodes[i].Attributes["type"].Value != "relatedConcept" && allNodes[i].Attributes["type"].Value != "relatedTerm" &&
                    allNodes[i].Attributes["type"].Value != "responsibility" && allNodes[i].Attributes["type"].Value != "source" &&
                    allNodes[i].Attributes["type"].Value != "termLocation" && allNodes[i].Attributes["type"].Value != "termType" &&
                    allNodes[i].Attributes["type"].Value != "transactionType" && allNodes[i].Attributes["type"].Value != "xGraphic")
                {
                    nodesToRemove.Add(allNodes[i]);
                }
            }

            if (nodesToRemove.Count > 0)
            {
                Dictionary<string, int> nodeOccuranceTracker = new Dictionary<string, int>();
                //string outputPath = errorPath.Replace("ConvertedTBX.tbx", "ExceptionLog.txt");
                StreamWriter stream = new StreamWriter(errorPath, false);

                stream.WriteLine("Begin document analysis:");
                stream.WriteLine();
                stream.WriteLine();

                LogStrippedInfo(nodesToRemove, stream, nodeOccuranceTracker, "Basic");

                stream.WriteLine();
                stream.WriteLine("Finished. Printing report:");
                stream.WriteLine();
                stream.WriteLine();

                foreach (KeyValuePair<string, int> pair in nodeOccuranceTracker)
                {
                    string output = pair.Key + " Data Category: " + pair.Value;
                    stream.WriteLine(output);
                }

                stream.Close();
            }
        }

        private void StripInvalidNodes(string tbxOutputDialect, string errorPath)
        {
            if (tbxOutputDialect == "TBX-Core")
            {
                StripForCore(errorPath);
            }
            else if (tbxOutputDialect == "TBX-Min")
            {
                StripForMin(errorPath);
            }
            else if (tbxOutputDialect == "TBX-Basic")
            {
                StripForBasic(errorPath);
            }
        }

        private void TruncateDates()
        {
            XmlNodeList dates = MultiTermDoc.SelectNodes("//date");
            foreach (XmlNode date in dates)
            {
                Match match = Regex.Match(date.InnerText, @"T[^<]*");
                string truncatedDate = match.Groups[0].Value;
                date.InnerText = date.InnerText.Replace(truncatedDate, "");
            }
        }

        // Pretty Printing Steps

        private void RemoveWhitespaceChildren(XmlNode root)
        {
            List<XmlNode> childrenToRemove = new List<XmlNode>();

            for (int i = 0; i < root.ChildNodes.Count; i++)
            {
                XmlNode child = root.ChildNodes[i];
                if (child.NodeType == XmlNodeType.Whitespace)
                {
                    childrenToRemove.Add(child);
                }

                if (child.HasChildNodes)
                {
                    RemoveWhitespaceChildren(child);
                }
            }

            foreach (XmlNode node in childrenToRemove)
            {
                root.RemoveChild(node);
            }
        }

        // XML Conversion

        private XmlNode SelectRoot(XmlReader reader)
        {
            XmlNode node;

            while (reader.Name != "mtf")
            {
                reader.Read();
            }

            node = MultiTermDoc.ReadNode(reader);
            return node;
        }

        private void PrettyPrintFile(string printPath)
        {
            MultiTermDoc.DocumentElement.SetAttribute("xmlns", "urn:iso:std:iso:30042:ed-2");
            MemoryStream xmlStream = new MemoryStream();
            MultiTermDoc.Save(xmlStream);
            xmlStream.Flush();
            xmlStream.Position = 0;
            MultiTermDoc.Load(xmlStream);


            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t"
            };
            using (XmlWriter writer = XmlWriter.Create(printPath, settings))
            {
                MultiTermDoc.WriteTo(writer);
                writer.Flush();
            }
        }

        private void AddNameSpace(XmlNode currentNode)
        {
            XmlDocument transferDoc = new XmlDocument();
            //transferDoc.DocumentElement.SetAttribute("xmlns", "urn:iso:std:iso:30042:ed:3.0");
            //transferDoc.LoadXml(multiTermDoc.OuterXml);
            //multiTermDoc.DocumentElement.SetAttribute("xmlns", "urn:iso:std:iso:30042:ed:3.0");
        }

        private void CheckXrefs()
        {
            XmlNodeList xrefs = MultiTermDoc.SelectNodes("//xref");
            foreach (XmlNode xref in xrefs.AsParallel())
            {
                if (xref.Attributes?["target"] != null) 
                {
                    string target = xref.Attributes["target"].Value;

                    if (!Regex.IsMatch(target, "^https?://"))
                    {
                        CommentifyNode(xref);
                    }
                }
                else
                {
                    CommentifyNode(xref);
                }
            }
        }

        private void CommentifyNode(XmlNode node)
        {
            string commentContent = node.OuterXml;
            XmlComment xmlComment = MultiTermDoc.CreateComment(commentContent);
            XmlNode parent = node.ParentNode;
            parent.ReplaceChild(xmlComment, node);
        }

        private void ConvertXML(FileStream xmlData, string outputPath, LevelOneClass initialJSON, string tbxOutputDialect, string errorPath)
        {
            XmlReaderSettings readerSettings = new XmlReaderSettings();

            string dialect = initialJSON.GetDialect();
            string xcs = initialJSON.GetXCS();

            Dictionary<string, object> highLevelDictionaryStorage = new Dictionary<string, object>();
            highLevelDictionaryStorage = initialJSON.GetMasterDictionary();

            Dictionary<string, string[]> queueOrders = new Dictionary<string, string[]>();
            queueOrders = initialJSON.GetQueueOrders();

            using (XmlReader reader = XmlReader.Create(xmlData, readerSettings))
            {
                XmlNode root = SelectRoot(reader);

                // Reorder Queue Bundling Pairs
                ExecuteQueueBundlingOrders(queueOrders);

                // Output root and header for TBX V03
                ModifyTBXHeader(xcs, root, tbxOutputDialect);
                root = MultiTermDoc.LastChild;

                // Remove superfluous tab characters
                RemoveXref(root);

                // Reorder elements before parsing
                ReorderXML(root);

                // Parse with the highLevelDictionaryStorage for invalid Data Categories
                ParseForDataCategories(root, highLevelDictionaryStorage);

                // Pair necessary Node that may have been created during Data category parsing
                PairNodes();

                // Strip away content that does not belong to the User-Specified output
                StripInvalidNodes(tbxOutputDialect, errorPath);

                // Correct ordering of elements
                CorrectOrdering();

                // Comment out <xref> elts with invalid target values
                CheckXrefs();

                // Recursively remove built up white space
                RemoveWhitespaceChildren(MultiTermDoc);

                // Truncate dates for XML Validation
                TruncateDates();

                // Apply new XMLNS to finished file
                AddNameSpace(MultiTermDoc);

                // Output our constructed file
                PrettyPrintFile(outputPath);
            }
        }

        public (string,string) DeserializeFile(string mappingFile, string multiTermXML, string tbxOutputDialect, bool isCalledFromMappingWizard)
        {
            JArray castedJSONData;

            if (isCalledFromMappingWizard)
            {
                castedJSONData = (JArray)JsonConvert.DeserializeObject(mappingFile);
            }
            else
            {
                string unserializedMappingFile = File.ReadAllText(mappingFile);
                castedJSONData = (JArray)JsonConvert.DeserializeObject(unserializedMappingFile);
            }

            string dialect = (string)castedJSONData[0];
            string xcs = (string)castedJSONData[0];

            // Stores First two strings and the remainder of the data as an un-parsed object
            LevelOneClass initialJSON = new LevelOneClass(dialect, xcs, castedJSONData);

            // Parses the file from the concept-map down to the bottom
            initialJSON.ParseCMap();

            // Import XML
            FileStream xmlData = File.OpenRead(multiTermXML);
            MultiTermDoc = new XmlDocument
            {
                PreserveWhitespace = true
            };
            MultiTermDoc.Load(multiTermXML);

            SaveFileDialog dlg = new SaveFileDialog
            {
                DefaultExt = ".tbx",
                Filter = "TBX File (*.tbx)|*.tbx"
            };

            bool? result = dlg.ShowDialog();

            string outputPath = "";
            string pathBuilder = Path.GetFileName(multiTermXML);
            if (result == true)
            {
                outputPath = dlg.FileName;
            }
            else
            {
                outputPath = multiTermXML.Replace(pathBuilder, "ConvertedTBX.txt");
            }

            string errorPath = multiTermXML.Replace(pathBuilder, $"{Path.GetFileNameWithoutExtension(outputPath)}_ExceptionsLog_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.txt").Replace(".tbx", "");

            ConvertXML(xmlData, outputPath, initialJSON, tbxOutputDialect, errorPath);

            return (outputPath, errorPath);
        }


        public static string ReadFile(string type)
        {
            string fn = "";

            OpenFileDialog dlg = new OpenFileDialog
            {
                Title = "Please select your " + type + " file."
            };

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                fn = dlg.FileName;
            }

            return fn;
        }

    }
}