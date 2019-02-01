using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTermTBXMapper
{
    partial class Mapping
    {
        public string dialect = "";
        public string xcs = "";
        public CategoricalMapping catMap = new CategoricalMapping();
        public QueueDrainOrders queueDrainOrders = new QueueDrainOrders();
        public object empty = new object();

        //initialize Mapping object
        public Mapping(string dialect = "TBX-Default", string xcs = "TBXXCSV02.xcs")
        {
            this.dialect = dialect;
            this.xcs = xcs;
        }

        public string Serialize()
        {
            return string.Format("[\"{0}\", \"{1}\", {2}, {3}, {4}]", dialect, xcs, catMap.Serialize(), queueDrainOrders.Serialize(), "{}");
        }

        //interior function for grabbing the correct target based on XML representation
        public string getTarget(string elt)
        {
            switch (elt)
            {
                case "<termNote>":
                    return "termNote";
                case "<termComp>":
                    return "termComp";
                case "<termCompList>":
                    return "termCompList";
                case "<adminNote>":
                case "<transac>":
                case "<transacNote>":
                case "<xref>":
                case "<ref>":
                case "<hi>":
                case "<bpt>":
                case "<date>":
                case "<ept>":
                case "<foreign>":
                case "<ph>":
                case "<term>":
                case "<descrip>":
                case "<descripNote>":
                case "<admin>":
                case "<note>":
                    return "auxInfo";
                default:
                    return "unhandled";
            }
        }

        //function for constructing XML representation
        public string getEltAtt(string elt, string type)
        {
            switch(elt)
            {
                case "<adminNote>":
                    return getXMLnoContent("adminNote", type);
                case "<transac>":
                    return getXMLnoContent("transac", type);
                case "<transacNote>":
                    return getXMLnoContent("transacNote", type);
                case "<xref>":
                    return "<xref type='" + type + "' >see target</xref>";
                case "<ref>":
                    return getXMLnoContent("ref", type);
                case "<hi>":
                    return getXMLnoContent("hi", type);
                case "<bpt>":
                    return "<bpt />";
                case "<date>":
                    return "<date />";
                case "<ept>":
                    return "<ept />";
                case "<foreign>":
                    return "<foregin />";
                case "<ph>":
                    return "<ph />";
                case "<term>":
                    return "<term />";
                case "<descrip>":
                    return getXMLnoContent("descrip", type);
                case "<descripNote>":
                    return getXMLnoContent("descripNote", type);
                case "<admin>":
                    return getXMLnoContent("admin", type);
                case "<termNote>":
                    return getXMLnoContent("termNote", type);
                case "<termCompList>":
                    return getXMLnoContent("termCompList", type);
                case "<note>":
                    return "<note />";
                default:
                    return getXMLnoContent("unhandled", type);
            }
        }

        //function used to aide in constructing xml representations
        private static string getXMLnoContent(string elt, string type)
        {
            return "<" + elt + " type='" + type + "' />";
        }
    }

    class CategoricalMapping : Dictionary<string, OneLevel>
    {
        public CategoricalMapping()
        {
            Add("concept", new OneLevel("concept"));
            Add("language", new OneLevel("language"));
            Add("term", new OneLevel("term"));
        }

        public string Serialize()
        {
            string s = "{";
            int i = 0;
            foreach (string key in Keys)
            {
                if (i > 0)
                {
                    s += ",";
                }
                s += string.Format("\"{0}\": {1}", key, this[key].Serialize());
                i++;
            }
            s += "}";

            return s;
        }
    }

    // a "OneLevel" is just a simple way to refer to the concept/language/term key sections of the JSON
    class OneLevel : Dictionary<string, TemplateSet>
    {
        private class InvalidLevelException : Exception
        {
            public InvalidLevelException() : base("Only levels 'concept', 'language', and 'term' are allowed as OneLevel values.") { }
        }

        public OneLevel(string level)
        {
            if (level != "concept" && level != "language" && level != "term")
            {
                throw new InvalidLevelException();
            }
        }

        /// <summary>
        /// Serialize to JSON string
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            string s = "{";
            
            int i = 0;
            foreach(string k in Keys)
            {
                if (i > 0)
                {
                    s += ",";
                }
                s += string.Format("\"{0}\": {1}", k, this[k].Serialize());
                i++;
            }

            s += "}";
            return s;
        }

        //constructs the default OneLevel construction
        private void addDefault(string level)
        {
            switch(level)
            {
                case "concept":
                    Add("Subject", addSubject());
                    Add("Status", addStatus());
                    Add("Source", addSource());
                    Add("Note", addNote());
                    break;
                case "language":
                    Add("Definition", addDefinition());
                    Add("Note", addNote());
                    break;
                case "term":
                    Add("Status", addTermStatus());
                    Add("Note", addNote());
                    Add("Context", addContext());
                    Add("Grammatical Gender", addGrammaticalGender());
                    Add("Grammatical Number", addGrammaticalNumber());
                    Add("Usage Regiser", addUsageRegister());
                    Add("Part of Speech", addPartOfSpeech());
                    Add("Source", addSource());
                    Add("Category", addCategory());
                    break;
            }
        }

        private static string getAuxInfo()
        {
            return "auxInfo";
        }

        private static string getTermNote()
        {
            return "termNote";
        }

        private static string getAdmin()
        {
            return "admin";
        }

        private static string getUnhandled()
        {
            return "unhandled";
        }


        // shortcuts for adding typical MultiTerm fields
        private TemplateSet addSubject()
        {
            TemplateSet ts = new TemplateSet();
            ((ts[0] as KeyList)[0] as Teasp).setAll(getAuxInfo(), new DescripXML("subjectField"));
            return ts;
        }

        private TemplateSet addStatus()
        {
            TemplateSet ts = new TemplateSet();
            ((ts[0] as KeyList)[0] as Teasp).setAll(getAuxInfo(), new NoteXML(), "category tag");
            return ts;
        }

        private TemplateSet addSource()
        {
            TemplateSet ts = new TemplateSet();
            ((ts[0] as KeyList)[0] as Teasp).setAll(getAuxInfo(), new AdminXML("source"));
            return ts;
        }

        private TemplateSet addNote()
        {
            TemplateSet ts = new TemplateSet();
            ((ts[0] as KeyList)[0] as Teasp).setAll(getAuxInfo(), new NoteXML());
            return ts;
        }

        private TemplateSet addDefinition()
        {
            TemplateSet ts = new TemplateSet();
            ((ts[0] as KeyList)[0] as Teasp).setAll(getAuxInfo(), new DescripXML("definition"));
            return ts;
        }

        private TemplateSet addTermStatus()
        {
            TemplateSet ts = new TemplateSet();
            ((ts[0] as KeyList)[0] as Teasp).setAll(getUnhandled(), new AdminXML("Status"));

            ValueGroup vg1 = new ValueGroup() { "new", "nonstandardized", "proposed", "recommended" };
            ValueGroup vg2 = new ValueGroup() { "admitted", "deprecated", "legal", "preferred", "regulated", "standardized", "superseded" };
            (ts[0] as KeyList).addValueGroup(vg1);
            (ts[0] as KeyList).addValueGroup(vg2);

            Teasp specialTeasp1 = new Teasp();
            specialTeasp1.setAll("termNote", new TermNoteXML("language-planningQualifier"), 
                new Dictionary<string, string>()
                {
                    { "nonstandardized", "nonstandardizedTerm" },
                    { "proposed", "proposedTerm" },
                    { "new", "newTerm" },
                    { "recommended", "recommendedTerm" }
                }
            );
            Teasp specialTeasp2 = new Teasp();
            specialTeasp2.setAll("termNote", new TermNoteXML("administrativeStatus"),
                new Dictionary<string, string>()
                {
                    { "deprecated", "deprecatedTerm-admn-sts" },
                    { "regulated", "regulatedTerm-admn-sts" },
                    { "admitted", "admittedTerm-admn-sts" },
                    { "standardized", "standardizedTerm-admin-sts" },
                    { "legal", "legalTerm-admn-sts" },
                    { "superseded", "superseded-admn-sts" },
                    { "preferred", "preferredTerm-admn-sts" }
                }
            );

            ts.addSpecialTeasp(specialTeasp1);
            ts.addSpecialTeasp(specialTeasp2);

            return ts;
        }

        private TemplateSet addContext()
        {
            TemplateSet ts = new TemplateSet();
            ((ts[0] as KeyList)[0] as Teasp).setAll(getAuxInfo(), new DescripXML("context"));
            return ts;
        }

        private TemplateSet addGrammaticalGender()
        {
            TemplateSet ts = new TemplateSet();
            ((ts[0] as KeyList)[0] as Teasp).setAll(getTermNote(), new TermNoteXML("grammaticalGender"), 
                new Dictionary<string, string>()
                {
                    { "other", "otherGender" }
                }
            );
            return ts;
        }

        private TemplateSet addGrammaticalNumber()
        {
            TemplateSet ts = new TemplateSet();
            ((ts[0] as KeyList)[0] as Teasp).setAll(getTermNote(), new TermNoteXML("grammaticalNumber"), 
                new Dictionary<string, string>()
                {
                    { "other", "otherNumber" }
                }
            );
            return ts;
        }

        private TemplateSet addUsageRegister()
        {
            TemplateSet ts = new TemplateSet();
            ((ts[0] as KeyList)[0] as Teasp).setAll(getTermNote(), new TermNoteXML("register"),
                new Dictionary<string, string>()
                {
                    { "slang", "slangRegister" },
                    { "in-house", "in-houseRegister" },
                    { "bench-level", "bench-levelRegister" },
                    { "vulgar", "vulgarRegister" },
                    { "technical", "technicalRegister" },
                    { "neutral", "neutralRegister" }
                }
                
            );
            return ts;
        }

        private TemplateSet addPartOfSpeech()
        {
            TemplateSet ts = new TemplateSet();
            ((ts[0] as KeyList)[0] as Teasp).setAll(getTermNote(), new TermNoteXML("partOfSpeech"));
            return ts;
        }

        private TemplateSet addCategory()
        {
            TemplateSet ts = new TemplateSet();
            ((ts[0] as KeyList)[0] as Teasp).setAll(getUnhandled(), new TermNoteXML("unknownTermType"));

            ValueGroup vg1 = new ValueGroup() { "abbreviation","acronym","equation","formula","internationalism","symbol"};
            ValueGroup vg2 = new ValueGroup() { "common name", "full form", "international scientific term", "part number", "short form", "transcribed form", "transliterated form" };
            ValueGroup vg3 = new ValueGroup() { "phraseologism", "stock keeping unit", "orthographical variant" };
            ValueGroup vg4 = new ValueGroup() { "antonym" };
            (ts[0] as KeyList).addValueGroup(vg1);
            (ts[0] as KeyList).addValueGroup(vg2);
            (ts[0] as KeyList).addValueGroup(vg3);
            (ts[0] as KeyList).addValueGroup(vg4);

            TermNoteXML xml = new TermNoteXML("termType");

            Teasp specialTeasp1 = new Teasp();
            specialTeasp1.setAll(getTermNote(), xml);
            Teasp specialTeasp2 = new Teasp();
            specialTeasp2.setAll(getTermNote(), xml, "camel case");
            Teasp specialTeasp3 = new Teasp();
            specialTeasp3.setAll(getTermNote(), xml,
                new Dictionary<string, string>()
                {
                    { "stock keeping unit", "sku" },
                    { "phraseologism", "phraseologicalUnit" },
                    { "orthographical variant", "variant" }
                }
            );
            Teasp specialTeasp4 = new Teasp();
            specialTeasp4.setAll(getTermNote(), new TermNoteXML("antonymTerm"), placement: "null");

            ts.addSpecialTeasp(specialTeasp1);
            ts.addSpecialTeasp(specialTeasp2);
            ts.addSpecialTeasp(specialTeasp3);
            ts.addSpecialTeasp(specialTeasp4);

            return ts;
        }
    }

    class TemplateSet : List<object>
    {
        public TemplateSet()
        {
            Add(new KeyList());
        }
        /// <summary>
        /// Serialize into JSON string
        /// </summary>
        public string Serialize()
        {
            string s = string.Format("[{0}", (this[0] as KeyList).Serialize());

            if (Count > 1)
            {
                foreach(Teasp teasp in GetRange(1, Count-1))
                {
                    s += string.Format(", {0}", teasp.Serialize());
                }
            }

            s += "]";

            return s;
        }

        /// <summary>
        /// Add a lisp of special teasps to the TemplateSet
        /// </summary>
        /// <param name="teasps">List of Teasps</param>
        public void addSpecialTeasps(List<Teasp> teasps)
        {
            foreach(Teasp t in teasps)
            {
                addSpecialTeasp(t);
            }
        }

        /// <summary>
        /// Add a special Teasp to the TemplateSet
        /// </summary>
        /// <param name="teasp">A Teasp object</param>
        public void addSpecialTeasp(Teasp teasp)
        {
            Add(teasp);
        }

        /// <summary>
        /// Shortcut method for adding multiple value groups to the KeyList in a TemplateSet
        /// </summary>
        /// <param name="vgs"></param>
        public void addValueGroups(List<ValueGroup> vgs)
        {
            (this[0] as KeyList).addValueGroups(vgs);
        }

        /// <summary>
        /// Shortcut method for adding ValueGroups to the Keylist in a TemplateSet
        /// </summary>
        /// <param name="group">ValueGroup object</param>
        public void addValueGroup(ValueGroup vg)
        {
            (this[0] as KeyList).addValueGroup(vg);
        }
    }

    class KeyList : List<object>
    {
        public KeyList()
        {
            Add(new Teasp());
        }

        public string Serialize()
        {
            string s = string.Format("[{0}", (this[0] as Teasp).Serialize());
            if (Count > 1)
            {
                foreach (ValueGroup vg in GetRange(1, Count-1))
                {
                    s += string.Format(",{0}", vg.Serialize());
                }
            }
            s += "]";
            return s;
        }

        /// <summary>
        /// method for adding multiple value groups to the KeyList
        /// </summary>
        /// <param name="vgs"></param>
        public void addValueGroups(List<ValueGroup> vgs)
        {
            foreach (ValueGroup vg in vgs)
            {
                addValueGroup(vg);
            }
        }

        /// <summary>
        /// method for adding individual ValueGroups to the Keylist
        /// </summary>
        /// <param name="group">ValueGroup object</param>
        public void addValueGroup(ValueGroup vg)
        {
            Add(vg);
        }
    }

    class Teasp
    {
        public object[] teasp = new object[4];

        public string Serialize()
        {
            return string.Format("[\"{0}\",\"{1}\", {2},\"{3}\"]", teasp[0], teasp[1], teasp[2], teasp[3]);
        }

        /// <summary>
        /// setAll overflow for TEASPs that contain substitutions
        /// </summary>
        /// <param name="target"></param>
        /// <param name="xml"></param>
        /// <param name="sub"></param>
        /// <param name="placement"></param>
        public void setAll(string target, string xml, Dictionary<string,string> sub, string placement = "content")
        {
            setTarget(target);
            setEltAtt(xml);
            setSub(sub);
            setPlacement(placement);
        }

        /// <summary>
        ///  setAll overflow for TEASPs with no substitutions
        /// </summary>
        /// <param name="target"></param>
        /// <param name="xml"></param>
        /// <param name="sub"></param>
        /// <param name="placement"></param>
        public void setAll(string target, string xml, string sub = "null", string placement = "content")
        {
            setTarget(target);
            setEltAtt(xml);
            setSub(sub);
            setPlacement(placement);
        }

        public void setTarget(string target)
        {
            teasp[0] = "@" + target;
        }

        public void setEltAtt(string xml)
        {
            teasp[1] = xml;
        }

        public void setSub(Dictionary<string,string> sub)
        {
            if (sub != null)
            {
                //Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();
                teasp[2] = "{";
                //teasp[2] = Newtonsoft.Json.JsonConvert.SerializeObject(sub);
                int i = 0;
                foreach(string k in sub.Keys)
                {
                    if (i > 0)
                    {
                        teasp[2] += ",";
                    }
                    teasp[2] += string.Format("\"{0}\": \"{1}\"", k, sub[k]);
                    i++;
                }
                teasp[2] += "}";
            }
            else
            {
                teasp[2] = "\"null\"";
            }
        }

        public void setSub(string sub = "null")
        {
            teasp[2] = string.Format("\"{0}\"",sub);
        }

        public void setPlacement(string placement)
        {
            teasp[3] = placement;
        }

        public void setUnhandled()
        {
            setAll("unhandled", new TermNoteXML("unknownTermType"));
        }
    }

    /// <summary>
    /// A ValueGroup is a list which contains contents of user data categories which are all mapped to the same TBX data category.
    /// </summary>
    class ValueGroup : List<string>
    {
        public string Serialize()
        {
            string s = "[";

            for (int i = 0; i < Count; i++)
            {
                if (i > 0)
                {
                    s += ", ";
                }

                s += string.Format("\"{0}\"", this[i]);
            }
            s += "]";

            return s;
        }

    }

    public class QueueDrainOrders : Dictionary<string, QueueDrainLevel>
    {
        public QueueDrainOrders()
        {
            Add("conceptGrp", new QueueDrainLevel());
            Add("languageGrp", new QueueDrainLevel());
            Add("termGrp", new QueueDrainLevel());
        }

        public void AddOrder(string[] order)
        {
            QueueDrainOrder queueOrder = new QueueDrainOrder(order);

            switch(order[3])
            {
                case "conceptGrp":
                    this["conceptGrp"].Add(queueOrder);
                    break;
                case "languageGrp":
                    this["languageGrp"].Add(queueOrder);
                    break;
                case "termGrp":
                    this["termGrp"].Add(queueOrder);
                    break;
            }
        }
        
        public string Serialize()
        {
            if (this["conceptGrp"].Count < 1 && this["languageGrp"].Count < 1 && this["termGrp"].Count < 1)
            {
                return "{}";
            }

            return string.Format("{{\"conceptGrp\":{0}, \"languageGrp\":{1}, \"termGrp\":{2}}}", this["conceptGrp"].Serialize(), this["languageGrp"].Serialize(), this["termGrp"].Serialize());
        }
    }

    public class QueueDrainLevel : List<QueueDrainOrder>
    {
        public string Serialize()
        {
            string s = "[";

            for(int i = 0; i < Count; i++)
            {
                if (i > 0)
                {
                    s += ",";
                }
                s += string.Format("{0}", this[i].Serialize());
            }
            s += "]";

            return s;
        }
    }

    public class QueueDrainOrder : List<string>
    {
        public QueueDrainOrder(string[] values)
        {
            Add(values[0]);
            Add(values[1]);
            Add(values[2]);
        }

        public string Serialize()
        {
            if (Count > 0)
            {
                return string.Format("[\"{0}\",\"{1}\",\"{2}\"]", this[0], this[1], this[2]);
            }
            else
            {
                return "[]";
            }
        }

        //public bool addRule(int index, string rule)
        //{
        //    if (-1 < index && index > 2)
        //    {
        //        return false;
        //    }

        //    this[index] = rule;
        //    return true;
        //}

        //public void addRules(string rule1, string rule2, string rule3)
        //{
        //    this[0] = rule1;
        //    this[1] = rule2;
        //    this[2] = rule3;
        //}
    }
}
