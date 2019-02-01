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
        public List<string[]> valueGroupCollection; // Each string[] will correspond with the teasp in the same position
        public List<object> correspondingValGrpTeasps; // Every one of these will have substitutions, and therefore the default will not be in this list
        public TeaspNoSubstitution defaultTeaspSub;

        public ExtendedTeaspStorageManager(List<string[]> ls, List<object> lt, TeaspNoSubstitution dt)
        {
            valueGroupCollection = ls;
            correspondingValGrpTeasps = lt;
            defaultTeaspSub = dt;
        }

        public List<string[]> getValueGroupCollection()
        {
            return valueGroupCollection;
        }

        public List<object> getCorrespondingValGrpTeasps()
        {
            return correspondingValGrpTeasps;
        }

        public TeaspNoSubstitution getDefaultTeaspSub()
        {
            return defaultTeaspSub;
        }
    }

    // This is the vanilla teasp template. It takes an array of strings as its constructor and stores the appropriate values, although it currently does not account for the possibility of a 
    // dictionary-like object for a substitution.

    public class TeaspNoSubstitution
    {
        public string target;
        public string elementOrAttributes;
        public string substitution;
        public string placement;

        public TeaspNoSubstitution(string t, string ea, string s, string p)
        {
            target = t;
            elementOrAttributes = ea;
            substitution = s;
            placement = p;
        }

        public string getTarget()
        {
            return target;
        }

        public string getElementOrAttribute()
        {
            return elementOrAttributes;
        }

        public string getSubstitution()
        {
            return substitution;
        }

        public string getPlacement()
        {
            return placement;
        }

    }

    // This holds teasps that have no value groups, but do have substitutions

    public class TeaspWithSubstitution
    {
        public string target;
        public string elementOrAttributes;
        public Dictionary<string, string> substitution = new Dictionary<string, string>();
        public string placement;

        public TeaspWithSubstitution(string t, string ea, Dictionary<string, string> s, string p)
        {
            target = t;
            elementOrAttributes = ea;
            foreach (KeyValuePair<string, string> entry in s)
            {
                substitution.Add(entry.Key, entry.Value);
            }
            placement = p;
        }

        public string getTarget()
        {
            return target;
        }

        public string getElementOrAttribute()
        {
            return elementOrAttributes;
        }

        public Dictionary<string, string> getSubstitution()
        {
            return substitution;
        }

        public string getPlacement()
        {
            return placement;
        }

    }

    // The template set is constructed by seperating all of the template-set jObjects from the dictionaries, and stored in individual Lists. Each list is then parsed and seperated into 
    // the default teasp and value-groups, followed by the subsequent teasps.

    public class TemplateSetApp
    {
        public List<object> conceptMappingTemplates = new List<object>();
        public List<string> conceptMappingTemplatesKeys = new List<string>();
        public List<object> languageMappingTemplates = new List<object>();
        public List<string> languageMappingTemplatesKeys = new List<string>();
        public List<object> termMappingTemplates = new List<object>();
        public List<string> termMappingTemplatesKeys = new List<string>();
        public object[] castObjArray;
        public string key;
        public TeaspNoSubstitution teaspNS;
        public TeaspWithSubstitution teaspWS;
        public int handler = 0;
        public int keyCounter = 0;
        public int templateKeyTracker = 0;
        public string[] valGrp;

        // // // //

        // This is the Dicitonary that will contain the Mapping Templates Strings and an object (Either a plain teasp or a extendedTeaspStorageManager object).
        // Regardless of what kind of object each Key-Value pair has, type will be determined at runtime and processing will be done then.

        public Dictionary<string, object> grandMasterDictionary = new Dictionary<string, object>();

        // // // //

        public string t;
        public string ea;
        // Declare s at runtime
        public string p;

        public TemplateSetApp(Dictionary<string, object> c, Dictionary<string, object> l, Dictionary<string, object> t)
        {
            foreach (var entry in c)
            {
                object tempUNK1 = entry.Value;
                conceptMappingTemplates.Add(tempUNK1);

                key = entry.Key;
                conceptMappingTemplatesKeys.Add(key);
            }
            foreach (var entry2 in l)
            {
                object tempUNK2 = entry2.Value;
                languageMappingTemplates.Add(tempUNK2);

                key = entry2.Key;
                languageMappingTemplatesKeys.Add(key);
            }
            foreach (var entry3 in t)
            {
                object tempUNK3 = entry3.Value;
                termMappingTemplates.Add(tempUNK3);

                key = entry3.Key;
                termMappingTemplatesKeys.Add(key);
            }

            convertTemplateSets();
        }

        public Dictionary<string, object> getGrandMasterDictionary()
        {
            return grandMasterDictionary;
        }

        private string keySelector()
        {
            string returnKey = "";

            if (templateKeyTracker == 1)
            {
                returnKey = conceptMappingTemplatesKeys[keyCounter];
            }
            else if (templateKeyTracker == 2)
            {
                returnKey = languageMappingTemplatesKeys[keyCounter];
            }
            else if (templateKeyTracker == 3)
            {
                returnKey = termMappingTemplatesKeys[keyCounter];
            }
            else if (templateKeyTracker == -1)
            {
                throw new Exception("ERROR! Invalid Dictionary Key");
            }

            return returnKey;
        }

        private void addToMasterDictionaryAfterHandle(JArray plainTeasp)
        {
            if (handler == 0)
            {
                Dictionary<string, string> exceptionSub = (Dictionary<string, string>)plainTeasp[2].ToObject(typeof(Dictionary<string, string>));
                var teaspMy = new TeaspWithSubstitution(t, ea, exceptionSub, p);

                grandMasterDictionary.Add(keySelector(), teaspMy);
            }
            else if (handler == 1)
            {
                string s = (string)plainTeasp[2].ToObject(typeof(string));
                var teaspMy = new TeaspNoSubstitution(t, ea, s, p);

                grandMasterDictionary.Add(keySelector(), teaspMy);
            }
        }

        private void handlePlainTemplateSet()
        {
            JArray tempJA = (JArray)castObjArray[0];
            object[] teasp = (object[])tempJA.ToObject(typeof(object[]));
            JArray plainTeasp = (JArray)teasp[0];

            t = (string)plainTeasp[0].ToObject(typeof(string));
            ea = (string)plainTeasp[1].ToObject(typeof(string));
            p = (string)plainTeasp[3].ToObject(typeof(string));

            // // //
            JToken castTest = (JToken)plainTeasp[2].ToObject(typeof(JToken));
            string castTestString = "";

            try
            {
                castTestString = (string)castTest.ToObject(typeof(string));
            }
            catch (Exception e)
            {
                handler = 0;
            }

            try
            {
                Dictionary<string, string> castTestDictionary = (Dictionary<string, string>)castTest.ToObject(typeof(Dictionary<string, string>));
            }
            catch (Exception e)
            {
                handler = 1;
            }

            addToMasterDictionaryAfterHandle(plainTeasp);
        }

        private List<Object> addToLocalList(List<Object> lt0, JArray tsp, string s)
        {
            if (handler == 0)
            {
                Dictionary<string, string> exceptionSub = (Dictionary<string, string>)tsp[2].ToObject(typeof(Dictionary<string, string>));
                var teaspMy = new TeaspWithSubstitution(t, ea, exceptionSub, p);

                lt0.Add(teaspMy);
                return lt0;
            }
            else if (handler == 1)
            {
                string str = (string)tsp[2].ToObject(typeof(string));
                var teaspMy = new TeaspNoSubstitution(t, ea, s, p);

                lt0.Add(teaspMy);
                return lt0;
            }

            return null;
        }

        private void prepareContentForTemplateWithValueGroups(int identifierForLevel)
        {
            List<string[]> ls0 = new List<string[]>();
            List<object> lt0 = new List<object>();
            TeaspNoSubstitution defTeasp0;

            JArray temp = (JArray)castObjArray[0]; // This will have the default Teasp and subsequent value groups
            object[] deftsp = (object[])temp.ToObject(typeof(object[])); // Grab the default teasp
            JArray defaultTsp = (JArray)deftsp[0];

            t = (string)defaultTsp[0].ToObject(typeof(string));
            ea = (string)defaultTsp[1].ToObject(typeof(string));
            string s = (string)defaultTsp[2].ToObject(typeof(string));
            p = (string)defaultTsp[3].ToObject(typeof(string));

            teaspNS = new TeaspNoSubstitution(t, ea, s, p); // This is now ready to give to the extendedTeaspStorageManager
            defTeasp0 = teaspNS;

            deftsp = deftsp.Skip(1).ToArray(); // We dont want the first array, it is its own teasp, this array now just has value groups
            foreach (JArray st in deftsp)
            {
                string[] singleValGrp = (string[])st.ToObject(typeof(string[]));
                ls0.Add(singleValGrp); // This populates the list of string[] for the extendedTeaspStorageManager with all value-groups
            }

            castObjArray = castObjArray.Skip(1).ToArray(); // We dont want the first array, because it will be handled seperately (above)
            foreach (JArray tsp in castObjArray) // This handles the teasps that correspond with each value-group
            {
                t = (string)tsp[0].ToObject(typeof(string));
                ea = (string)tsp[1].ToObject(typeof(string));
                p = (string)tsp[3].ToObject(typeof(string));

                // // //
                JToken castTest = (JToken)tsp[2].ToObject(typeof(JToken));
                string castTestString = "";

                try
                {
                    castTestString = (string)castTest.ToObject(typeof(string));
                }
                catch (Exception e)
                {
                    handler = 0;
                }

                try
                {
                    Dictionary<string, string> castTestDictionary = (Dictionary<string, string>)castTest.ToObject(typeof(Dictionary<string, string>));
                }
                catch (Exception e)
                {
                    handler = 1;
                }

                // // // Check casting here

                lt0 = addToLocalList(lt0, tsp, s);

            }

            // When this is finished, lt will now have all the teasps that correspond to each value group ready

            if (identifierForLevel == 0)
            {
                handleConceptTemplateSetWithValueGroups(ls0, lt0, defTeasp0);
            }
            else if (identifierForLevel == 1)
            {
                handleLanguageTemplateSetWithValueGroups(ls0, lt0, defTeasp0);
            }
            else if (identifierForLevel == 2)
            {
                handleTermTemplateSetWithValueGroups(ls0, lt0, defTeasp0);
            }
        }

        private void handleConceptTemplateSetWithValueGroups(List<string[]> ls0, List<Object> lt0, TeaspNoSubstitution defTeasp0)
        {
            // We are now ready to build the extendedTeaspStorageManager
            ExtendedTeaspStorageManager ETSM1 = new ExtendedTeaspStorageManager(ls0, lt0, defTeasp0);

            // Add it to the dictionary
            grandMasterDictionary.Add(conceptMappingTemplatesKeys[keyCounter], ETSM1);
        }

        private void handleLanguageTemplateSetWithValueGroups(List<string[]> ls2, List<Object> lt2, TeaspNoSubstitution defTeasp2)
        {
            // We are now ready to build the extendedTeaspStorageManager

            ExtendedTeaspStorageManager ETSM2 = new ExtendedTeaspStorageManager(ls2, lt2, defTeasp2);

            // Add it to the dictionary
            grandMasterDictionary.Add(languageMappingTemplatesKeys[keyCounter], ETSM2);
        }

        private void handleTermTemplateSetWithValueGroups(List<string[]> ls3, List<Object> lt3, TeaspNoSubstitution defTeasp3)
        {
            // We are now ready to build the extendedTeaspStorageManager

            ExtendedTeaspStorageManager ETSM3 = new ExtendedTeaspStorageManager(ls3, lt3, defTeasp3);

            // Add it to the dictionary
            grandMasterDictionary.Add(termMappingTemplatesKeys[keyCounter], ETSM3);
        }

        public void convertTemplateSets()
        {
            // Logic: A plain template-set will have only 1 internal array, where those that have value groups will have multiple internal arrays, and the first array will hold value groups
            keyCounter = 0;
            templateKeyTracker = 1;

            foreach (JArray j in conceptMappingTemplates)
            {
                castObjArray = (object[])j.ToObject(typeof(object[]));

                if (castObjArray.Length == 1) // This is "plain" template set
                {
                    handlePlainTemplateSet();
                    keyCounter++;
                }
                else if (castObjArray.Length != 0 && castObjArray.Length > 1) // This is a template set with Value groups 
                {
                    prepareContentForTemplateWithValueGroups(0);
                    keyCounter++;
                }
            }

            keyCounter = 0;
            templateKeyTracker = 2;

            foreach (JArray j in languageMappingTemplates)
            {
                castObjArray = (object[])j.ToObject(typeof(object[]));

                if (castObjArray.Length == 1)
                {
                    handlePlainTemplateSet();
                    keyCounter++;
                }
                else if (castObjArray.Length != 0 && castObjArray.Length > 1)
                {
                    prepareContentForTemplateWithValueGroups(1);
                    keyCounter++;
                }

            }

            keyCounter = 0;
            templateKeyTracker = 3;

            foreach (JArray j in termMappingTemplates)
            {
                castObjArray = (object[])j.ToObject(typeof(object[]));

                if (castObjArray.Length == 1)
                {
                    handlePlainTemplateSet();
                    keyCounter++;

                }
                else if (castObjArray.Length != 0 && castObjArray.Length > 1)
                {
                    prepareContentForTemplateWithValueGroups(2);
                    keyCounter++;
                }

            }

            keyCounter = 0;
            templateKeyTracker = -1;

        }

    }

    // The One-level mappings are broken down into 3 dictionaries, each belonging to one of the original concept levels, and then sent to parse the template-sets that are still JObjects at this point;

    public class OneLevelMapping
    {
        public Dictionary<string, object> cOLvlDictionary = new Dictionary<string, object>();
        public Dictionary<string, object> lOLvlDictionary = new Dictionary<string, object>();
        public Dictionary<string, object> tOLvlDictionary = new Dictionary<string, object>();
        public TemplateSetApp ts;

        public OneLevelMapping(Dictionary<string, JObject> d)
        {
            JObject tempC = d["concept"];
            cOLvlDictionary = tempC.ToObject<Dictionary<string, object>>();

            JObject tempL = d["language"];
            lOLvlDictionary = tempL.ToObject<Dictionary<string, object>>();

            JObject tempT = d["term"];
            tOLvlDictionary = tempT.ToObject<Dictionary<string, object>>();
        }

        public Dictionary<string, object> beginTemplate()
        {
            ts = new TemplateSetApp(cOLvlDictionary, lOLvlDictionary, tOLvlDictionary);
            return ts.getGrandMasterDictionary();
        }
    }

    // A dictionary is created for the 3 possible categorical mappings: Concept, Language and Term. Their values are still JObjects that are handed off to the next function.

    public class CMapClass
    {
        public Dictionary<string, JObject> cDefault = new Dictionary<string, JObject>();
        public OneLevelMapping passDictionary;
        public Dictionary<string, object> ds = new Dictionary<string, object>();

        public CMapClass(JObject c, JObject l, JObject t)
        {
            cDefault.Add("concept", c);
            cDefault.Add("language", l);
            cDefault.Add("term", t);
        }

        public Dictionary<string, object> parseOLvl() //Hand over dictionary to oneLevelMapping
        {
            passDictionary = new OneLevelMapping(cDefault);
            ds = passDictionary.beginTemplate();
            return ds;
        }

    }

    // The orders are seperated into lists for each key that exists. This is the end of handling the Queue-draining orders

    public class ListOfOrders
    {
        List<string[]> concept = new List<string[]>();
        List<string[]> language = new List<string[]>();
        List<string[]> term = new List<string[]>();

        public ListOfOrders(Dictionary<string, JArray> k)
        {
            JArray c = (JArray)k["conceptGrp"];
            if (c != null)
            {
                object[] sArray1 = (object[])c.ToObject(typeof(object[]));
                string[][] s = c.ToObject<string[][]>();
                foreach (string[] a in s)
                {
                    concept.Add(a);
                }
            }

            JArray l = (JArray)k["languageGrp"];
            if (l != null)
            {
                object[] sArray2 = (object[])l.ToObject(typeof(object[]));
                string[][] s1 = l.ToObject<string[][]>();
                foreach (string[] a in s1)
                {
                    language.Add(a);
                }
            }
            
            JArray t = (JArray)k["termGrp"];
            if (t != null)
            {
                object[] sArray3 = (object[])t.ToObject(typeof(object[]));
                string[][] s2 = t.ToObject<string[][]>();
                foreach (string[] a in s2)
                {
                    term.Add(a);
                }
            }
        }

        public List<string[]> getConcerpt()
        {
            return concept;
        }

        public List<string[]> getLanugage()
        {
            return language;
        }

        public List<string[]> getTerm()
        {
            return term;
        }
    }

    // The beginning of the Queue-drainind orders method. The object is constructed with the JObject[3] sent from the original JObject. A dictionary is created and passed for parsing the orders

    public class QueueOrders
    {
        public Dictionary<string, JArray> qBOrders = new Dictionary<string, JArray>(); //Or just a regular object?? 
        public ListOfOrders loo;

        public QueueOrders(JObject j)
        {
            JArray cGStrings = (JArray)j["conceptGrp"];
            JArray lGStrings = (JArray)j["languageGrp"];
            JArray tGStrings = (JArray)j["termGrp"];

            if (cGStrings == null && lGStrings == null && tGStrings == null)
            {
                loo = null;
            }
            else
            {
                qBOrders.Add("conceptGrp", cGStrings);
                qBOrders.Add("languageGrp", lGStrings);
                qBOrders.Add("termGrp", tGStrings);
                loo = new ListOfOrders(qBOrders);
            }
        }

        public Dictionary<string, string[]> getOrders()
        {
            if (loo == null) { return null; }
            Dictionary<string, string[]> combinedOrders = new Dictionary<string, string[]>();
            List<string[]> c = loo.getConcerpt();
            List<string[]> l = loo.getLanugage();
            List<string[]> t = loo.getTerm();

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
        public string dialect { get; set; }
        public string xcsElement { get; set; }
        public JArray objectStorage { get; set; }
        public CMapClass parseCMP;
        public QueueOrders QDO;
        public Dictionary<string, object> dictionaryStorage = new Dictionary<string, object>();

        public LevelOneClass(string d, string x, JArray cmp)
        {
            dialect = d;
            xcsElement = x;
            objectStorage = cmp;
        }

        public string getDialect()
        {
            return dialect;
        }

        public string getXCS()
        {
            return xcsElement;
        }

        public void parseCMap()
        {
            JObject conceptLvl = (JObject)objectStorage[2]["concept"];
            JObject languageLvl = (JObject)objectStorage[2]["language"];
            JObject termLvl = (JObject)objectStorage[2]["term"];

            parseCMP = new CMapClass(conceptLvl, languageLvl, termLvl);
            dictionaryStorage = parseCMP.parseOLvl();
            startQueue();
        }

        public void startQueue()
        {
            JObject j = (JObject)objectStorage[3];
            QDO = new QueueOrders(j);
        }

        public Dictionary<string, object> getMasterDictionary()
        {
            return dictionaryStorage;
        }

        public Dictionary<string, string[]> getQueueOrders()
        {
            return QDO.getOrders();
        }


    }

    public class ConverterApp
    {

        private int conceptCounter = 1;
        private XmlDocument multiTermDoc;

        // Converter Utilities

        private void placePreviousAttributes(XmlNode oldRoot, XmlElement newRoot)
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

        public void renameXMLNode(XmlNode oldRoot, string newname)
        {
            XmlElement newRootElement = multiTermDoc.CreateElement(newname);

            placePreviousAttributes(oldRoot, newRootElement);

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
                multiTermDoc.ReplaceChild(newRoot, multiTermDoc.DocumentElement);
            }
        }

        // Queue-Bundling Orders

        private void executeQueueBundlingOrders(Dictionary<string, string[]> queueOrders)
        {
            if (queueOrders == null) { return; }
            foreach (KeyValuePair<string, string[]> pair in queueOrders)
            {
                string queryItemOne = "//descrip[@type='";
                queryItemOne = queryItemOne + pair.Value[0] + "']";

                XmlNodeList pairItemOne = multiTermDoc.SelectNodes(queryItemOne);

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

        private void modifyTBXHeader(string xcs, XmlNode root, string tbxDialect)
        {
       
            // Surround the body in the <text> and <body> tags
            XmlNode text = multiTermDoc.CreateElement("text");
            renameXMLNode(root, "body");
            root = multiTermDoc.SelectSingleNode("body");

            // Whitespace cleanup
            multiTermDoc.RemoveChild(root);
            multiTermDoc.RemoveChild(multiTermDoc.LastChild);

            text.AppendChild(root);
            multiTermDoc.AppendChild(text);

            XmlElement tbxRoot = multiTermDoc.CreateElement("tbx");
            tbxRoot.SetAttribute("style", "dca");
            tbxRoot.SetAttribute("type", tbxDialect);
            tbxRoot.SetAttribute("xml:lang", "en");
            //tbxRoot.SetAttribute("xmlns", "urn:iso:std:iso:30042:ed:3.0");
            XmlNode tbx = tbxRoot;

            XmlNode tbxHeader = multiTermDoc.CreateElement("tbxHeader");

            XmlNode fileDesc = multiTermDoc.CreateElement("fileDesc");

            XmlNode titleStmt = multiTermDoc.CreateElement("titleStmt");

            XmlNode title = multiTermDoc.CreateElement("title");
            title.InnerText = "MultiTerm Termbase TBX File";

            titleStmt.AppendChild(title);

            XmlNode sourceDesc = multiTermDoc.CreateElement("sourceDesc");

            XmlNode p = multiTermDoc.CreateElement("p");
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

            multiTermDoc.AppendChild(tbx);
        }

        // Reorder Internal Nodes

        private void placeOriginationElements()
        {
            XmlNodeList origination = multiTermDoc.SelectNodes("//transac[@type='origination']");

            foreach (XmlNode node in origination)
            {
                string textValue = node.InnerText;
                XmlElement responsibility = multiTermDoc.CreateElement("transacNote");
                responsibility.SetAttribute("type", "responsibility");
                responsibility.InnerText = node.InnerText;
                node.ParentNode.InsertAfter(responsibility, node);
                node.InnerText = "origination";
            }
        }

        private void placeModificationElements()
        {
            XmlNodeList modification = multiTermDoc.SelectNodes("//transac[@type='modification']");

            foreach (XmlNode node in modification)
            {
                string textValue = node.InnerText;
                XmlElement responsibility = multiTermDoc.CreateElement("transacNote");
                responsibility.SetAttribute("type", "responsibility");
                responsibility.InnerText = node.InnerText;
                node.ParentNode.InsertAfter(responsibility, node);
                node.InnerText = "modification";
            }
        }

        private void createTransacGrpPairs()
        {
            placeOriginationElements();
            placeModificationElements();
        }

        private void extractLanguageInfo()
        {
            XmlNodeList languageNodeList = multiTermDoc.SelectNodes("//language");
            foreach (XmlNode node in languageNodeList)
            {
                XmlNode parent = node.ParentNode;
                string languageCode = node.Attributes["lang"].Value;
                parent.RemoveChild(node);
                XmlAttribute xmlLang = multiTermDoc.CreateAttribute("xml:lang");
                xmlLang.Value = languageCode;
                parent.Attributes.Append(xmlLang);
                renameXMLNode(parent, "langSec");
            }
        }

        private void extractConceptInfo()
        {
            XmlNodeList conceptNodeList = multiTermDoc.SelectNodes("//conceptGrp");
            foreach (XmlNode node in conceptNodeList)
            {
                XmlNode conceptNode = node.SelectSingleNode("concept");
                if (conceptNode != null)
                {
                    string idValue = conceptNode.InnerText;
                    node.RemoveChild(conceptNode);
                    XmlAttribute id = multiTermDoc.CreateAttribute("id");
                    id.Value = "_" + idValue;
                    node.Attributes.Append(id);
                }
                else
                {
                    XmlAttribute id = multiTermDoc.CreateAttribute("id");
                    id.Value = "c" + conceptCounter;
                    node.Attributes.Append(id);
                    conceptCounter++;
                }
                renameXMLNode(node, "conceptEntry");
            }
        }

        private void extractSingleChildDescripGrp()
        {
            XmlNodeList descripGrpList = multiTermDoc.SelectNodes("//descripGrp");

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

        private void replaceTermNoteLocations(XmlNode termGrp)
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

        private void replaceDescripGrpLocations(XmlNode termGrp)
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

        private void replaceTransacGrpLocations(XmlNode termGrp)
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

        private void termGrpReordering()
        {
            XmlNodeList termGrpList = multiTermDoc.SelectNodes("tbx/text/body/conceptGrp/languageGrp/termGrp");

            foreach (XmlNode termGrp in termGrpList)
            {

                replaceTermNoteLocations(termGrp);

                replaceDescripGrpLocations(termGrp);

                replaceTransacGrpLocations(termGrp);

                renameXMLNode(termGrp, "termSec");

            }
        }

        private void reorderXML(XmlNode root)
        {
            // Process termGrp elements
            termGrpReordering();

            // Extract single child descripGrps
            extractSingleChildDescripGrp();

            // Extract language information
            extractLanguageInfo();

            // Extract concept information
            extractConceptInfo();

            // Create transacGrp pairs
            createTransacGrpPairs();
        }

        // Node Removal

        private void removeXref(XmlNode root)
        {
            XmlNodeList xref = multiTermDoc.SelectNodes("//xref");
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

        private int findTeaspIndex(List<string[]> ValGrpTemp, string currentContent)
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

        private void handleTeasp(object teasp, XmlNode node)
        {
            // Shared members between both classes

            string target = "";
            string element = "";
            string placement = "";

            if (teasp.GetType() == typeof(TeaspNoSubstitution))
            {
                target = ((TeaspNoSubstitution)teasp).getTarget();
                element = ((TeaspNoSubstitution)teasp).getElementOrAttribute();
                placement = ((TeaspNoSubstitution)teasp).getPlacement();
            }
            else if (teasp.GetType() == typeof(TeaspWithSubstitution))
            {
                target = ((TeaspWithSubstitution)teasp).getTarget();
                element = ((TeaspWithSubstitution)teasp).getElementOrAttribute();
                placement = ((TeaspWithSubstitution)teasp).getPlacement();
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
                XmlAttribute type = multiTermDoc.CreateAttribute("type");
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

            renameXMLNode(node, currentNodeName);

        }

        private void handleTeaspNoSubstitution(TeaspNoSubstitution teasp, XmlNode node)
        {
            handleTeasp(teasp, node);
        }

        private void handleTeaspWithSubstitution(TeaspWithSubstitution teasp, XmlNode node)
        {
            // Replace Substitution String
            Dictionary<string, string> substitution = teasp.getSubstitution();
            node.InnerText = substitution[node.InnerText];

            handleTeasp(teasp, node);
        }

        private void handleIndexedTeasp(ExtendedTeaspStorageManager teasp, XmlNode node, List<object> valueGroupCorrespondingTeasps, int index)
        {
            object neutralTeasp = valueGroupCorrespondingTeasps[index];
            if (neutralTeasp is TeaspNoSubstitution)
            {
                TeaspNoSubstitution castedTeasp = (TeaspNoSubstitution)neutralTeasp;
                handleTeaspNoSubstitution(castedTeasp, node);
            }
            else if (neutralTeasp is TeaspWithSubstitution)
            {
                TeaspWithSubstitution castedTeasp = (TeaspWithSubstitution)neutralTeasp;
                handleTeaspWithSubstitution(castedTeasp, node);
            }
        }

        private void handleDefaultTeasp(ExtendedTeaspStorageManager teasp, XmlNode node)
        {
            TeaspNoSubstitution defaultTeasp = teasp.getDefaultTeaspSub();
            handleTeasp(defaultTeasp, node);
        }

        private void hanldeExtendedTeaspStorageManager(ExtendedTeaspStorageManager teasp, XmlNode node)
        {
            List<string[]> valueGroupList = teasp.getValueGroupCollection();
            List<object> valueGroupCorrespondingTeasps = teasp.getCorrespondingValGrpTeasps();

            int relativeTeaspIndex = findTeaspIndex(valueGroupList, node.InnerText);

            if (relativeTeaspIndex >= 0)
            {
                handleIndexedTeasp(teasp, node, valueGroupCorrespondingTeasps, relativeTeaspIndex);
            }
            else if (relativeTeaspIndex == -1)
            {
                handleDefaultTeasp(teasp, node);
            }
        }

        private void handleExecutionOfTeasps(XmlNode node, Dictionary<string, object> highLevelDictionaryStorage)
        {
            if (node.Attributes != null && node.Attributes["type"] != null)
            {
                string currentAttributeValue = node.Attributes["type"].Value;
                if (highLevelDictionaryStorage.ContainsKey(currentAttributeValue))
                {
                    Object teaspObj = highLevelDictionaryStorage[currentAttributeValue];
                    if (teaspObj.GetType() == typeof(TeaspNoSubstitution))
                    {
                        handleTeaspNoSubstitution((TeaspNoSubstitution)teaspObj, node);
                    }
                    else if (teaspObj.GetType() == typeof(TeaspWithSubstitution))
                    {
                        handleTeaspWithSubstitution((TeaspWithSubstitution)teaspObj, node);
                    }
                    else if (teaspObj.GetType() == typeof(ExtendedTeaspStorageManager))
                    {
                        hanldeExtendedTeaspStorageManager((ExtendedTeaspStorageManager)teaspObj, node);
                    }
                }
            }
        }

        private void parseForDataCategories(XmlNode root, Dictionary<string, object> highLevelDictionaryStorage)
        {
            handleExecutionOfTeasps(root, highLevelDictionaryStorage);
            XmlNodeList children = root.ChildNodes;
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].NodeType == XmlNodeType.Whitespace) { continue; }
                parseForDataCategories(children[i], highLevelDictionaryStorage);
            }
        }

        // Node Pairing

        private void pairNodes()
        {
            XmlNodeList adminSource = multiTermDoc.SelectNodes("//admin[@type='source']");
            foreach (XmlNode node in adminSource)
            {
                if (node.ParentNode.Name == "termGrp")
                {
                    XmlNode pairingSibling = node.PreviousSibling;
                    XmlNode staticSibling = pairingSibling.PreviousSibling;

                    XmlNode descripGrouper = multiTermDoc.CreateElement("descripGrp");
                    descripGrouper.AppendChild(pairingSibling);
                    descripGrouper.AppendChild(node);

                    staticSibling.ParentNode.InsertAfter(descripGrouper, staticSibling);
                }
            }
        }

        // TBX Dialect processing

        private string findConceptId(XmlNode node)
        {
            if (node.Name == "tbx")
            {
                return null;
            }


            if (node.Name != "conceptEntry")
            {
                findConceptId(node.ParentNode);
            }
            else
            {
                return node.Attributes["id"].Value;
            }

            return null;
        }

        private void logStrippedInfo(List<XmlNode> nodesToRemove, StreamWriter stream, Dictionary<string, int> nodeOccuranceTracker, string dialect)
        {
            foreach (XmlNode node in nodesToRemove)
            {
                XmlNode parent = node.ParentNode;
                parent.RemoveChild(node);

                string idNumber = findConceptId(parent);

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

        private void stripForCore(string pathBuilder)
        {
            XmlProcessingInstruction rng = multiTermDoc.CreateProcessingInstruction("xml-model", "href=\"https://raw.githubusercontent.com/LTAC-Global/TBX-Core_dialect/master/Schemas/TBXcoreStructV03_TBX-Core_integrated.rng\" " +
             "type=\"application/xml\" schematypens=\"http://relaxng.org/ns/structure/1.0\"");
            XmlProcessingInstruction sch = multiTermDoc.CreateProcessingInstruction("xml-model", "href=\"https://raw.githubusercontent.com/LTAC-Global/TBX-Core_dialect/master/Schemas/TBX-Core.sch\" " +
                "type=\"application/xml\" schematypens=\"http://purl.oclc.org/dsdl/schematron\"");

            multiTermDoc.PrependChild(sch);
            multiTermDoc.PrependChild(rng);

            XmlNodeList allNodes = multiTermDoc.SelectNodes("//*");
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
                string outputPath = pathBuilder.Replace("ConvertedTBX.tbx", "ExceptionLog.txt");
                StreamWriter stream = new StreamWriter(outputPath, false);

                stream.WriteLine("Begin document analysis:");
                stream.WriteLine();
                stream.WriteLine();

                logStrippedInfo(nodesToRemove, stream, nodeOccuranceTracker, "Core");

                XmlNodeList emptyTransacGrp = multiTermDoc.SelectNodes("//transacGrp");
                foreach (XmlNode transacGrp in emptyTransacGrp)
                {
                    transacGrp.ParentNode.RemoveChild(transacGrp);
                }

                XmlNodeList emptyDescripGrp = multiTermDoc.SelectNodes("//descripGrp");
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

        private void stripForMin(string pathBuilder)
        {
            XmlProcessingInstruction rng = multiTermDoc.CreateProcessingInstruction("xml-model", "href=\"https://raw.githubusercontent.com/LTAC-Global/TBX-Min_dialect/master/DCA/TBXcoreStructV03_TBX-Min_integrated.rng\" " +
             "type=\"application/xml\" schematypens=\"http://relaxng.org/ns/structure/1.0\"");
            XmlProcessingInstruction sch = multiTermDoc.CreateProcessingInstruction("xml-model", "href=\"https://raw.githubusercontent.com/LTAC-Global/TBX-Min_dialect/master/DCA/TBX-Min_DCA.sch\" " +
                "type=\"application/xml\" schematypens=\"http://purl.oclc.org/dsdl/schematron\"");

            multiTermDoc.PrependChild(sch);
            multiTermDoc.PrependChild(rng);

            XmlNodeList allNodes = multiTermDoc.SelectNodes("//*");
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
                string outputPath = pathBuilder.Replace("ConvertedTBX.tbx", "ExceptionLog.txt");
                StreamWriter stream = new StreamWriter(outputPath, false);

                stream.WriteLine("Begin document analysis:");
                stream.WriteLine();
                stream.WriteLine();

                logStrippedInfo(nodesToRemove, stream, nodeOccuranceTracker, "Min");

                XmlNodeList emptyTransacGrp = multiTermDoc.SelectNodes("//transacGrp");
                foreach (XmlNode transacGrp in emptyTransacGrp)
                {
                    transacGrp.ParentNode.RemoveChild(transacGrp);
                }

                XmlNodeList emptyDescripGrp = multiTermDoc.SelectNodes("//descripGrp");
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

        private void stripForBasic(string pathBuilder)
        {
            XmlProcessingInstruction rng = multiTermDoc.CreateProcessingInstruction("xml-model", "href=\"https://raw.githubusercontent.com/LTAC-Global/TBX-Basic_dialect/master/DCA/TBXcoreStructV03_TBX-Basic_integrated.rng\" " +
                "type=\"application/xml\" schematypens=\"http://relaxng.org/ns/structure/1.0\"");
            XmlProcessingInstruction sch = multiTermDoc.CreateProcessingInstruction("xml-model", "href=\"https://raw.githubusercontent.com/LTAC-Global/TBX-Basic_dialect/master/DCA/TBX-Basic_DCA.sch\" " +
                "type=\"application/xml\" schematypens=\"http://purl.oclc.org/dsdl/schematron\"");

            multiTermDoc.InsertBefore(sch, multiTermDoc.DocumentElement);
            multiTermDoc.InsertBefore(rng, multiTermDoc.DocumentElement);

            XmlNodeList allNodes = multiTermDoc.SelectNodes("//*");
            List<XmlNode> nodesToRemove = new List<XmlNode>();

            // Important for TBX-Basic: We need to reposition "definition" data categories from inside the termSec to the LangSec level for valdiation
            // We will lose any idea of which term the definition pertained to, so a descrip group will be added with a note

            XmlNodeList allDefinitionDatCat = multiTermDoc.SelectNodes("//descrip[@type='definition']");  
            foreach (XmlNode node in allDefinitionDatCat)
            {
                XmlNode parent = node.ParentNode;
                XmlNode termSibling = parent.SelectSingleNode("term");
                string termValue = termSibling.InnerText;

                XmlElement descripParent = multiTermDoc.CreateElement("descripGrp");
                XmlNode grp = descripParent;

                XmlElement note = multiTermDoc.CreateElement("note");
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
                string outputPath = pathBuilder.Replace("ConvertedTBX.tbx", "ExceptionLog.txt");
                StreamWriter stream = new StreamWriter(outputPath, false);

                stream.WriteLine("Begin document analysis:");
                stream.WriteLine();
                stream.WriteLine();

                logStrippedInfo(nodesToRemove, stream, nodeOccuranceTracker, "Basic");

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

        private void stripInvalidNodes(string tbxOutputDialect, string pathBuilder)
        {
            if (tbxOutputDialect == "TBX-Core")
            {
                stripForCore(pathBuilder);
            }
            else if (tbxOutputDialect == "TBX-Min")
            {
                stripForMin(pathBuilder);
            }
            else if (tbxOutputDialect == "TBX-Basic")
            {
                stripForBasic(pathBuilder);
            }
        }

        private void truncateDates()
        {
            XmlNodeList dates = multiTermDoc.SelectNodes("//date");
            foreach (XmlNode date in dates)
            {
                Match match = Regex.Match(date.InnerText, @"T[^<]*");
                string truncatedDate = match.Groups[0].Value;
                date.InnerText = date.InnerText.Replace(truncatedDate, "");
            }
        }

        // Pretty Printing Steps

        private void removeWhitespaceChildren(XmlNode root)
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
                    removeWhitespaceChildren(child);
                }
            }

            foreach (XmlNode node in childrenToRemove)
            {
                root.RemoveChild(node);
            }
        }

        // XML Conversion

        private XmlNode selectRoot(XmlReader reader)
        {
            XmlNode node;

            while (reader.Name != "mtf")
            {
                reader.Read();
            }

            node = multiTermDoc.ReadNode(reader);
            return node;
        }

        private void prettyPrintFile(string printPath)
        {
            multiTermDoc.DocumentElement.SetAttribute("xmlns", "urn:iso:std:iso:30042:ed:3.0");
            MemoryStream xmlStream = new MemoryStream();
            multiTermDoc.Save(xmlStream);
            xmlStream.Flush();
            xmlStream.Position = 0;
            multiTermDoc.Load(xmlStream);


            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            XmlWriter writer = XmlWriter.Create(printPath, settings);
            multiTermDoc.WriteTo(writer);
            writer.Flush();
        }

        private void addNameSpace(XmlNode currentNode)
        {
            XmlDocument transferDoc = new XmlDocument();
            //transferDoc.DocumentElement.SetAttribute("xmlns", "urn:iso:std:iso:30042:ed:3.0");
            //transferDoc.LoadXml(multiTermDoc.OuterXml);
            //multiTermDoc.DocumentElement.SetAttribute("xmlns", "urn:iso:std:iso:30042:ed:3.0");
        }

        private void convertXML(FileStream xmlData, string outputPath, LevelOneClass initialJSON, string tbxOutputDialect, string pathBuilder)
        {
            XmlReaderSettings readerSettings = new XmlReaderSettings();

            string dialect = initialJSON.getDialect();
            string xcs = initialJSON.getXCS();

            Dictionary<string, object> highLevelDictionaryStorage = new Dictionary<string, object>();
            highLevelDictionaryStorage = initialJSON.getMasterDictionary();

            Dictionary<string, string[]> queueOrders = new Dictionary<string, string[]>();
            queueOrders = initialJSON.getQueueOrders();

            using (XmlReader reader = XmlReader.Create(xmlData, readerSettings))
            {
                XmlNode root = selectRoot(reader);

                // Reorder Queue Bundling Pairs
                executeQueueBundlingOrders(queueOrders);

                // Output root and header for TBX V03
                modifyTBXHeader(xcs, root, tbxOutputDialect);
                root = multiTermDoc.LastChild;

                // Remove superfluous tab characters
                removeXref(root);

                // Reorder elements before parsing
                reorderXML(root);

                // Parse with the highLevelDictionaryStorage for invalid Data Categories
                parseForDataCategories(root, highLevelDictionaryStorage);

                // Pair necessary Node that may have been created during Data category parsing
                pairNodes();

                // Strip away content that does not belong to the User-Specified output
                stripInvalidNodes(tbxOutputDialect, pathBuilder);

                // Recursively remove built up white space
                removeWhitespaceChildren(multiTermDoc);

                // Truncate dates for XML Validation
                truncateDates();

                // Apply new XMLNS to finished file
                addNameSpace(multiTermDoc);

                // Output our constructed file
                prettyPrintFile(outputPath);
            }
        }

        public void deserializeFile(string mappingFile, string multiTermXML, string tbxOutputDialect, bool isCalledFromMappingWizard)
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
            initialJSON.parseCMap();

            // Import XML
            FileStream xmlData = File.OpenRead(multiTermXML);
            multiTermDoc = new XmlDocument();
            multiTermDoc.PreserveWhitespace = true;
            multiTermDoc.Load(multiTermXML);
            string pathBuilder = System.IO.Path.GetFileName(multiTermXML);
            string outputPath = multiTermXML.Replace(pathBuilder, "ConvertedTBX.tbx");

            convertXML(xmlData, outputPath, initialJSON, tbxOutputDialect, outputPath);
        }


        public static string readFile(string type)
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



