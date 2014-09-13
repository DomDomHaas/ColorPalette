using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlSharp.Elements.Tags
{
    public class UnknownTag : Tag
    {
        public UnknownTag(string name)
            : this(name, new Element[0])
        {
        }

        public UnknownTag(string name, params Element[] children)
            : this(name, new TagAttribute[0], children)
        {
        }

        public UnknownTag(string name, params TagAttribute[] attributes)
            : this(name, attributes, new Element[0])
        {

        }

        public UnknownTag(string name, IEnumerable<TagAttribute> attributes, params Element[] children)
            : base(attributes, children)
        {
            TagName = name;
        }


    }
}
