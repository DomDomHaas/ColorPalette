using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Elements;

namespace HtmlSharp.Css
{
    public class AttributeFilter : IFilter
    {
        protected string type;

        public AttributeFilter(string type)
        {
            this.type = type;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                AttributeFilter t = (AttributeFilter)obj;
                return type.Equals(t.type);
            }
        }

        public override int GetHashCode()
        {
            return type.GetHashCode();
        }

        public virtual IEnumerable<Tag> Apply(IEnumerable<Tag> tags)
        {
            return tags.Where(tag => tag[type] != null);
        }
    }
}
