using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlSharp.Elements.Tags;
using System.Collections.ObjectModel;
using HtmlSharp.Css;

namespace HtmlSharp.Elements
{
    public abstract class Tag : Element
    {
        Dictionary<string, TagAttribute> attributes = new Dictionary<string, TagAttribute>();
        public bool Hidden { get; protected set; }
        public IEnumerable<TagAttribute> Attributes { get { return attributes.Values; } }
        public string TagName { get; protected set; }
        public bool IsSelfClosing { get; protected set; }
        public bool ResetsNesting { get; protected set; }

        static Dictionary<string, Func<IEnumerable<TagAttribute>, Tag>> tagMap =
            new Dictionary<string, Func<IEnumerable<TagAttribute>, Tag>>()
            {
                {"a", attributes => new A(attributes)},
                {"abbr", attributes => new Abbr(attributes)},
                {"acronym", attributes => new Acronym(attributes)},
                {"address", attributes => new Address(attributes)},
                {"applet", attributes => new Applet(attributes)},
                {"area", attributes => new Area(attributes)},
                {"b", attributes => new B(attributes)},
                {"base", attributes => new Base(attributes)},
                {"basefont", attributes => new BaseFont(attributes)},
                {"bdo", attributes => new Bdo(attributes)},
                {"big", attributes => new Big(attributes)},
                {"blockquote", attributes => new BlockQuote(attributes)},
                {"body", attributes => new Body(attributes)},
                {"br", attributes => new Br(attributes)},
                {"button", attributes => new Button(attributes)},
                {"caption", attributes => new Caption(attributes)},
                {"center", attributes => new Center(attributes)},
                {"cite", attributes => new Cite(attributes)},
                {"code", attributes => new Code(attributes)},
                {"col", attributes => new Col(attributes)},
                {"colgroup", attributes => new ColGroup(attributes)},
                {"dd", attributes => new DD(attributes)},
                {"del", attributes => new Del(attributes)},
                {"dfn", attributes => new Dfn(attributes)},
                {"dir", attributes => new Dir(attributes)},
                {"div", attributes => new Div(attributes)},
                {"dl", attributes => new DL(attributes)},
                {"dt", attributes => new DT(attributes)},
                {"em", attributes => new Em(attributes)},
                {"fieldset", attributes => new FieldSet(attributes)},
                {"font", attributes => new Font(attributes)},
                {"form", attributes => new Form(attributes)},
                {"frame", attributes => new Frame(attributes)},
                {"frameset", attributes => new Frameset(attributes)},
                {"h1", attributes => new H1(attributes)},
                {"h2", attributes => new H2(attributes)},
                {"h3", attributes => new H3(attributes)},
                {"h4", attributes => new H4(attributes)},
                {"h5", attributes => new H5(attributes)},
                {"h6", attributes => new H6(attributes)},
                {"head", attributes => new Head(attributes)},
                {"hr", attributes => new HR(attributes)},
                {"html", attributes => new Html(attributes)},
                {"i", attributes => new I(attributes)},
                {"iframe", attributes => new IFrame(attributes)},
                {"img", attributes => new Img(attributes)},
                {"input", attributes => new Input(attributes)},
                {"ins", attributes => new Ins(attributes)},
                {"isindex", attributes => new IsIndex(attributes)},
                {"kbd", attributes => new Kbd(attributes)},
                {"label", attributes => new Label(attributes)},
                {"legend", attributes => new Legend(attributes)},
                {"li", attributes => new LI(attributes)},
                {"link", attributes => new Link(attributes)},
                {"map", attributes => new Map(attributes)},
                {"menu", attributes => new Menu(attributes)},
                {"meta", attributes => new Meta(attributes)},
                {"noframes", attributes => new NoFrames(attributes)},
                {"noscript", attributes => new NoScript(attributes)},
                {"object", attributes => new Tags.Object(attributes)},
                {"ol", attributes => new OL(attributes)},
                {"optgroup", attributes => new OptGroup(attributes)},
                {"option", attributes => new Option(attributes)},
                {"p", attributes => new P(attributes)},
                {"param", attributes => new Param(attributes)},
                {"pre", attributes => new Pre(attributes)},
                {"q", attributes => new Q(attributes)},
                {"s", attributes => new S(attributes)},
                {"samp", attributes => new Samp(attributes)},
                {"script", attributes => new Script(attributes)},
                {"select", attributes => new Select(attributes)},
                {"small", attributes => new Small(attributes)},
                {"span", attributes => new Span(attributes)},
                {"strike", attributes => new Strike(attributes)},
                {"strong", attributes => new Strong(attributes)},
                {"style", attributes => new Style(attributes)},
                {"sub", attributes => new Sub(attributes)},
                {"sup", attributes => new Sup(attributes)},
                {"table", attributes => new Table(attributes)},
                {"tbody", attributes => new TBody(attributes)},
                {"td", attributes => new TD(attributes)},
                {"textarea", attributes => new TextArea(attributes)},
                {"tfoot", attributes => new TFoot(attributes)},
                {"th", attributes => new TH(attributes)},
                {"thead", attributes => new THead(attributes)},
                {"title", attributes => new Title(attributes)},
                {"tr", attributes => new TR(attributes)},
                {"tt", attributes => new TT(attributes)},
                {"u", attributes => new U(attributes)},
                {"ul", attributes => new UL(attributes)},
                {"var", attributes => new Var(attributes)},
                {"[document]", attributes => new Root(attributes)}
            };

