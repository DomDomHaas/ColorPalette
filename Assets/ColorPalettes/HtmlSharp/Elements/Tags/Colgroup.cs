using System;
using System.Collections.Generic;

namespace HtmlSharp.Elements.Tags
{
    public class ColGroup : Tag
    {
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

        public string Span { get { return this["span"]; } }

        public string Style { get { return this["style"]; } }

        public string Title { get { return this["title"]; } }

        public string Valign { get { return this["valign"]; } }

        public string Width { get { return this["width"]; } }

        public ColGroup()
            : this(new Element[0])
        {
        }

        public ColGroup(params Element[] children)
            : this(new TagAttribute[0], children)
        {
        }

        public ColGroup(params TagAttribute[] attributes)
            : this(attributes, new Element[0])
        {
        }

        public ColGroup(IEnumerable<TagAttribute> attributes, params Element[] children)
            : base(attributes, children)
        {
            TagName = "colgroup";
        }
    }
}