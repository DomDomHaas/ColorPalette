using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Elements;

namespace HtmlSharp.Css
{
    public class NthLastChildFilter : IFilter
    {
        Expression expression;

        public NthLastChildFilter(Expression expression)
        {
            this.expression = expression;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                NthLastChildFilter t = (NthLastChildFilter)obj;
                return expression.Equals(t.expression);
            }
        }

        public override int GetHashCode()
        {
            return expression.GetHashCode();
        }

        public IEnumerable<Tag> Apply(IEnumerable<Tag> tags)
        {
            foreach (var tag in tags)
            {
                IList<Tag> childrenTags = tag.Children.OfType<Tag>().ToList();
                foreach (var index in expression.GetValues().TakeWhile(n => n > 0))
                {
                    if (index <= childrenTags.Count)
                    {
                        yield return childrenTags[childrenTags.Count - index];
                    }
                }
            }
        }
    }
}
