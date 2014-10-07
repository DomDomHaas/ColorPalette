using System;
using System.Collections.Generic;

namespace HtmlSharp.Elements.Tags
{
    public class Script : Tag
    {
        public string Charset { get { return this["charset"]; } }

        public string Defer { get { return this["defer"]; } }

        public string Language { get { return this["language"]; } }

        public string Src { get { return this["src"]; } }

        public string Type { get { return this["type"]; } }

        public Script()
            : this(new Element[0])
        {
        }

        public Script(params Element[] children)
            : this(new TagAttribute[0], children)
        {
        }

        public Script(params TagAttribute[] attributes)
            : this(attributes, new Element[0])
        {
        }

        public Script(IEnumerable<TagAttribute> attributes, params Element[] children)
            : base(attributes, children)
        {
            TagName = "script";
        }
    }
}