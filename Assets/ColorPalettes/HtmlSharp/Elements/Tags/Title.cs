using System;
using System.Collections.Generic;

namespace HtmlSharp.Elements.Tags
{
    public class Title : Tag
    {
        public string Dir { get { return this["dir"]; } }

        public string Lang { get { return this["lang"]; } }

        public Title()
            : this(new Element[0])
        {
        }

        public Title(params Element[] children)
            : this(new TagAttribute[0], children)
        {
        }

        public Title(params TagAttribute[] attributes)
            : this(attributes, new Element[0])
        {
        }

        public Title(IEnumerable<TagAttribute> attributes, params Element[] children)
            : base(attributes, children)
        {
            TagName = "title";
        }
    }
}