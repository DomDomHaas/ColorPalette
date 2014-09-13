using System;
using System.Collections.Generic;

namespace HtmlSharp.Elements.Tags
{
    public class Input : Tag
    {
        public string Accept { get { return this["accept"]; } }

        public string Accesskey { get { return this["accesskey"]; } }

        public string Align { get { return this["align"]; } }

        public string Alt { get { return this["alt"]; } }

        public string Checked { get { return this["checked"]; } }

        public string Class { get { return this["class"]; } }

        public string Dir { get { return this["dir"]; } }

        public string Disabled { get { return this["disabled"]; } }

        public string Id { get { return this["id"]; } }

        public string Ismap { get { return this["ismap"]; } }

        public string Lang { get { return this["lang"]; } }

        public string Maxlength { get { return this["maxlength"]; } }

        public string Name { get { return this["name"]; } }

        public string Onblur { get { return this["onblur"]; } }

        public string Onchange { get { return this["onchange"]; } }

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

        public string Onselect { get { return this["onselect"]; } }

        public string ReadOnly { get { return this["readonly"]; } }

        public string Size { get { return this["size"]; } }

        public string Src { get { return this["src"]; } }

        public string Style { get { return this["style"]; } }

        public string Tabindex { get { return this["tabindex"]; } }

        public string Title { get { return this["title"]; } }

        public string Type { get { return this["type"]; } }

        public string Usemap { get { return this["usemap"]; } }

        public string Value { get { return this["value"]; } }

        public Input()
            : this(new Element[0])
        {
        }

        public Input(params Element[] children)
            : this(new TagAttribute[0], children)
        {
        }

        public Input(params TagAttribute[] attributes)
            : this(attributes, new Element[0])
        {
        }

        public Input(IEnumerable<TagAttribute> attributes, params Element[] children)
            : base(attributes, children)
        {
            IsSelfClosing = true;
            TagName = "input";
        }
    }
}