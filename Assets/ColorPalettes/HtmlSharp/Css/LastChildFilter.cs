using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Elements;

namespace HtmlSharp.Css
{
    public class LastChildFilter : IFilter
    {
        public override bool Equals(object obj)
        {
            return obj != null && GetType() == obj.GetType();
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }

        public IEnumerable<Tag> Apply(IEnumerable<Tag> tags)
        {
            foreach (var tag in tags)
            {
                var childrenTags = tag.Children.OfType<Tag>().ToList();
                if (childrenTags.Count > 0)
                {
                    yield return childrenTags.Last();
                }
            }
        }
    }
}
