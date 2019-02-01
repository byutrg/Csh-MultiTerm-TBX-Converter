using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTermTBXMapper
{
    class AdminXML
    {
        readonly string _value;

        public AdminXML(string value)
        {
            _value = "<admin type='" + value + "' />";
        }

        public static implicit operator string(AdminXML admin)
        {
            return admin._value;
        }
        public static implicit operator AdminXML(string admin)
        {
            return new AdminXML(admin);
        }
    }

    class DescripXML
    {
        readonly string _value;

        public DescripXML(string value)
        {
            _value = "<descrip type='" + value + "' />";
        }

        public static implicit operator string(DescripXML descrip)
        {
            return descrip._value;
        }
        public static implicit operator DescripXML(string descrip)
        {
            return new DescripXML(descrip);
        }
    }

    class TermNoteXML
    {
        readonly string _value;

        public TermNoteXML(string value)
        {
            _value = "<termNote type='" + value + "' />";
        }

        public static implicit operator string(TermNoteXML termNote)
        {
            return termNote._value;
        }
        public static implicit operator TermNoteXML(string termNote)
        {
            return new TermNoteXML(termNote);
        }
    }

    class NoteXML
    {
        readonly string _value;

        public NoteXML()
        {
            _value = "<note />";
        }

        public static implicit operator string(NoteXML note)
        {
            return note._value;
        }

        public static implicit operator NoteXML(string n = null)
        {
            return new NoteXML();
        }
    }

    class XrefXML
    {
        readonly string _value;

        public XrefXML(string value)
        {
            _value = "<xref type='" + value + "' >see target</xref>";
        }

        public static implicit operator string(XrefXML xref)
        {
            return xref._value;
        }

        public static implicit operator XrefXML(string xref)
        {
            return new XrefXML(xref);
        }
    }

    class RefXML
    {
        readonly string _value;

        public RefXML(string value)
        {
            _value = "<ref type='" + value + "' />";
        }

        public static implicit operator string(RefXML @ref)
        {
            return @ref._value;
        }

        public static implicit operator RefXML(string @ref)
        {
            return new RefXML(@ref);
        }
    }

    class TermCompListXML
    {
        readonly string _value;

        public TermCompListXML(string value)
        {
            _value = "<termCompList type='" + value + "' />";
        }

        public static implicit operator string(TermCompListXML xml)
        {
            return xml._value;
        }

        public static implicit operator TermCompListXML(string xml)
        {
            return new TermCompListXML(xml);
        }
    }    
}
