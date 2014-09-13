using System;
using System.Collections.Generic;

namespace HtmlSharp.Elements.Tags
{
    public class OL : Tag, IAllowsNestingSelf
    {
        public IEnumerable<Type> NestingBreakers { get { return new Type[0]; } }

        public string Class { get { return this["class"]; } }

        public string Compact { get { return this["compact"]; } }

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

        public string Start { get { return this["start"]; } }

        public string Style { get { return this["style"]; } }

        public string Title { get { return this["title"]; } }

        public string Type { get { return this["type"]; } }

        public OL()
            : this(new Element[0])
        {
        }

        public OL(params Element[] children)
            : this(new TagAttribute[0], children)
        {
        }

        public OL(params TagAttribute[] attributes)
            : this(attributes, new Element[0])
        {
        }

        public OL(IEnumerable<TagAttribute> attributes, params Element[] children)
            : base(attributes, children)
        {
            ResetsNesting = true;
            TagName = "ol";
        }
    }
}