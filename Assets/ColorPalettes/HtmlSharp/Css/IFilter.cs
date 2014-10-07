using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Elements;

namespace HtmlSharp.Css
{
    public interface IFilter
    {
        IEnumerable<Tag> Apply(IEnumerable<Tag> tags);
    }
}
