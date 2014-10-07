using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Elements;

namespace HtmlSharp.Css
{
    public class AttributeSubstringFilter : AttributeFilter
    {
        string substring;

        public AttributeSubstringFilter(string type, string substring)
            : base(type)
        {
            this.substring = substring;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                AttributeSubstringFilter t = (AttributeSubstringFilter)obj;
                return substring.Equals(t.substring) && base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ substring.GetHashCode();
        }

        public override IEnumerable<Tag> Apply(IEnumerable<Tag> tags)
        {
            return tags.Where(tag => tag[type] != null && tag[type].Contains(substring));
        }
    }
}
