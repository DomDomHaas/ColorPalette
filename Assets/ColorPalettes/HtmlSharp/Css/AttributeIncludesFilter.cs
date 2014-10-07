using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Elements;

namespace HtmlSharp.Css
{
    public class AttributeIncludesFilter : AttributeFilter
    {
        string includes;

        public AttributeIncludesFilter(string type, string includes)
            : base(type)
        {
            this.includes = includes;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                AttributeIncludesFilter t = (AttributeIncludesFilter)obj;
                return includes.Equals(t.includes) && base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ includes.GetHashCode();
        }

        public override IEnumerable<Tag> Apply(IEnumerable<Tag> tags)
        {
            return tags.Where(tag => tag[type] != null && tag[type].Split(' ').Any(value => value == includes));
        }
    }
}
