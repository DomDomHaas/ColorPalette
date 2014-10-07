using System;
using System.Collections.Generic;

namespace HtmlSharp.Elements.Tags
{
    public class Base : Tag
    {
        public string Class { get { return this["class"]; } }

        public string Href { get { return this["href"]; } }

        public string Id { get { return this["id"]; } }

        public string Style { get { return this["style"]; } }

        public string Target { get { return this["target"]; } }

        public string Title { get { return this["title"]; } }

        public Base()
            : this(new Element[0])
        {
        }

        public Base(params Element[] children)
            : this(new TagAttribute[0], children)
        {
        }

        public Base(params TagAttribute[] attributes)
            : this(attributes, new Element[0])
        {
        }

        public Base(IEnumerable<TagAttribute> attributes, params Element[] children)
            : base(attributes, children)
        {
            IsSelfClosing = true;
            TagName = "base";
        }
    }
}