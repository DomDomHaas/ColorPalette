using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Elements;

namespace HtmlSharp.Css
{
    public class TypeSelector
    {
        public string Name { get; private set; }
        public SelectorNamespacePrefix Namespace { get; private set; }

        public TypeSelector(string name)
        {
            this.Name = name;
        }

        public TypeSelector(string name, SelectorNamespacePrefix prefix)
            : this(name)
        {
            this.Namespace = prefix;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                TypeSelector t = (TypeSelector)obj;
                return Name == t.Name && Namespace == t.Namespace;
            }
        }

        public override int GetHashCode()
        {
            return Namespace == null ? Name.GetHashCode() : Namespace.GetHashCode() ^ Name.GetHashCode();
        }

        public virtual IEnumerable<Tag> Apply(IEnumerable<Tag> tags)
        {
            foreach (var tag in tags.Where(tag => tag.TagName == Name))
            {
                yield return tag;
            }
        }
    }
}
