using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlSharp.Elements
{
    public interface IAllowsNestingSelf
    {
        IEnumerable<Type> NestingBreakers { get; }
    }
}
