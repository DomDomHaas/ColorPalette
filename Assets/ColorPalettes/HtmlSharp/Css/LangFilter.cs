using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Elements;

namespace HtmlSharp.Css
{
    public class LangFilter : AttributeDashFilter
    {
        string lang;

        public LangFilter(string lang)
            : base("lang", lang)
        {
            this.lang = lang;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                LangFilter t = (LangFilter)obj;
                return lang.Equals(t.lang);
            }
        }

        public override int GetHashCode()
        {
            return lang.GetHashCode();
        }
    }
}
