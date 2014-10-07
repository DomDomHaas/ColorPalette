using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlSharp.Elements
{
    public class ProcessingInstruction : HtmlText
    {
        public override string ToString()
        {
            return "<?" + base.ToString() + "?>";
        }
    }
}
