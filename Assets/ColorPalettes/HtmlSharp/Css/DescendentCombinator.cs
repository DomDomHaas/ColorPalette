using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Elements;

namespace HtmlSharp.Css
{
    public class DescendantCombinator : Combinator
    {
        public override string ToString()
        {
            return " ";
        }

        public override IEnumerable<Tag> Apply(IEnumerable<Tag> tags)
        {
            foreach (var element in tags)
            {
                Tag child = element as Tag;
                if (child != null)
                {
                    foreach (var tag in ChildTags(child))
                    {
                        yield return tag;
                    }
                }
            }
        }

        private IEnumerable<Tag> ChildTags(Tag tag)
        {
            foreach (var element in tag.Children)
            {
                Tag child = element as Tag;
                if (child != null)
                {
                    yield return child;
                    foreach (var childTag in ChildTags(child))
                    {
                        yield return childTag;
                    }
                }
            }
        }
    }
}
