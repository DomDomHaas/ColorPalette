using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Elements;

namespace HtmlSharp.Css
{
    public class Selector
    {
        List<SimpleSelectorSequence> selectors = new List<SimpleSelectorSequence>();
        List<Combinator> combinators = new List<Combinator>();

        public Selector(SimpleSelectorSequence selector)
        {
            selectors.Add(selector);
        }

        public Selector(IEnumerable<SimpleSelectorSequence> selectors, IEnumerable<Combinator> combinators)
        {
            this.selectors.AddRange(selectors);
            this.combinators.AddRange(combinators);
        }

        public override string ToString()
        {
            StringBuilder selectorBuilder = new StringBuilder();
            for (int i = 0; i < selectors.Count; i++)
            {
                selectorBuilder.Append(selectors[i]);
                if (combinators.Count > i)
                {
                    selectorBuilder.Append(combinators[i]);
                }
            }
            return selectorBuilder.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                Selector t = (Selector)obj;
                return combinators.SequenceEqual(t.combinators) && selectors.SequenceEqual(t.selectors);
            }
        }

        public override int GetHashCode()
        {
            return combinators.Aggregate(0, (a, b) => a ^= b.GetHashCode()) & selectors.Aggregate(0, (a, b) => a ^= b.GetHashCode());
        }

        public IEnumerable<Tag> Apply(IEnumerable<Tag> tags)
        {
            for (int i = 0; i < selectors.Count; i++)
            {
                if (i < selectors.Count)
                {
                    tags = selectors[i].Apply(tags);
                }
                if (i < combinators.Count)
                {
                    tags = combinators[i].Apply(tags);
                }
            }
            return tags;
        }
    }
}
