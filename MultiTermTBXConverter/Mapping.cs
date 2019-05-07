using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTermTBXMapper
{
    public static class SerializationHelper
    {
        /// <summary>
        /// The SerializationHelper.Serialize method is a way to run Select method on an IEnumerable<T>, 
        /// send the contents to a "," delimited string, and surround that string with brackets.
        /// </summary>
        /// <typeparam name="TSource">Any T</typeparam>
        /// <typeparam name="TResult">Any T</typeparam>
        /// <param name="bracketType">'()', '{}', '<>', or '[]'</param>
        /// <param name="collection">An IEnumerable on which the Select() method can be run.</param>
        /// <param name="selector">The Func<> which is to be used in the Select() method.</param>
        /// <returns></returns>
        public static string Serialize<TSource, TResult>(string bracketType, IEnumerable<TSource> collection, Func<TSource, TResult> selector)
        {
            string leftBracket;
            string rightBracket;

            Exception InvalidBracketException = new Exception("Parameter 'bracketType' must be one of '()', '{}', '<>', or '[]'");

            switch (bracketType)
            {
                case "()":
                    leftBracket = "(";
                    rightBracket = ")";
                    break;
                case "[]":
                    leftBracket = "[";
                    rightBracket = "]";
                    break;
                case "{}":
                    leftBracket = "{";
                    rightBracket = "}";
                    break;
                case "<>":
                    leftBracket = "<";
                    rightBracket = ">";
                    break;
                default:
                    throw InvalidBracketException;
            }


            var selection = collection.Select(selector);
            StringBuilder s = new StringBuilder(leftBracket);
            s.Append(String.Join(",", selection));
            s.Append(rightBracket);

            return s.ToString();
        }
    }

    partial class Mapping : ISerializable
    {
        public string Dialect { get; set; } = "";
        public string XCS { get; set; } = "";
        public CategoricalMapping CatMap { get; set; } = new CategoricalMapping();
        public QueueDrainOrders QueueDrainOrders { get; set; } = new QueueDrainOrders();
        public object Empty { get; set; } = new object();

        //initialize Mapping object
        public Mapping(string dialect = "TBX-Default", string xcs = "TBXXCSV02.xcs")
        {
            Dialect = dialect;
            XCS = xcs;
        }

        #region ISerializable members
        public string Serialize()
        {
            List<string> items = new List<string>() { $@"""{Dialect}""", $@"""{XCS}""", CatMap.Serialize(), QueueDrainOrders.Serialize(), "{}" };
            return SerializationHelper.Serialize("[]", items, x => x );
        }
        #endregion

        //interior function for grabbing the correct target based on XML representation
        public string GetTarget(string elt)
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
        public string GetEltAtt(string elt, string type)
        {
            switch(elt)
            {
                case "<adminNote>":
                    return GetXMLnoContent("adminNote", type);
                case "<transac>":
                    return GetXMLnoContent("transac", type);
                case "<transacNote>":
                    return GetXMLnoContent("transacNote", type);
                case "<xref>":
                    return "<xref type='" + type + "' >see target</xref>";
                case "<ref>":
                    return GetXMLnoContent("ref", type);
                case "<hi>":
                    return GetXMLnoContent("hi", type);
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
                    return GetXMLnoContent("descrip", type);
                case "<descripNote>":
                    return GetXMLnoContent("descripNote", type);
                case "<admin>":
                    return GetXMLnoContent("admin", type);
                case "<termNote>":
                    return GetXMLnoContent("termNote", type);
                case "<termCompList>":
                    return GetXMLnoContent("termCompList", type);
                case "<note>":
                    return "<note />";
                default:
                    return GetXMLnoContent("unhandled", type);
            }
        }

        //function used to aide in constructing xml representations
        private static string GetXMLnoContent(string elt, string type)
        {
            return $"<{elt} type='{type}' />";
        }
    }

    class CategoricalMapping : Dictionary<string, OneLevel>, ISerializable
    {
        public CategoricalMapping()
        {
            Add("concept", new OneLevel("concept"));
            Add("language", new OneLevel("language"));
            Add("term", new OneLevel("term"));
        }

        #region ISerializable members
        public string Serialize() => SerializationHelper.Serialize("{}", this, x => $@"""{x.Key}"": {x.Value.Serialize()}");
        #endregion
    }

    // a "OneLevel" is just a simple way to refer to the concept/language/term key sections of the JSON
    class OneLevel : Dictionary<string, TemplateSet>, ISerializable
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

        #region ISerializable members
        /// <summary>
        /// Serialize to JSON string
        /// </summary>
        /// <returns></returns>
        public string Serialize() => SerializationHelper.Serialize("{}", this, x => $@"""{x.Key}"": {x.Value.Serialize()}");
        #endregion

        //constructs the default OneLevel construction
        private void AddDefault(string level)
        {
            switch(level)
            {
                case "concept":
                    Add("Subject", AddSubject());
                    Add("Status", AddStatus());
                    Add("Source", AddSource());
                    Add("Note", AddNote());
                    break;
                case "language":
                    Add("Definition", AddDefinition());
                    Add("Note", AddNote());
                    break;
                case "term":
                    Add("Status", AddTermStatus());
                    Add("Note", AddNote());
                    Add("Context", AddContext());
                    Add("Grammatical Gender", AddGrammaticalGender());
                    Add("Grammatical Number", AddGrammaticalNumber());
                    Add("Usage Regiser", AddUsageRegister());
                    Add("Part of Speech", AddPartOfSpeech());
                    Add("Source", AddSource());
                    Add("Category", AddCategory());
                    break;
            }
        }

        private static string GetAuxInfo()
        {
            return "auxInfo";
        }

        private static string GetTermNote()
        {
            return "termNote";
        }

        private static string GetAdmin()
        {
            return "admin";
        }

        private static string GetUnhandled()
        {
            return "unhandled";
        }


        // shortcuts for adding typical MultiTerm fields
        private TemplateSet AddSubject()
        {
            TemplateSet ts = new TemplateSet();
            ((ts[0] as KeyList)[0] as Teasp).SetAll(GetAuxInfo(), new DescripXML("subjectField"));
            return ts;
        }

        private TemplateSet AddStatus()
        {
            TemplateSet ts = new TemplateSet();
            ((ts[0] as KeyList)[0] as Teasp).SetAll(GetAuxInfo(), new NoteXML(), "category tag");
            return ts;
        }

        private TemplateSet AddSource()
        {
            TemplateSet ts = new TemplateSet();
            ((ts[0] as KeyList)[0] as Teasp).SetAll(GetAuxInfo(), new AdminXML("source"));
            return ts;
        }

        private TemplateSet AddNote()
        {
            TemplateSet ts = new TemplateSet();
            ((ts[0] as KeyList)[0] as Teasp).SetAll(GetAuxInfo(), new NoteXML());
            return ts;
        }

        private TemplateSet AddDefinition()
        {
            TemplateSet ts = new TemplateSet();
            ((ts[0] as KeyList)[0] as Teasp).SetAll(GetAuxInfo(), new DescripXML("definition"));
            return ts;
        }

        private TemplateSet AddTermStatus()
        {
            TemplateSet ts = new TemplateSet();
            ((ts[0] as KeyList)[0] as Teasp).SetAll(GetUnhandled(), new AdminXML("Status"));

            ValueGroup vg1 = new ValueGroup() { "new", "nonstandardized", "proposed", "recommended" };
            ValueGroup vg2 = new ValueGroup() { "admitted", "deprecated", "legal", "preferred", "regulated", "standardized", "superseded" };
            (ts[0] as KeyList).AddValueGroup(vg1);
            (ts[0] as KeyList).AddValueGroup(vg2);

            Teasp specialTeasp1 = new Teasp();
            specialTeasp1.SetAll("termNote", new TermNoteXML("language-planningQualifier"), 
                new Dictionary<string, string>()
                {
                    { "nonstandardized", "nonstandardizedTerm" },
                    { "proposed", "proposedTerm" },
                    { "new", "newTerm" },
                    { "recommended", "recommendedTerm" }
                }
            );
            Teasp specialTeasp2 = new Teasp();
            specialTeasp2.SetAll("termNote", new TermNoteXML("administrativeStatus"),
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

            ts.AddSpecialTeasp(specialTeasp1);
            ts.AddSpecialTeasp(specialTeasp2);

            return ts;
        }

        private TemplateSet AddContext()
        {
            TemplateSet ts = new TemplateSet();
            ((ts[0] as KeyList)[0] as Teasp).SetAll(GetAuxInfo(), new DescripXML("context"));
            return ts;
        }

        private TemplateSet AddGrammaticalGender()
        {
            TemplateSet ts = new TemplateSet();
            ((ts[0] as KeyList)[0] as Teasp).SetAll(GetTermNote(), new TermNoteXML("grammaticalGender"), 
                new Dictionary<string, string>()
                {
                    { "other", "otherGender" }
                }
            );
            return ts;
        }

        private TemplateSet AddGrammaticalNumber()
        {
            TemplateSet ts = new TemplateSet();
            ((ts[0] as KeyList)[0] as Teasp).SetAll(GetTermNote(), new TermNoteXML("grammaticalNumber"), 
                new Dictionary<string, string>()
                {
                    { "other", "otherNumber" }
                }
            );
            return ts;
        }

        private TemplateSet AddUsageRegister()
        {
            TemplateSet ts = new TemplateSet();
            ((ts[0] as KeyList)[0] as Teasp).SetAll(GetTermNote(), new TermNoteXML("register"),
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

        private TemplateSet AddPartOfSpeech()
        {
            TemplateSet ts = new TemplateSet();
            ((ts[0] as KeyList)[0] as Teasp).SetAll(GetTermNote(), new TermNoteXML("partOfSpeech"));
            return ts;
        }

        private TemplateSet AddCategory()
        {
            TemplateSet ts = new TemplateSet();
            ((ts[0] as KeyList)[0] as Teasp).SetAll(GetUnhandled(), new TermNoteXML("unknownTermType"));

            ValueGroup vg1 = new ValueGroup() { "abbreviation","acronym","equation","formula","internationalism","symbol"};
            ValueGroup vg2 = new ValueGroup() { "common name", "full form", "international scientific term", "part number", "short form", "transcribed form", "transliterated form" };
            ValueGroup vg3 = new ValueGroup() { "phraseologism", "stock keeping unit", "orthographical variant" };
            ValueGroup vg4 = new ValueGroup() { "antonym" };
            (ts[0] as KeyList).AddValueGroup(vg1);
            (ts[0] as KeyList).AddValueGroup(vg2);
            (ts[0] as KeyList).AddValueGroup(vg3);
            (ts[0] as KeyList).AddValueGroup(vg4);

            TermNoteXML xml = new TermNoteXML("termType");

            Teasp specialTeasp1 = new Teasp();
            specialTeasp1.SetAll(GetTermNote(), xml);
            Teasp specialTeasp2 = new Teasp();
            specialTeasp2.SetAll(GetTermNote(), xml, "camel case");
            Teasp specialTeasp3 = new Teasp();
            specialTeasp3.SetAll(GetTermNote(), xml,
                new Dictionary<string, string>()
                {
                    { "stock keeping unit", "sku" },
                    { "phraseologism", "phraseologicalUnit" },
                    { "orthographical variant", "variant" }
                }
            );
            Teasp specialTeasp4 = new Teasp();
            specialTeasp4.SetAll(GetTermNote(), new TermNoteXML("antonymTerm"), placement: "null");

            ts.AddSpecialTeasp(specialTeasp1);
            ts.AddSpecialTeasp(specialTeasp2);
            ts.AddSpecialTeasp(specialTeasp3);
            ts.AddSpecialTeasp(specialTeasp4);

            return ts;
        }
    }

    class TemplateSet : List<object>, ISerializable
    {
        public TemplateSet()
        {
            Add(new KeyList());
        }

        #region ISerializable members
        /// <summary>
        /// Serialize into JSON string
        /// </summary>
        public string Serialize()
        {
            IEnumerable<string> items = from x in this
                                        select (x as ISerializable).Serialize();

            return SerializationHelper.Serialize("[]", items, x => x);
        }
        #endregion

        /// <summary>
        /// Add a lisp of special teasps to the TemplateSet
        /// </summary>
        /// <param name="teasps">List of Teasps</param>
        public void AddSpecialTeasps(List<Teasp> teasps)
        {
            foreach(Teasp t in teasps)
            {
                AddSpecialTeasp(t);
            }
        }

        /// <summary>
        /// Add a special Teasp to the TemplateSet
        /// </summary>
        /// <param name="teasp">A Teasp object</param>
        public void AddSpecialTeasp(Teasp teasp)
        {
            Add(teasp);
        }

        /// <summary>
        /// Shortcut method for adding multiple value groups to the KeyList in a TemplateSet
        /// </summary>
        /// <param name="vgs"></param>
        public void AddValueGroups(List<ValueGroup> vgs)
        {
            (this[0] as KeyList).AddValueGroups(vgs);
        }

        /// <summary>
        /// Shortcut method for adding ValueGroups to the Keylist in a TemplateSet
        /// </summary>
        /// <param name="group">ValueGroup object</param>
        public void AddValueGroup(ValueGroup vg)
        {
            (this[0] as KeyList).AddValueGroup(vg);
        }
    }

    class KeyList : List<object>, ISerializable
    {
        public KeyList()
        {
            Add(new Teasp());
        }

        #region ISerializable members
        public string Serialize()
        {
            IEnumerable<string> items = from item in this
                                        select (item as ISerializable).Serialize();

            return SerializationHelper.Serialize("[]", items, x => x);
        }
        #endregion

        /// <summary>
        /// method for adding multiple value groups to the KeyList
        /// </summary>
        /// <param name="vgs"></param>
        public void AddValueGroups(List<ValueGroup> vgs)
        {
            foreach (ValueGroup vg in vgs)
            {
                AddValueGroup(vg);
            }
        }

        /// <summary>
        /// method for adding individual ValueGroups to the Keylist
        /// </summary>
        /// <param name="group">ValueGroup object</param>
        public void AddValueGroup(ValueGroup vg)
        {
            Add(vg);
        }
    }

    class Teasp : ISerializable
    {
        public string[] TEASP { get; set; } = new string[4];

        #region ISerializable members
        public string Serialize () => SerializationHelper.Serialize("[]", TEASP, x => (Array.IndexOf(TEASP, x) != 2) ? $@"""{x}""" : x);
        #endregion

        /// <summary>
        /// setAll overflow for TEASPs that contain substitutions
        /// </summary>
        /// <param name="target"></param>
        /// <param name="xml"></param>
        /// <param name="sub"></param>
        /// <param name="placement"></param>
        public void SetAll(string target, string xml, Dictionary<string,string> sub, string placement = "content")
        {
            SetTarget(target);
            SetEltAtt(xml);
            SetSub(sub);
            SetPlacement(placement);
        }

        /// <summary>
        ///  setAll overflow for TEASPs with no substitutions
        /// </summary>
        /// <param name="target"></param>
        /// <param name="xml"></param>
        /// <param name="sub"></param>
        /// <param name="placement"></param>
        public void SetAll(string target, string xml, string sub = "null", string placement = "content")
        {
            SetTarget(target);
            SetEltAtt(xml);
            SetSub(sub);
            SetPlacement(placement);
        }

        public void SetTarget(string target)
        {
            TEASP[0] = $"@{target}";
        }

        public void SetEltAtt(string xml)
        {
            TEASP[1] = xml;
        }

        public void SetSub(Dictionary<string,string> sub)
        {
            if (sub != null)
            {
                TEASP[2] = SerializationHelper.Serialize("{}", sub, x => $@"""{x.Key}"": ""{x.Value}""");
            }
            else
            {
                TEASP[2] = "\"null\"";
            }
        }

        public void SetSub(string sub = "null")
        {
            TEASP[2] = $@"""{sub}""";
        }

        public void SetPlacement(string placement)
        {
            TEASP[3] = placement;
        }

        public void SetUnhandled()
        {
            SetAll("unhandled", new TermNoteXML("unknownTermType"));
        }
    }

    /// <summary>
    /// A ValueGroup is a list which contains contents of user data categories which are all mapped to the same TBX data category.
    /// </summary>
    class ValueGroup : List<string>, ISerializable
    {
        #region ISerializable members
        public string Serialize()
        {
            return SerializationHelper.Serialize("[]", this, x => $@"""{x}""");
        }
        #endregion

    }

    public class QueueDrainOrders : Dictionary<string, QueueDrainLevel>, ISerializable
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

        #region ISerializable members
        public string Serialize()
        {
            if (this["conceptGrp"].Count < 1 && this["languageGrp"].Count < 1 && this["termGrp"].Count < 1)
            {
                return "{}";
            }

            return SerializationHelper.Serialize("{}", this, x => $@"""{x.Key}"": {x.Value.Serialize()}");
        }
        #endregion
    }

    public class QueueDrainLevel : List<QueueDrainOrder>, ISerializable
    {
        #region ISerializable members
        public string Serialize()
        {
            return SerializationHelper.Serialize("[]", this, x => x.Serialize());
        }
        #endregion
    }

    public class QueueDrainOrder : List<string>, ISerializable
    {
        public QueueDrainOrder(string[] values)
        {
            Add(values[0]);
            Add(values[1]);
            Add(values[2]);
        }

        #region ISerializable members
        public string Serialize()
        {
            if (Count > 0)
            {
                return SerializationHelper.Serialize("[]", this, x => $@"""{x}""");
            }
            else
            {
                return "[]";
            }
        }
        #endregion
    }
}
