using System;
using System.Collections.Generic;

namespace HtmlSharp.Elements.Tags
{
    public class Area : Tag
    {
        public string Accesskey { get { return this["accesskey"]; } }

        public string Alt { get { return this["alt"]; } }

        public string Class { get { return this["class"]; } }

        public string Coords { get { return this["coords"]; } }

        public string Dir { get { return this["dir"]; } }

        public string Href { get { return this["href"]; } }

        public string Id { get { return this["id"]; } }

        public string Lang { get { return this["lang"]; } }

        public string Nohref { get { return this["nohref"]; } }

        public string Onblur { get { return this["onblur"]; } }

        public string Onclick { get { return this["onclick"]; } }

        public string Ondblclick { get { return this["ondblclick"]; } }

        public string Onfocus { get { return this["onfocus"]; } }

        public string Onkeydown { get { return this["onkeydown"]; } }

        public string Onkeypress { get { return this["onkeypress"]; } }

        public string Onkeyup { get { return this["onkeyup"]; } }

        public string Onmousedown { get { return this["onmousedown"]; } }

        public string Onmousemove { get { return this["onmousemove"]; } }

        public string Onmouseout { get { return this["onmouseout"]; } }

        public string Onmouseover { get { return this["onmouseover"]; } }

        public string Onmouseup { get { return this["onmouseup"]; } }

        public string Shape { get { return this["shape"]; } }

        public string Style { get { return this["style"]; } }

        public string Tabindex { get { return this["tabindex"]; } }

        public string Target { get { return this["target"]; } }

        public string Title { get { return this["title"]; } }

        public Area()
            : this(new Element[0])
        {
        }

        public Area(params Element[] children)
            : this(new TagAttribute[0], children)
        {
        }

        public Area(params TagAttribute[] attributes)
            : this(attributes, new Element[0])
        {
        }

        public Area(IEnumerable<TagAttribute> attributes, params Element[] children)
            : base(attributes, children)
        {
            TagName = "area";
        }
    }
}