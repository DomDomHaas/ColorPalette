using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Extensions;
using System.Globalization;

namespace HtmlSharp
{
    public class TagAttribute
    {
        string _value;

        public string Name { get; private set; }       
        public string Value
        {
            get
            {
                return _value.HtmlDecode();
            }
            private set
            {
                _value = value;
            }
        }

        public TagAttribute(string name, string value)
        {
            Name = name.ToLowerInvariant();
            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                TagAttribute t = (TagAttribute)obj;
                return Name == t.Name && Value == t.Value;
            }
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Value.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}=\"{1}\"", Name, Value);
        }
    }
}
