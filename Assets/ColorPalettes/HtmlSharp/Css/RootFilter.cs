using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Elements;
using HtmlSharp.Elements.Tags;

namespace HtmlSharp.Css
{
    public class RootFilter : IFilter
    {
        public RootFilter()
        {

        }

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
            return tags.Where(tag => tag.Parent is Root);
        }
    }
}
