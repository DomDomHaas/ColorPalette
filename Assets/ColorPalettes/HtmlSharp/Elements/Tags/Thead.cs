using System;
using System.Collections.Generic;

namespace HtmlSharp.Elements.Tags
{
    public class THead : Tag, IAllowsNestingSelf
    {
        public IEnumerable<Type> NestingBreakers { get { return new Type[] { typeof(Table) }; } }

        public string Align { get { return this["align"]; } }

        public string Char { get { return this["char"]; } }

        public string Charoff { get { return this["charoff"]; } }

        public string Class { get { return this["class"]; } }

        public string Dir { get { return this["dir"]; } }

        public string Id { get { return this["id"]; } }

        public string Lang { get { return this["lang"]; } }

        public string Onclick { get { return this["onclick"]; } }

        public string Ondblclick { get { return this["ondblclick"]; } }

        public string Onkeydown { get { return this["onkeydown"]; } }

        public string Onkeypress { get { return this["onkeypress"]; } }

        public string Onkeyup { get { return this["onkeyup"]; } }

        public string Onmousedown { get { return this["onmousedown"]; } }

        public string Onmousemove { get { return this["onmousemove"]; } }

        public string Onmouseout { get { return this["onmouseout"]; } }

        public string Onmouseover { get { return this["onmouseover"]; } }

        public string Onmouseup { get { return this["onmouseup"]; } }

        public string Style { get { return this["style"]; } }

        public string Title { get { return this["title"]; } }

        public string Valign { get { return this["valign"]; } }

        public THead()
            : this(new Element[0])
        {
        }

        public THead(params Element[] children)
            : this(new TagAttribute[0], children)
        {
        }

        public THead(params TagAttribute[] attributes)
            : this(attributes, new Element[0])
        {
        }

        public THead(IEnumerable<TagAttribute> attributes, params Element[] children)
            : base(attributes, children)
        {
            ResetsNesting = true;
            TagName = "thead";
        }
    }
}