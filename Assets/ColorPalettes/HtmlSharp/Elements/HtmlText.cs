using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Extensions;

namespace HtmlSharp.Elements
{
    public class HtmlText : Element
    {
        string _value;
        public string Value
        {
            get { return _value.HtmlDecode(); }
            set { _value = value; }
        }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                HtmlText t = (HtmlText)obj;
                return t.Value.Equals(Value);
            }
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
