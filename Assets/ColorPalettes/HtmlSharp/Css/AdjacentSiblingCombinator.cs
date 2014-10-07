using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Elements;

namespace HtmlSharp.Css
{
    public class AdjacentSiblingCombinator : Combinator
    {
        public override string ToString()
        {
            return "+";
        }

        public override IEnumerable<Tag> Apply(IEnumerable<Tag> tags)
        {
            foreach (var tag in tags)
            {
                var sibling = tag.NextSibling;
                while (sibling != null)
                {
                    Tag siblingTag = sibling as Tag;
                    if (siblingTag != null)
                    {
                        yield return siblingTag;
                        break;
                    }
                    sibling = sibling.NextSibling;
                }
            }
        }
    }
}
