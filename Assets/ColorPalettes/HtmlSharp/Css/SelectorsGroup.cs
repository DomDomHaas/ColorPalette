using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Elements;

namespace HtmlSharp.Css
{
    public class SelectorsGroup
    {
        IEnumerable<Selector> selectors;

        public SelectorsGroup(IEnumerable<Selector> selectors)
        {
            this.selectors = selectors;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                SelectorsGroup t = (SelectorsGroup)obj;
                return selectors.SequenceEqual(t.selectors);
            }
        }

        public override int GetHashCode()
        {
            return selectors.Aggregate(0, (a, b) => a ^= b.GetHashCode());
        }

        public IEnumerable<Tag> Apply(IEnumerable<Tag> tags)
        {
            foreach (var selector in selectors)
            {
                foreach (var tag in selector.Apply(tags))
                {
                    yield return tag;
                }
            }
        }
    }
}
