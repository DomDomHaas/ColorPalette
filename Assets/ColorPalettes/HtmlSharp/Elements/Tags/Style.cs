using System;
using System.Collections.Generic;

namespace HtmlSharp.Elements.Tags
{
    public class Style : Tag
    {
        public string Dir { get { return this["dir"]; } }

        public string Lang { get { return this["lang"]; } }

        public string Media { get { return this["media"]; } }

        public string Title { get { return this["title"]; } }

        public string Type { get { return this["type"]; } }

        public Style()
            : this(new Element[0])
        {
        }

        public Style(params Element[] children)
            : this(new TagAttribute[0], children)
        {
        }

        public Style(params TagAttribute[] attributes)
            : this(attributes, new Element[0])
        {
        }

        public Style(IEnumerable<TagAttribute> attributes, params Element[] children)
            : base(attributes, children)
        {
            TagName = "style";
        }
    }
}