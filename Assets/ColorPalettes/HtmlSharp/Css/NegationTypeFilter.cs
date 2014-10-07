using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Elements;

namespace HtmlSharp.Css
{
    public class NegationTypeFilter : IFilter
    {
        TypeSelector selector;

        public NegationTypeFilter(TypeSelector selector)
        {
            this.selector = selector;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                NegationTypeFilter t = (NegationTypeFilter)obj;
                return selector.Equals(t.selector);
            }
        }

        public override int GetHashCode()
        {
            return selector.GetHashCode();
        }

        public IEnumerable<Tag> Apply(IEnumerable<Tag> tags)
        {
            foreach (var tag in tags)
            {
                if (selector.Apply(new[] { tag }).Count() == 0)
                {
                    yield return tag;
                }
            }
        }
    }
}
