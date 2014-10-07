using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Elements;

namespace HtmlSharp.Css
{
    public class ClassFilter : IFilter
    {
        string klass;

        public ClassFilter(string klass)
        {
            this.klass = klass;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                ClassFilter t = (ClassFilter)obj;
                return klass.Equals(t.klass);
            }
        }

        public override int GetHashCode()
        {
            return klass.GetHashCode();
        }

        public IEnumerable<Tag> Apply(IEnumerable<Tag> tags)
        {
            return tags.Where(tag => tag["class"] == klass);
        }
    }
}
