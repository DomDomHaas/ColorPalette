using System;
using System.Collections.Generic;

namespace HtmlSharp.Elements.Tags
{
    public class Br : Tag
    {
        public string Class { get { return this["class"]; } }

        public string Clear { get { return this["clear"]; } }

        public string Id { get { return this["id"]; } }

        public string Style { get { return this["style"]; } }

        public string Title { get { return this["title"]; } }

        public Br()
            : this(new Element[0])
        {
        }

        public Br(params Element[] children)
            : this(new TagAttribute[0], children)
        {
        }

        public Br(params TagAttribute[] attributes)
            : this(attributes, new Element[0])
        {
        }

        public Br(IEnumerable<TagAttribute> attributes, params Element[] children)
            : base(attributes, children)
        {
            IsSelfClosing = true;
            TagName = "br";
        }
    }
}