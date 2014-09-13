using System;
using System.Collections.Generic;

namespace HtmlSharp.Elements.Tags
{
    public class Frameset : Tag
    {
        public string Class { get { return this["class"]; } }

        public string Cols { get { return this["cols"]; } }

        public string Id { get { return this["id"]; } }

        public string Onload { get { return this["onload"]; } }

        public string Onunload { get { return this["onunload"]; } }

        public string Rows { get { return this["rows"]; } }

        public string Style { get { return this["style"]; } }

        public string Title { get { return this["title"]; } }

        public Frameset()
            : this(new Element[0])
        {
        }

        public Frameset(params Element[] children)
            : this(new TagAttribute[0], children)
        {
        }

        public Frameset(params TagAttribute[] attributes)
            : this(attributes, new Element[0])
        {
        }

        public Frameset(IEnumerable<TagAttribute> attributes, params Element[] children)
            : base(attributes, children)
        {
            TagName = "frameset";
        }
    }
}