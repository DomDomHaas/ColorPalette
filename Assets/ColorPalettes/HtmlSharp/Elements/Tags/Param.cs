using System;
using System.Collections.Generic;

namespace HtmlSharp.Elements.Tags
{
    public class Param : Tag
    {
        public string Id { get { return this["id"]; } }

        public string Name { get { return this["name"]; } }

        public string Type { get { return this["type"]; } }

        public string Value { get { return this["value"]; } }

        public string Valuetype { get { return this["valuetype"]; } }

        public Param()
            : this(new Element[0])
        {
        }

        public Param(params Element[] children)
            : this(new TagAttribute[0], children)
        {
        }

        public Param(params TagAttribute[] attributes)
            : this(attributes, new Element[0])
        {
        }

        public Param(IEnumerable<TagAttribute> attributes, params Element[] children)
            : base(attributes, children)
        {
            TagName = "param";
        }
    }
}