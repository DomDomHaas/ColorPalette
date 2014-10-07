using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlSharp.Elements
{
    public class Comment : HtmlText
    {
        public override string ToString()
        {
            return "<!--" + base.ToString() + "-->";
        }
    }
}
