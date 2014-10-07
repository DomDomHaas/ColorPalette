using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Elements;

namespace HtmlSharp.Css
{
    public class NthOfTypeFilter : IFilter
    {
        Expression expression;

        public NthOfTypeFilter(Expression expression)
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
                NthOfTypeFilter t = (NthOfTypeFilter)obj;
                return expression.Equals(t.expression);
            }
        }

        public override int GetHashCode()
        {
            return expression.GetHashCode();
        }

        public virtual IEnumerable<Tag> Apply(IEnumerable<Tag> tags)
        {
            foreach (var tag in tags)
            {
                var start = tag.Parent.Children.OfType<Tag>().First(t => t.TagName == tag.TagName);

                List<Tag> siblingTags = new List<Tag>() { start };

                var sibling = start.NextSibling;
                while (sibling != null)
                {
                    Tag siblingTag = sibling as Tag;
                    if (siblingTag != null && siblingTag.TagName == tag.TagName)
                    {
                        siblingTags.Add(siblingTag);
                    }
                    sibling = sibling.NextSibling;
                }

                foreach (var index in expression.GetValues().TakeWhile(n => n <= siblingTags.Count))
                {
                    if (index > 0)
                    {
                        yield return siblingTags[index - 1];
                    }
                }
            }
        }
    }
}