        public static Tag Create(string name)
        {
            return Create(name, new TagAttribute[0]);
        }

        public static Tag Create(string name, IEnumerable<TagAttribute> attributes)
        {
            if (tagMap.ContainsKey(name))
            {
                return tagMap[name](attributes);
            }
            return new UnknownTag(name, attributes);
        }

        protected Tag()
        {
            attributes = new Dictionary<string, TagAttribute>();
        }

        protected Tag(params Element[] children)
            : this(new TagAttribute[0], children)
        {
        }

        protected Tag(params TagAttribute[] attributes)
            : this(attributes, new Element[0])
        {
        }

        protected Tag(IEnumerable<TagAttribute> attributes, params Element[] children)
            : this()
        {
            foreach (var child in children)
            {
                AddChild(child);
            }
            foreach (var attribute in attributes)
            {
                this.attributes.Add(attribute.Name, attribute);
            }
        }


        public string this[string attribute]
        {
            get
            {
                if (attributes.ContainsKey(attribute))
                {
                    return attributes[attribute].Value;
                }
                else
                {
                    return null;
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                Tag t = (Tag)obj;
                if (TagName != t.TagName)
                {
                    return false;
                }
                return Attributes.SequenceEqual(t.Attributes) && Children.SequenceEqual(t.Children);
            }
        }

        public override int GetHashCode()
        {
            return TagName.GetHashCode() ^ Attributes.Aggregate(0, (a, b) => a ^= b.GetHashCode());
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("<");
            builder.Append(TagName);
            foreach (TagAttribute attribute in Attributes)
            {
                builder.Append(" ").Append(attribute);
            }
            builder.Append(">");
            foreach (Element element in Children)
            {
                builder.Append(element);
            }
            builder.AppendFormat("</{0}>", TagName);
            return builder.ToString();
        }

        public IEnumerable<Tag> FindAll(string selector)
        {
            SelectorParser parser = new SelectorParser();
            var selectorGroup = parser.Parse(selector);
            return selectorGroup.Apply(GetTags());
        }

        public Tag Find(string selector)
        {
            return FindAll(selector).FirstOrDefault();
        }

        public T Find<T>(string selector)
        {
            return FindAll<T>(selector).FirstOrDefault();
        }

        public IEnumerable<T> FindAll<T>(string selector)
        {
            return FindAll(selector).OfType<T>();
        }

        IEnumerable<Tag> GetTags()
        {
            Element currentTag = Children.ElementAtOrDefault(0);
            while (currentTag != null)
            {
                Tag current = currentTag as Tag;
                if (current != null)
                {
                    yield return current;
                }
                currentTag = currentTag.Next;
            }
        }
    }
}
