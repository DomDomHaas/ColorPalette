using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Elements;

namespace HtmlSharp.Css
{
    public class UniversalSelector : TypeSelector
    {
        public UniversalSelector()
            : base("*")
        {

        }

        public UniversalSelector(SelectorNamespacePrefix prefix)
            : base("*", prefix)
        {

        }

        public override string ToString()
        {
            return "*";
        }

        public override IEnumerable<Tag> Apply(IEnumerable<Tag> tags)
        {
            return tags;
        }
    }
}
