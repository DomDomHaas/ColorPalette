using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Elements;

namespace HtmlSharp.Css
{
    public class NegationFilter : IFilter
    {
        IFilter filter;

        public NegationFilter(IFilter filter)
        {
            this.filter = filter;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                NegationFilter t = (NegationFilter)obj;
                return filter.Equals(t.filter);
            }
        }

        public override int GetHashCode()
        {
            return filter.GetHashCode();
        }

        public IEnumerable<Tag> Apply(IEnumerable<Tag> tags)
        {
            foreach (var tag in tags)
            {
                if (filter.Apply(new[] { tag }).Count() == 0)
                {
                    yield return tag;
                }
            }
        }
    }
}
