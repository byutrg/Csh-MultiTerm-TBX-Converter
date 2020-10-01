using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Win32;
using System.Xml.XPath;
using System.Xml;
using System.Diagnostics;

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
        private XDocument MultiTermDoc { get; set; }

        // Converter Utilities

        private void PlacePreviousAttributes(XElement oldRoot, XElement newRoot)
        {
            if (oldRoot.Attributes().Any())
            {
                if (oldRoot.Attribute("type") != null)
                {
                    newRoot.SetAttributeValue("type", oldRoot.Attribute("type").Value);
                }

                if (oldRoot.Attribute("lang") != null)
                {
                    newRoot.SetAttributeValue("lang", oldRoot.Attribute("lang").Value);
                }

                if (oldRoot.Attribute("id") != null)
                {
                    newRoot.SetAttributeValue("id", oldRoot.Attribute("id").Value);
                }

                if (oldRoot.Attribute(XNamespace.Xml + "lang") != null)
                {
                    newRoot.SetAttributeValue(XNamespace.Xml + "lang", oldRoot.Attribute(XNamespace.Xml + "lang").Value);
                }
            }
        }

        public void RenameXElement(XElement oldRoot, string newname)
        {
            oldRoot.Name = oldRoot.GetDefaultNamespace() + newname;
        }

        // Queue-Bundling Orders

        private void ExecuteQueueBundlingOrders(Dictionary<string, string[]> queueOrders)
        {
            if (queueOrders == null) { return; }
            foreach (KeyValuePair<string, string[]> pair in queueOrders)
            {
                string queryItemOne = "//descrip[@type='";
                queryItemOne = queryItemOne + pair.Value[0] + "']";

                List<XElement> pairItemOne = MultiTermDoc.XPathSelectElements(queryItemOne).ToList();

                foreach (XElement tempNode in pairItemOne)
                {
                    XElement directParent = tempNode.Parent;
                    XElement grandparent = directParent.Parent;

                    string queryItemTwo = "//descrip[@type='";
                    queryItemTwo = queryItemTwo + pair.Value[0] + "']";

                    List<XElement> limitedScopeQuery = grandparent.XPathSelectElements(queryItemTwo).ToList();

                    foreach (XElement elt in limitedScopeQuery)
                    {
                        tempNode.AddAfterSelf(elt);
                    }
                }
            }
        }

        // TBXHeader Addition

        private void ModifyTBXHeader(string xcs, XElement root, string tbxDialect)
        {
       
            // Surround the body in the <text> and <body> tags
            XElement text = new XElement("text");
            RenameXElement(root, "body");
            root = MultiTermDoc.Descendants("body").FirstOrDefault();

            text.Add(root);
            MultiTermDoc.Root.Add(text);

            XElement tbxRoot = new XElement("tbx");
            tbxRoot.SetAttributeValue("style", "dca");
            tbxRoot.SetAttributeValue("type", tbxDialect);
            tbxRoot.SetAttributeValue(XNamespace.Xml + "lang", "en");
            //tbxRoot.SetAttribute("xmlns", "urn:iso:std:iso:30042:ed:3.0");
            XElement tbx = tbxRoot;

            XElement tbxHeader = new XElement("tbxHeader");

            XElement fileDesc = new XElement("fileDesc");

            XElement titleStmt = new XElement("titleStmt");

            XElement title = new XElement("title");
            title.Add(new XText("MultiTerm Termbase TBX File"));

            titleStmt.Add(title);

            XElement sourceDesc = new XElement("sourceDesc");

            XElement p = new XElement("p");
            p.Add(new XText("Converted from MultiTerm XML"));

            sourceDesc.Add(p);

            fileDesc.Add(titleStmt);
            fileDesc.Add(sourceDesc);

            /*

            XNode encodingDesc = multiTermDoc.CreateElement("encodingDesc");

            XElement p2 = multiTermDoc.CreateElement("p");
            p2.SetAttribute("type", "DCSName");
            p2.InnerText = xcs;
            XNode secondP = p2;

            encodingDesc.AppendChild(secondP);

            */

            tbxHeader.Add(fileDesc);

            // tbxHeader.AppendChild(encodingDesc);

            // Append the header info
            tbx.Add(tbxHeader);

            // Append text, which has the rest of the body
            tbx.Add(text);

            MultiTermDoc.Root.ReplaceWith(tbx);
        }

        // Reorder Internal Nodes

        private void PlaceOriginationElements()
        {
            List<XElement> origination = MultiTermDoc.XPathSelectElements("//transac[@type='origination']").ToList();

            foreach (XElement node in origination)
            {
                string textValue = node.Value;
                XElement responsibility = new XElement("transacNote");
                responsibility.SetAttributeValue("type", "responsibility");
                responsibility.SetValue(node.Value);
                node.AddAfterSelf(responsibility);
                node.SetValue("origination");
            }
        }

        private void PlaceModificationElements()
        {
            List<XElement> modification = MultiTermDoc.XPathSelectElements("//transac[@type='modification']").ToList();

            foreach (XElement node in modification)
            {
                string textValue = node.Value;
                XElement responsibility = new XElement("transacNote");
                responsibility.SetAttributeValue("type", "responsibility");
                responsibility.SetValue(node.Value);
                node.AddAfterSelf(responsibility);
                node.SetValue("modification");
            }
        }

        private void CreateTransacGrpPairs()
        {
            PlaceOriginationElements();
            PlaceModificationElements();
        }

        private void ExtractLanguageInfo()
        {
            List<XElement> languageNodeList = MultiTermDoc.XPathSelectElements("//language").ToList();
            foreach (XElement node in languageNodeList)
            {
                XElement parent = node.Parent;
                string languageCode = node.Attribute("lang")?.Value;
                node.Remove();
                XAttribute xmlLang = new XAttribute(XNamespace.Xml + "lang", languageCode);
                parent.Add(xmlLang);
                RenameXElement(parent, "langSec");
            }
        }

        private void ExtractConceptInfo()
        {
            List<XElement> conceptNodeList = MultiTermDoc.XPathSelectElements("//conceptGrp").ToList();
            foreach (XElement node in conceptNodeList)
            {
                XElement conceptNode = node.XPathSelectElement("concept");
                if (conceptNode != null)
                {
                    string idValue = conceptNode.Value;
                    conceptNode.Remove();
                    XAttribute id = new XAttribute("id", "_" + idValue);
                    node.Add(id);
                }
                else
                {
                    XAttribute id = new XAttribute("id", "c" + ConceptCounter);
                    node.Add(id);
                    ConceptCounter++;
                }
                RenameXElement(node, "conceptEntry");
            }
        }

        private void ExtractSingleChildDescripGrp()
        {
            List<XElement> descripGrpList = MultiTermDoc.XPathSelectElements("//descripGrp").ToList();

            foreach (XElement node in descripGrpList)
            {
                // whitespace, descip, whitespace - or just the descrip
                if (node.Elements().Count() == 3)
                {
                    XElement extractedChild = node.Elements().ElementAt(1);
                    node.ReplaceWith(extractedChild);
                }
                else if (node.Elements().Count() == 1)
                {
                    XNode extractedChild = node.Elements().ElementAt(0);
                    node.ReplaceWith(extractedChild);
                }
            }
        }

        private void ReplaceTermNoteLocations(XElement termGrp)
        {
            XElement term = termGrp.Element("term");

            foreach (XElement child in termGrp.Elements("termNote").Reverse().ToList())
            {
                // After terms
                child.Remove();
                term.AddAfterSelf(child);
            }
        }

        private void ReplaceDescripGrpLocations(XElement termGrp)
        {
            XElement refNode = termGrp.Elements().Where(elt => elt.Name.LocalName.StartsWith("term")).Last();

            foreach (XElement child in termGrp.Elements("descripGrp").Reverse().ToList())
            {
                // After termNotes
                child.Remove();
                refNode.AddAfterSelf(child);
            }
        }

        private void ReplaceTransacGrpLocations(XElement termGrp)
        {
            XElement refNode = termGrp.Elements()
                                        .Where(elt => elt.Name.LocalName.StartsWith("term") ||
                                                      elt.Name.LocalName.Equals("descripGrp"))
                                        .Last();

            foreach (XElement child in termGrp.Elements("transacGrp").Reverse().ToList())
            {
                // After descripGrps
                child.Remove();
                refNode.AddAfterSelf(child);
            }
        }

        private void TermGrpReordering()
        {
            List<XElement> termGrpList = MultiTermDoc.Descendants("termGrp").ToList();

            foreach (XElement termGrp in termGrpList)
            {
                XElement term = termGrp.Element("term");
                term.Remove();
                termGrp.AddFirst(term);

                ReplaceTermNoteLocations(termGrp);

                ReplaceDescripGrpLocations(termGrp);

                ReplaceTransacGrpLocations(termGrp);

                RenameXElement(termGrp, "termSec");
            }
        }

        private void ReorderXML(XNode root)
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

        private void RemoveXref(XNode root)
        {
            List<XElement> xref = MultiTermDoc.XPathSelectElements("//xref").ToList();
            foreach (XElement node in xref)
            {
                if (node.Parent != null)
                {
                    XElement parent = node.Parent;
                    string textBeforeRemoval = parent.Value;
                    node.ReplaceWith(new XText(textBeforeRemoval));
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

        private void HandleTeasp(object teasp, XElement node)
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

            string currentNodeName = node.Name.LocalName;

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

            if ((!node.Attributes().Any() || !node.Attributes("type").Any()) && attribute != "")
            {
                XAttribute type = new XAttribute("type", attribute);
                node.Add(type);
            }
            else
            {
                if (attribute != "")
                {
                    node.SetAttributeValue("type", attribute);
                }
                else
                {
                    node.Attributes().Remove();
                }
            }

            RenameXElement(node, currentNodeName);
            if (node.Name.LocalName.Equals("xref") && node.Attribute("target") == null)
            {
                node.SetAttributeValue("target", "");
            }
        }

        private void HandleTeaspNoSubstitution(TeaspNoSubstitution teasp, XElement node)
        {
            HandleTeasp(teasp, node);
        }

        private void HandleTeaspWithSubstitution(TeaspWithSubstitution teasp, XElement node)
        {
            // Replace Substitution String
            Dictionary<string, string> substitution = teasp.GetSubstitution();
            node.SetValue(substitution[node.Value]);

            HandleTeasp(teasp, node);
        }

        private void HandleIndexedTeasp(ExtendedTeaspStorageManager teasp, XElement node, List<object> valueGroupCorrespondingTeasps, int index)
        {
            object neutralTeasp = valueGroupCorrespondingTeasps[index];
            if (neutralTeasp is TeaspNoSubstitution)
            {
                TeaspNoSubstitution castedTeasp = (TeaspNoSubstitution)neutralTeasp;
                HandleTeaspNoSubstitution(castedTeasp, node);
            }
            else if (neutralTeasp is TeaspWithSubstitution)
            {
                TeaspWithSubstitution castedTeasp = (TeaspWithSubstitution)neutralTeasp;
                HandleTeaspWithSubstitution(castedTeasp, node);
            }
        }

        private void HandleDefaultTeasp(ExtendedTeaspStorageManager teasp, XElement node)
        {
            TeaspNoSubstitution defaultTeasp = teasp.GetDefaultTeaspSub();
            HandleTeasp(defaultTeasp, node);
        }

        private void HandleExtendedTeaspStorageManager(ExtendedTeaspStorageManager teasp, XElement node)
        {
            List<string[]> valueGroupList = teasp.GetValueGroupCollection();
            List<object> valueGroupCorrespondingTeasps = teasp.GetCorrespondingValGrpTeasps();

            int relativeTeaspIndex = FindTeaspIndex(valueGroupList, node.Value);

            if (relativeTeaspIndex >= 0)
            {
                HandleIndexedTeasp(teasp, node, valueGroupCorrespondingTeasps, relativeTeaspIndex);
            }
            else if (relativeTeaspIndex == -1)
            {
                HandleDefaultTeasp(teasp, node);
            }
        }

        private void HandleExecutionOfTeasps(XElement node, Dictionary<string, object> highLevelDictionaryStorage)
        {
            if (node.Attributes("type").Any())
            {
                string currentAttributeValue = node.Attribute("type").Value;
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

        private void ParseForDataCategories(XElement root, Dictionary<string, object> highLevelDictionaryStorage)
        {
            HandleExecutionOfTeasps(root, highLevelDictionaryStorage);
            List<XElement> children = root.Elements().ToList();
            foreach (XElement elt in children)
            {
                if (elt.NodeType == XmlNodeType.Whitespace) { continue; }
                ParseForDataCategories(elt, highLevelDictionaryStorage);
            }
        }

        // Node Pairing

        private void PairNodes()
        {
            List<XElement> adminSource = MultiTermDoc.XPathSelectElements("//admin[@type='source']").ToList();
            foreach (XElement node in adminSource)
            {
                if (node.Parent.Name.LocalName == "termGrp")
                {
                    XElement pairingSibling = node.ElementsBeforeSelf().Last();
                    XElement staticSibling = pairingSibling.ElementsBeforeSelf()
                                                        .ElementAt(pairingSibling.ElementsBeforeSelf().Count() - 2);

                    XElement descripGrouper = new XElement("descripGrp");
                    descripGrouper.Add(pairingSibling);
                    descripGrouper.Add(node);

                    staticSibling.AddAfterSelf(descripGrouper);
                }
            }
        }

        // TBX Dialect processing

        private string FindConceptId(XElement node)
        {
            if (node.Name.LocalName == "tbx")
            {
                return null;
            }


            if (node.Name.LocalName != "conceptEntry")
            {
                FindConceptId(node.Parent);
            }
            else
            {
                return node.Attribute("id")?.Value;
            }

            return null;
        }

        private void LogStrippedInfo(List<XElement> nodesToRemove, StreamWriter stream, Dictionary<string, int> nodeOccuranceTracker, string dialect)
        {
            foreach (XElement node in nodesToRemove)
            {
                XElement parent = node.Parent;
                node.Remove();

                string idNumber = FindConceptId(parent);

                string currentLine = node.Attribute("type").Value + " discovered!";
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


                if (nodeOccuranceTracker.ContainsKey(node.Attribute("type").Value))
                {
                    int currentIndex = nodeOccuranceTracker[node.Attribute("type").Value];
                    nodeOccuranceTracker[node.Attribute("type").Value] = ++currentIndex;
                }
                else
                {
                    nodeOccuranceTracker[node.Attribute("type").Value] = 1;
                }
            }
        }

        private void StripForCore(string errorPath)
        {
            XProcessingInstruction rng = new XProcessingInstruction("xml-model", "href=\"https://raw.githubusercontent.com/LTAC-Global/TBX-Core_dialect/master/Schemas/TBXcoreStructV03_TBX-Core_integrated.rng\" " +
             "type=\"application/xml\" schematypens=\"http://relaxng.org/ns/structure/1.0\"");
            XProcessingInstruction sch = new XProcessingInstruction("xml-model", "href=\"https://raw.githubusercontent.com/LTAC-Global/TBX-Core_dialect/master/Schemas/TBX-Core.sch\" " +
                "type=\"application/xml\" schematypens=\"http://purl.oclc.org/dsdl/schematron\"");

            MultiTermDoc.Root.AddBeforeSelf(sch);
            MultiTermDoc.Root.AddBeforeSelf(rng);

            List<XElement> allNodes = MultiTermDoc.Descendants().ToList();
            List<XElement> nodesToRemove = new List<XElement>();

            foreach (XElement elt in allNodes)
            {
                if (elt.Attribute("type") != null && elt.Attribute("type").Value != "TBX-Core" && elt.Attribute("type").Value != "DCSName")
                {
                    nodesToRemove.Add(elt);
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

                List<XElement> emptyTransacGrp = MultiTermDoc.XPathSelectElements("//transacGrp").ToList();
                foreach (XElement transacGrp in emptyTransacGrp)
                {
                    transacGrp.Remove();
                }

                List<XElement> emptyDescripGrp = MultiTermDoc.XPathSelectElements("//descripGrp").ToList();
                foreach (XElement descripGrp in emptyDescripGrp)
                {
                    descripGrp.Remove();
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
            XProcessingInstruction rng = new XProcessingInstruction("xml-model", "href=\"https://raw.githubusercontent.com/LTAC-Global/TBX-Min_dialect/master/DCA/TBXcoreStructV03_TBX-Min_integrated.rng\" " +
             "type=\"application/xml\" schematypens=\"http://relaxng.org/ns/structure/1.0\"");
            XProcessingInstruction sch = new XProcessingInstruction("xml-model", "href=\"https://raw.githubusercontent.com/LTAC-Global/TBX-Min_dialect/master/DCA/TBX-Min_DCA.sch\" " +
                "type=\"application/xml\" schematypens=\"http://purl.oclc.org/dsdl/schematron\"");

            MultiTermDoc.Root.AddBeforeSelf(sch);
            MultiTermDoc.Root.AddBeforeSelf(rng);

            List<XElement> allNodes = MultiTermDoc.XPathSelectElements("//*").ToList();
            List<XElement> nodesToRemove = new List<XElement>();

            foreach (XElement elt in allNodes)
            {
                if (elt.Attribute("type") != null &&
                    elt.Attribute("type").Value != "TBX-Min" && elt.Attribute("type").Value != "DCSName" &&
                    elt.Attribute("type").Value != "administrativeStatus" && elt.Attribute("type").Value != "customerSubset" &&
                    elt.Attribute("type").Value != "partOfSpeech" && elt.Attribute("type").Value != "subjectField")
                {
                    nodesToRemove.Add(elt);
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

                List<XElement> emptyTransacGrp = MultiTermDoc.XPathSelectElements("//transacGrp").ToList();
                foreach (XNode transacGrp in emptyTransacGrp)
                {
                    transacGrp.Remove();
                }

                List<XElement> emptyDescripGrp = MultiTermDoc.XPathSelectElements("//descripGrp").ToList();
                foreach (XElement descripGrp in emptyDescripGrp)
                {
                    descripGrp.Remove();
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
            XProcessingInstruction rng = new XProcessingInstruction("xml-model", "href=\"https://raw.githubusercontent.com/LTAC-Global/TBX-Basic_dialect/master/DCA/TBXcoreStructV03_TBX-Basic_integrated.rng\" " +
                "type=\"application/xml\" schematypens=\"http://relaxng.org/ns/structure/1.0\"");
            XProcessingInstruction sch = new XProcessingInstruction("xml-model", "href=\"https://raw.githubusercontent.com/LTAC-Global/TBX-Basic_dialect/master/DCA/TBX-Basic_DCA.sch\" " +
                "type=\"application/xml\" schematypens=\"http://purl.oclc.org/dsdl/schematron\"");

            MultiTermDoc.Root.AddBeforeSelf(sch);
            MultiTermDoc.Root.AddBeforeSelf(rng);

            List<XElement> allNodes = MultiTermDoc.XPathSelectElements("//*").ToList();
            List<XElement> nodesToRemove = new List<XElement>();

            // Important for TBX-Basic: We need to reposition "definition" data categories from inside the termSec to the LangSec level for valdiation
            // We will lose any idea of which term the definition pertained to, so a descrip group will be added with a note

            List<XElement> allDefinitionDatCat = MultiTermDoc.XPathSelectElements("//descrip[@type='definition']").ToList();  
            foreach (XElement node in allDefinitionDatCat)
            {
                XElement parent = node.Parent;
                XElement termSibling = parent.XPathSelectElement("//termSec");
                string termValue = termSibling.Value;

                XElement descripParent = new XElement("descripGrp");
                XElement grp = descripParent;

                XElement note = new XElement("note");
                note.SetValue("This definition pertains to the following term: " + termValue);
                XElement grpNote = note;

                grp.Add(grpNote);
                grp.Add(node);

            }


            foreach (XElement elt in allNodes)
            {
                if (elt.Attribute("type") != null && 
                    elt.Attribute("type").Value != "TBX-Basic" && elt.Attribute("type").Value != "DCSName" &&
                    elt.Attribute("type").Value != "administrativeStatus" && elt.Attribute("type").Value != "customerSubset" &&
                    elt.Attribute("type").Value != "partOfSpeech" && elt.Attribute("type").Value != "subjectField" &&
                    elt.Attribute("type").Value != "context" && elt.Attribute("type").Value != "definition" &&
                    elt.Attribute("type").Value != "externalCrossReference" && elt.Attribute("type").Value != "grammaticalGender" &&
                    elt.Attribute("type").Value != "geographicalUsage" && elt.Attribute("type").Value != "projectSubset" &&
                    elt.Attribute("type").Value != "relatedConcept" && elt.Attribute("type").Value != "relatedTerm" &&
                    elt.Attribute("type").Value != "responsibility" && elt.Attribute("type").Value != "source" &&
                    elt.Attribute("type").Value != "termLocation" && elt.Attribute("type").Value != "termType" &&
                    elt.Attribute("type").Value != "transactionType" && elt.Attribute("type").Value != "xGraphic")
                {
                    nodesToRemove.Add(elt);
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

        private void ReorderTermSecChildren(XElement root)
        {
            List<XElement> termSecs = root.Descendants("termSec").ToList();
            foreach (XElement termSec in termSecs)
            {
                XElement term = termSec.Element("term");
                term.Remove();
                termSec.AddFirst(term);
                List<XElement> termNoteFamily = termSec.Elements().Where(elt => elt.Name.LocalName.StartsWith("termNote")).ToList(); ;
                termNoteFamily.Remove();
                term.AddAfterSelf(termNoteFamily);
            }
        }

        private void ReorderConceptEntryAndLangSecChildren(XElement root)
        {
            List<XElement> targets = root.Descendants()
                                                .Where(
                                                    elt => elt.Name.LocalName.Equals("conceptEntry") ||
                                                            elt.Name.LocalName.Equals("langSec")).ToList();
            foreach (XElement t in targets)
            {
                List<XElement> secs = t.Elements().Where(elt => elt.Name.LocalName.EndsWith("Sec")).ToList();
                secs.Remove();
                t.Add(secs);
            }
        }

        private void CleanGrps(XElement root)
        {
            List<XElement> grps = root.Descendants().Where(elt => elt.Name.LocalName.EndsWith("Grp")).ToList();
            foreach (XElement grp in grps)
            {
                string prefix = grp.Name.LocalName.Substring(0, grp.Name.LocalName.Length - 3);
                if (grp.Element(prefix) != null) continue;

                grp.ReplaceWith(grp.Nodes());
            }
        }

        private void TruncateDates()
        {
            List<XElement> dates = MultiTermDoc.Descendants("date").ToList();
            foreach (XElement date in dates)
            {
                Match match = Regex.Match(date.Value, @"T[^<]*");
                string truncatedDate = match.Groups[0].Value;
                date.SetValue(date.Value.Replace(truncatedDate, ""));
            }
        }

        // Pretty Printing Steps

        private void RemoveWhitespaceChildren(XElement root)
        {
            List<XElement> childrenToRemove = new List<XElement>();

            foreach (XElement child in root.Elements())
            {
                if (child.NodeType == XmlNodeType.Whitespace)
                {
                    childrenToRemove.Add(child);
                }

                if (child.HasElements)
                {
                    RemoveWhitespaceChildren(child);
                }
            }

            foreach (XElement node in childrenToRemove)
            {
                node.Remove();
            }
        }

        // XML Conversion

        private XElement SelectRoot(XmlReader reader)
        {
            return MultiTermDoc.Descendants("mtf").First();
        }

        private void SetNamespaceGlobally(XElement xElement, string ns)
        {
            foreach (XElement element in xElement.DescendantsAndSelf())
            {
                element.Name = ((XNamespace) ns) + element.Name.LocalName;
            }
        }

        private void PrettyPrintFile(string printPath)
        {
            SetNamespaceGlobally(MultiTermDoc.Root, "urn:iso:std:iso:30042:ed-2");
            MemoryStream xmlStream = new MemoryStream();
            MultiTermDoc.Save(xmlStream);
            xmlStream.Flush();
            xmlStream.Position = 0;
            MultiTermDoc = XDocument.Load(xmlStream);


            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            using (XmlWriter writer = XmlWriter.Create(printPath, settings))
            {
                MultiTermDoc.WriteTo(writer);
                writer.Flush();
            }
        }

        private void AddNameSpace(XNode currentNode)
        {
            XDocument transferDoc = new XDocument();
            //transferDoc.DocumentElement.SetAttribute("xmlns", "urn:iso:std:iso:30042:ed:3.0");
            //transferDoc.LoadXml(multiTermDoc.OuterXml);
            //multiTermDoc.DocumentElement.SetAttribute("xmlns", "urn:iso:std:iso:30042:ed:3.0");
        }

        private void CheckXrefs()
        {
            List<XElement> xrefs = MultiTermDoc.Descendants("xref").ToList();
            foreach (XElement xref in xrefs)
            {
                if (xref.Attribute("target") != null) 
                {
                    string target = xref.Attribute("target").Value;

                    bool targetMatched = Regex.IsMatch(target, "^https?://");
                    Match contentMatch = Regex.Match(xref.Value, @"(^https?://[^\s<]+)");

                    if (!targetMatched && contentMatch.Success) {
                        xref.SetAttributeValue("target", contentMatch.Groups[1].Value);
                    } else {
                        CommentifyNode(xref);
                    }
                }
                else
                {
                    CommentifyNode(xref);
                }
            }
        }

        private void CommentifyNode(XElement node)
        {
            string commentContent = node.ToString();
            XComment xmlComment = new XComment(commentContent);
            node.ReplaceWith(xmlComment);
        }
        private void ConvertXML(string outputPath, LevelOneClass initialJSON, string tbxOutputDialect, string errorPath)
        {
            string dialect = initialJSON.GetDialect();
            string xcs = initialJSON.GetXCS();

            Dictionary<string, object> highLevelDictionaryStorage = new Dictionary<string, object>();
            highLevelDictionaryStorage = initialJSON.GetMasterDictionary();

            Dictionary<string, string[]> queueOrders = new Dictionary<string, string[]>();
            queueOrders = initialJSON.GetQueueOrders();

            XElement root = MultiTermDoc.Root;

            // Reorder Queue Bundling Pairs
            ExecuteQueueBundlingOrders(queueOrders);

            // Output root and header for TBX V03
            ModifyTBXHeader(xcs, root, tbxOutputDialect);
            root = MultiTermDoc.Root;

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

            // Re-order termSec children
            ReorderTermSecChildren(root);

            // Comment out <xref> elts with invalid target values
            CheckXrefs();

            // Re-order langSec children
            ReorderConceptEntryAndLangSecChildren(root);

            // clean Grps
            CleanGrps(root);

            // Recursively remove built up white space
            RemoveWhitespaceChildren(MultiTermDoc.Root);

            // Truncate dates for XML Validation
            TruncateDates();

            // Apply new XMLNS to finished file
            AddNameSpace(MultiTermDoc);

            // Output our constructed file
            PrettyPrintFile(outputPath);
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
            MultiTermDoc = XDocument.Load(multiTermXML, LoadOptions.PreserveWhitespace);

            SaveFileDialog dlg = new SaveFileDialog();

            dlg.DefaultExt = ".tbx";
            dlg.Filter = "TBX File (*.tbx)|*.tbx";
            bool? result = dlg.ShowDialog();

            string outputPath = "";
            string pathBuilder = System.IO.Path.GetFileName(multiTermXML);
            if (result == true)
            {
                outputPath = dlg.FileName;
            }
            else
            {
                outputPath = multiTermXML.Replace(pathBuilder, "ConvertedTBX.txt");
            }

            string errorPath = multiTermXML.Replace(pathBuilder, $"{Path.GetFileNameWithoutExtension(outputPath)}_ExceptionsLog_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.txt").Replace(".tbx", "");

            ConvertXML(outputPath, initialJSON, tbxOutputDialect, errorPath);

            return (outputPath, errorPath);
        }


        public static string ReadFile(string type)
        {
            string fn = "";

            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Title = "Please select your " + type + " file.";

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                fn = dlg.FileName;
            }

            return fn;
        }

    }
}



