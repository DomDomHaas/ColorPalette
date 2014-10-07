using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlSharp.Css
{
    public abstract class Expression
    {
        public abstract IEnumerable<int> GetValues();
    }
}
