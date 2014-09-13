using System;
using System.Collections.Generic;

namespace HtmlSharp.Elements.Tags
{
    public class Object : Tag, IAllowsNestingSelf
    {
        public IEnumerable<Type> NestingBreakers { get { return new Type[0]; } }

        public string Align { get { return this["align"]; } }

        public string Archive { get { return this["archive"]; } }

        public string Border { get { return this["border"]; } }

        public string Class { get { return this["class"]; } }

        public string Classid { get { return this["classid"]; } }

        public string Codebase { get { return this["codebase"]; } }

        public string Codetype { get { return this["codetype"]; } }

        public string Data { get { return this["data"]; } }

        public string Declare { get { return this["declare"]; } }

        public string Dir { get { return this["dir"]; } }

        public string Height { get { return this["height"]; } }

        public string Hspace { get { return this["hspace"]; } }

        public string Id { get { return this["id"]; } }

        public string Lang { get { return this["lang"]; } }

        public string Name { get { return this["name"]; } }

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

        public string Standby { get { return this["standby"]; } }

        public string Style { get { return this["style"]; } }

        public string Tabindex { get { return this["tabindex"]; } }

        public string Title { get { return this["title"]; } }

        public string Type { get { return this["type"]; } }

        public string Usemap { get { return this["usemap"]; } }

        public string Vspace { get { return this["vspace"]; } }

        public string Width { get { return this["width"]; } }

        public Object()
            : this(new Element[0])
        {
        }

        public Object(params Element[] children)
            : this(new TagAttribute[0], children)
        {
        }

        public Object(params TagAttribute[] attributes)
            : this(attributes, new Element[0])
        {
        }

        public Object(IEnumerable<TagAttribute> attributes, params Element[] children)
            : base(attributes, children)
        {
            TagName = "object";
        }
    }
}