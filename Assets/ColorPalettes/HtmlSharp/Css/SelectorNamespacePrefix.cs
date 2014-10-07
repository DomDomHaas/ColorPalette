using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlSharp.Css
{
    public class SelectorNamespacePrefix
    {
        public string Namespace { get; private set; }
        public SelectorNamespacePrefix(string ns)
        {
            this.Namespace = ns;
        }
    }
}
