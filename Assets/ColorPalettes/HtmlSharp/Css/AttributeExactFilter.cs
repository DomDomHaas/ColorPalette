using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Elements;

namespace HtmlSharp.Css
{
    public class AttributeExactFilter : AttributeFilter
    {
        string text;

        public AttributeExactFilter(string type, string text)
            : base(type)
        {
            this.text = text;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                AttributeExactFilter t = (AttributeExactFilter)obj;
                return text.Equals(t.text) && base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ text.GetHashCode();
        }

        public override IEnumerable<Tag> Apply(IEnumerable<Tag> tags)
        {
            return tags.Where(tag => tag[type] == text);
        }
    }
}
