using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Elements;

namespace HtmlSharp.Css
{
    public class SimpleSelectorSequence
    {
        IEnumerable<IFilter> filters;
        TypeSelector selector;

        public SimpleSelectorSequence(TypeSelector selector, IEnumerable<IFilter> filters)
        {
            this.selector = selector;
            this.filters = filters;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                SimpleSelectorSequence t = (SimpleSelectorSequence)obj;
                return selector.Equals(t.selector) && filters.SequenceEqual(t.filters);
            }
        }

        public override int GetHashCode()
        {
            return selector.GetHashCode() ^ filters.Aggregate(0, (a, b) => a ^= b.GetHashCode());
        }

        public IEnumerable<Tag> Apply(IEnumerable<Tag> tags)
        {
            tags = selector.Apply(tags);
            return filters.Aggregate(tags, (t, filter) =>  filter.Apply(t));
        }
    }
}
