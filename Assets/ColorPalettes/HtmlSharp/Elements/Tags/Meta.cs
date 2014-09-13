using System;
using System.Collections.Generic;

namespace HtmlSharp.Elements.Tags
{
    public class Meta : Tag
    {
        public string Content { get { return this["content"]; } }

        public string Dir { get { return this["dir"]; } }

        public string Httpequiv { get { return this["http-equiv"]; } }

        public string Lang { get { return this["lang"]; } }

        public string Name { get { return this["name"]; } }

        public string Scheme { get { return this["scheme"]; } }

        public Meta()
            : this(new Element[0])
        {
        }

        public Meta(params Element[] children)
            : this(new TagAttribute[0], children)
        {
        }

        public Meta(params TagAttribute[] attributes)
            : this(attributes, new Element[0])
        {
        }

        public Meta(IEnumerable<TagAttribute> attributes, params Element[] children)
            : base(attributes, children)
        {
            IsSelfClosing = true;
            TagName = "meta";
        }
    }
}