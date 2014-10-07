using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Elements;

namespace HtmlSharp.Css
{
    public class OnlyOfTypeFilter : FirstOfTypeFilter
    {
        public override bool Equals(object obj)
        {
            return obj != null && GetType() == obj.GetType();
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }

        public override IEnumerable<Tag> Apply(IEnumerable<Tag> tags)
        {
            List<Tag> validTags = new List<Tag>();
            foreach (var tag in tags)
            {
                if (tag.Parent.Children.OfType<Tag>().Count(t => t.TagName == tag.TagName) == 1)
                {
                    validTags.Add(tag);
                }
            }
            return base.Apply(validTags);
        }
    }
}
