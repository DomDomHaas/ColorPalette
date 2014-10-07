using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Elements;

namespace HtmlSharp.Css
{
    public class NthLastOfTypeFilter : IFilter
    {
        Expression expression;

        public NthLastOfTypeFilter(Expression expression)
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
                NthLastOfTypeFilter t = (NthLastOfTypeFilter)obj;
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
                List<Tag> siblingTags = new List<Tag>() { tag };
                var sibling = tag.NextSibling;
                while (sibling != null)
                {
                    Tag siblingTag = sibling as Tag;
                    if (siblingTag != null && siblingTag.TagName == tag.TagName)
                    {
                        siblingTags.Add(siblingTag);
                    }
                    sibling = sibling.NextSibling;
                }

                foreach (var index in expression.GetValues().TakeWhile(n => n > 0))
                {
                    if (index <= siblingTags.Count)
                    {
                        yield return siblingTags[siblingTags.Count - index];
                    }
                }
            }
        }
    }
}
