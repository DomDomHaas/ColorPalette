using System;
using System.Collections.Generic;

namespace HtmlSharp.Elements.Tags
{
    public class Link : Tag
    {
        public string Charset { get { return this["charset"]; } }

        public string Class { get { return this["class"]; } }

        public string Dir { get { return this["dir"]; } }

        public string Href { get { return this["href"]; } }

        public string Hreflang { get { return this["hreflang"]; } }

        public string Id { get { return this["id"]; } }

        public string Lang { get { return this["lang"]; } }

        public string Media { get { return this["media"]; } }

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

        public string Rel { get { return this["rel"]; } }

        public string Rev { get { return this["rev"]; } }

        public string Style { get { return this["style"]; } }

        public string Target { get { return this["target"]; } }

        public string Title { get { return this["title"]; } }

        public string Type { get { return this["type"]; } }

        public Link()
            : this(new Element[0])
        {
        }

        public Link(params Element[] children)
            : this(new TagAttribute[0], children)
        {
        }

        public Link(params TagAttribute[] attributes)
            : this(attributes, new Element[0])
        {
        }

        public Link(IEnumerable<TagAttribute> attributes, params Element[] children)
            : base(attributes, children)
        {
            IsSelfClosing = true;
            TagName = "link";
        }
    }
}