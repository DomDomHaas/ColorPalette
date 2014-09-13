using System;
using System.Collections.Generic;

namespace HtmlSharp.Elements.Tags
{
    public class Html : Tag
    {
        public string Dir { get { return this["dir"]; } }

        public string Lang { get { return this["lang"]; } }

        public string Version { get { return this["version"]; } }

        public Html()
            : this(new Element[0])
        {
        }

        public Html(params Element[] children)
            : this(new TagAttribute[0], children)
        {
        }

        public Html(params TagAttribute[] attributes)
            : this(attributes, new Element[0])
        {
        }

        public Html(IEnumerable<TagAttribute> attributes, params Element[] children)
            : base(attributes, children)
        {
            TagName = "html";
        }
    }
}