using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Elements;

namespace HtmlSharp.Css
{
    public class AttributePrefixFilter : AttributeFilter
    {
        string prefix;

        public AttributePrefixFilter(string type, string prefix)
            : base(type)
        {
            this.prefix = prefix;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                AttributePrefixFilter t = (AttributePrefixFilter)obj;
                return prefix.Equals(t.prefix) && base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ prefix.GetHashCode();
        }

        public override IEnumerable<Tag> Apply(IEnumerable<Tag> tags)
        {
            return tags.Where(tag => tag[type] != null && tag[type].StartsWith(prefix));
        }
    }
}
