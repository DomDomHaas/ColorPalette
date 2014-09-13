using System;
using System.Collections.Generic;

namespace HtmlSharp.Elements.Tags
{
    public class IFrame : Tag
    {
        public string Align { get { return this["align"]; } }

        public string Class { get { return this["class"]; } }

        public string Frameborder { get { return this["frameborder"]; } }

        public string Height { get { return this["height"]; } }

        public string Id { get { return this["id"]; } }

        public string Longdesc { get { return this["longdesc"]; } }

        public string Marginheight { get { return this["marginheight"]; } }

        public string Marginwidth { get { return this["marginwidth"]; } }

        public string Name { get { return this["name"]; } }

        public string Scrolling { get { return this["scrolling"]; } }

        public string Src { get { return this["src"]; } }

        public string Style { get { return this["style"]; } }

        public string Title { get { return this["title"]; } }

        public string Width { get { return this["width"]; } }

        public IFrame()
            : this(new Element[0])
        {
        }

        public IFrame(params Element[] children)
            : this(new TagAttribute[0], children)
        {
        }

        public IFrame(params TagAttribute[] attributes)
            : this(attributes, new Element[0])
        {
        }

        public IFrame(IEnumerable<TagAttribute> attributes, params Element[] children)
            : base(attributes, children)
        {
            TagName = "iframe";
        }
    }
}