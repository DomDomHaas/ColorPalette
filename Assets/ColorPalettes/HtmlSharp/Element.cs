using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace HtmlSharp
{
    public abstract class Element
    {
        private List<Element> children = new List<Element>();

        public Element Parent { get; set; }
        public Element Previous { get; set; }
        public Element Next { get; set; }
        public Element NextSibling { get; set; }
        public Element PreviousSibling { get; set; }

        public ReadOnlyCollection<Element> Children { get { return children.AsReadOnly(); } }
        
        protected Element()
        {
        }

        public void Setup(Element parent, Element previous)
        {
            Parent = parent;
            Previous = previous;
            if (Parent != null && Parent.children.Count > 0)
            {
                PreviousSibling = Parent.children[Parent.children.Count - 1];
                PreviousSibling.NextSibling = this;
            }
        }

        internal void AddChild(Element element)
        {
            element.Parent = this;
            Element previous = children.ElementAtOrDefault(children.Count - 1);
            if (previous != null)
            {
                element.PreviousSibling = previous;
                previous.NextSibling = element;
            }
            children.Add(element);
        }
    }
}
