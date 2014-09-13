using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlSharp.Elements;
using HtmlSharp.Extensions;
using System.Globalization;

namespace HtmlSharp
{
    public class HtmlParser
    {
        //parser 
        Regex declnameMatch = new Regex(@"[a-zA-Z][-_.a-zA-Z0-9]*\s*");
        Regex declareStringLit = new Regex(@"('[^']*\'|""[^""]*"")\s*");
        Regex markedSectionClose = new Regex(@"]\s*]\s*>");
        Regex msMarkedSectionClose = new Regex(@"]\s*>");

        //html parser
        Regex interestingNormal = new Regex("[<]");
        Regex interestingCData = new Regex(@"<(/|\Z)");

        Regex startTagOpen = new Regex("<[a-zA-Z]");
        Regex processingInstructionClose = new Regex(">");
        Regex commentClose = new Regex(@"--\s*>");
        Regex tagFind = new Regex("[a-zA-Z][-.a-zA-Z0-9:_]*");
        Regex attributeFind = new Regex(@"\s*([a-zA-Z_][-.:a-zA-Z_0-9]*)(\s*=\s*('[^']*'|""[^""]*""|" +
            @"[-a-zA-Z0-9./,:;+*%?!&$\(\)_#=~@]*))?");

        Regex locateStartTagEnd = new Regex(@"<[a-zA-Z][-.a-zA-Z0-9:_]*(?:\s+(?:[a-zA-Z_][-.:a-zA-Z0-9_]*(?:\s*=\s*(?:'[^']*'|\""[^\""]*\""|[^'\"">\s]+))?))*\s*");
        Regex endEndTag = new Regex(">");
        Regex endTagFind = new Regex(@"</\s*([a-zA-Z][-.a-zA-Z0-9:_]*)\s*>");

        Regex interesting;

        char[] otherChars = { };
        string[] preserveWhitespaceTags = { "pre", "textarea" };
        string html = string.Empty;
        string lastTag;

        int lineNumber = 1;
        int offset;
        Tag currentTag;
        Tag root;
        List<string> currentData = new List<string>();

        Stack<Tag> tagStack = new Stack<Tag>();

        string[] CDataContentElements = { "script", "style" };
        Dictionary<string, string> quoteTags = new Dictionary<string, string>()
        { 
            { "script",   null }, 
            { "textarea", null }
        };

        Stack<string> quoteStack = new Stack<string>();

        public HtmlParser()
        {
            interesting = interestingNormal;
            root = Tag.Create("[document]");
            PushTag(root);
        }

        public Document Parse(string html)
        {
            Feed(html);
            return new Document(html, root);
        }

        //Feeds data into the parser
        void Feed(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                //markup massage
            }
            html += data;
            GoAhead();
            EndData();
            while (currentTag != root)
            {
                PopTag();
            }
        }

        void SetCDataMode(bool value)
        {
            if (value)
            {
                interesting = interestingCData;
            }
            else
            {
                interesting = interestingNormal;
            }
        }

        void GoAhead()
        {
            int i = 0;
            int j;
            int k;
            Match match;
            while (i < html.Length)
            {
                match = interesting.Match(html, i);
                if (match.Success)
                {
                    j = match.Index;
                }
                else
                {
                    j = html.Length;
                }
                if (i < j)
                {
                    HandleData(html.Substring(i, j - i));
                }
                i = UpdatePosition(i, j);
                if (i == html.Length)
                {
                    break;
                }
                if (html[i] == '<')
                {
                    if (startTagOpen.MatchAtIndex(html, i).Success)
                    {
                        k = ParseStartTag(i);
                    }
                    else if (html.Substring(i, 2) == "</")
                    {
                        k = ParseEndTag(i);
                    }
                    else if (html.Substring(i, 4) == "<!--")
                    {
                        //# Strictly speaking, a comment is --.*--
                        //# within a declaration tag <!...>.
                        //# This should be removed,
                        //# and comments handled only in parse_declaration.
                        k = ParseComment(i);
                    }
                    else if (html.Substring(i, 2) == "<?")
                    {
                        k = ParseProcessingInstruction(i);
                    }
                    else if (html.Substring(i, 2) == "<!")
                    {
                        k = ParseDeclaration(i);
                    }
                    else if (i + 1 < html.Length)
                    {
                        HandleData("<");
                        k = i + 1;
                    }
                    else
                    {
                        break;
                    }
                    if (k < 0)
                    {
                        break;
                    }
                    i = UpdatePosition(i, k);
                }
            }
            html = html.Substring(i);
        }

        int RealParseDeclaration(int i)
        {
            //# This is some sort of declaration; in "HTML as
            //# deployed," this should only be the document type
            //# declaration ("<!DOCTYPE html...>").
            //# ISO 8879:1986, however, has more complex
            //# declaration syntax for elements in <!...>, including:
            //# --comment--
            //# [marked section]
            //# name in the following list: ENTITY, DOCTYPE, ELEMENT,
            //# ATTLIST, NOTATION, SHORTREF, USEMAP,
            //# LINKTYPE, LINK, IDLINK, USELINK, SYSTEM
            int j = i + 2;
            string decltype;

            if (j >= html.Length || html[j] == '-')
            {
                //start of comment followed by buffer boundary or just a buffer boundary
                return -1;
            }
            if (html[j] == '>')
            {
                //empty comment <!>
                return j + 1;
            }

            //a simple, practical version could look like ((name|stringlit) S*)  + '>'

            if (html.Substring(j, 2) == "--")
            {
                return ParseComment(i);
            }
            else if (html[j] == '[') //marked section
            {
                //# Locate [statusWord [...arbitrary SGML...]] as the body of the marked section
                //# Where statusWord is one of TEMP, CDATA, IGNORE, INCLUDE, RCDATA
                //# Note that this is extended by Microsoft Office "Save as Web" function
                //# to include [if...] and [endif].
                return ParseMarkedSection(i);
            }
            else
            {
                KeyValuePair<string, int> both = ScanName(j, i);
                decltype = both.Key;
                j = both.Value;
            }
            if (j < 0)
            {
                return j;
            }
            if (decltype == "doctype")
            {
                otherChars = new char[0];
            }
            while (j < html.Length)
            {
                char c = html[j];
                if (c == '>')
                {
                    string data = html.Substring(i + 2, j - (i + 2));
                    if (decltype == "doctype")
                    {
                        HandleDeclaration(data);
                    }
                    return j + 1;
                }
                if (c == '"' || c == '\'')
                {
                    Match m = declareStringLit.MatchAtIndex(html, j);
                    if (!m.Success)
                    {
                        return -1;
                    }
                    j = j + m.Length;
                }
                else if (Char.IsLetter(c))
                {
                    KeyValuePair<string, int> both = ScanName(j, i);
                    j = both.Value;
                }
                else if (otherChars.Contains(c))
                {
                    j += 1;
                }
                else if (c == '[')
                {
                    if (decltype == "doctype")
                    {
                        j = ParseDoctypeSubset(j + 1, i);
                    }
                    else if (new[] { "attlist", "linktype", "link", "element" }.Contains(decltype))
                    {
                        //# must tolerate []'d groups in a content model in an element declaration
                        //# also in data attribute specifications of attlist declaration
                        //# also link type declaration subsets in linktype declarations
                        //# also link attribute specification lists in link declarations
                        throw new HtmlParseException("Unsupported [ char in declaration: " + decltype);
                    }
                    else
                    {
                        throw new HtmlParseException("Unexpected [ char in declaration");
                    }
                }
                else
                {
                    throw new HtmlParseException("Unexpected  char in declaration: " + html[j]);
                }
                if (j < 0)
                {
                    return j;
                }
            }
            return -1; // incomplete
        }

        int ParseDeclaration(int i)
        {
            int j;
            if (html.Substring(i, i + 9) == "<![CDATA[")
            {
                int k = html.IndexOf("]]>", i, StringComparison.Ordinal);
                if (k == -1)
                {
                    k = html.Length;
                }
                string data = html.Substring(i + 9, k - (i + 9));
                j = k + 3;
                ToStringSubClass(data, new CharacterData());
            }
            else
            {
                try
                {
                    j = RealParseDeclaration(i);
                }
                catch //HtmlParseException
                {
                    string toHandle = html.Substring(i);
                    HandleData(toHandle);
                    j = i + toHandle.Length;
                }
            }
            return j;
        }

        int ParseDoctypeSubset(int i, int declstartpos)
        {
            int j = i;
            while (j < html.Length)
            {
                char c = html[j];
                if (c == '<')
                {
                    if (j + 2 >= html.Length)
                    {
                        //end of buffer; incomplete
                        return -1;
                    }
                    string s = html.Substring(j, 2);
                    if (s != "<!")
                    {
                        UpdatePosition(declstartpos, j + 1);
                        throw new HtmlParseException("Unexpected char in internal subset: " + s);
                    }
                    if (j + 2 == html.Length)
                    {
                        // end of buffer; incomplete
                        return -1;
                    }
                    if (j + 4 > html.Length)
                    {
                        //end of buffer; incomplete
                        return -1;
                    }
                    if (html.Substring(j, 4) == "<!--")
                    {
                        j = ParseComment(j, false);
                        if (j < 0)
                        {
                            return j;
                        }
                        continue;
                    }
                    KeyValuePair<string, int> both = ScanName(j + 2, declstartpos);
                    string name = both.Key;
                    j = both.Value;
                    if (name == "element")
                    {
                        j = ParseDoctypeElement(j, declstartpos);
                    }
                    else if (name == "entity")
                    {
                        j = ParseDoctypeEntity(j, declstartpos);
                    }
                    else if (name == "notation")
                    {
                        j = ParseDoctypeNotation(j, declstartpos);
                    }
                    else if (name == "attlist")
                    {
                        j = ParseDoctypeAttlist(j, declstartpos);
                    }
                    else
                    {
                        UpdatePosition(declstartpos, j + 2);
                        throw new HtmlParseException("Unknown declaration in internal subset: " + name);
                    }
                    if (j < 0)
                    {
                        return j;
                    }
                }
                else if (c == '%')
                {
                    // parameter entity reference
                    if (j + 1 == html.Length)
                    {
                        //end of buffer; incomplete
                        return -1;
                    }
                    KeyValuePair<string, int> both = ScanName(j + 1, declstartpos);
                    string s = both.Key;
                    j = both.Value;
                    if (j < 0)
                    {
                        return j;
                    }
                    if (html[j] == ';')
                    {
                        j += 1;
                    }
                }
                else if (c == ']')
                {
                    j += 1;
                    while (j < html.Length && char.IsWhiteSpace(html[j]))
                    {
                        j += 1;
                    }
                    if (j < html.Length)
                    {
                        if (html[j] == '>')
                        {
                            return j;
                        }
                        UpdatePosition(declstartpos, j);
                        throw new HtmlParseException("Unexpected char after internal subset");
                    }
                    else
                    {
                        return -1;
                    }
                }
                else if (char.IsWhiteSpace(c))
                {
                    j += 1;
                }
                else
                {
                    UpdatePosition(declstartpos, j);
                    throw new HtmlParseException("Unexpected char in internal subset: " + c);
                }
            }

            return -1;
        }

        int ParseDoctypeAttlist(int i, int declstartpos)
        {
            var both = ScanName(i, declstartpos);
            int j = both.Value;
            if (j >= html.Length)
            {
                return -1;
            }
            char c = html[j];
            if (c == '>')
            {
                return j + 1;
            }
            while (true)
            {
                // scan a series of attribute descriptions; simplified:
                // name type [value] [#constraint]
                both = ScanName(j, declstartpos);
                j = both.Value;
                if (j < 0)
                {
                    return j;
                }
                if (j >= html.Length)
                {
                    return -1;
                }
                c = html[j];
                if (c == '(')
                {
                    if (html.Substring(j).Contains(")"))
                    {
                        j = html.IndexOf(")", j, StringComparison.Ordinal) + 1;
                    }
                    else
                    {
                        return -1;
                    }
                    while (char.IsWhiteSpace(html, j))
                    {
                        j += 1;
                    }
                    if (j >= html.Length)
                    {
                        return -1;
                    }
                }
                else
                {
                    both = ScanName(j, declstartpos);
                    j = both.Value;
                }

                if (j >= html.Length)
                {
                    return -1;
                }
                c = html[j];
                if (c == '"' || c == '\'')
                {
                    Match m = declareStringLit.MatchAtIndex(html, j);
                    if (m.Success)
                    {
                        j = j + m.Length;
                    }
                    else
                    {
                        return -1;
                    }
                    c = html[j];
                    if (j >= html.Length)
                    {
                        return -1;
                    }
                }
                if (c == '#')
                {
                    if (html.Substring(j) == "#")
                    {
                        // end of buffer
                        return -1;
                    }
                    both = ScanName(j + 1, declstartpos);
                    j = both.Value;
                    if (j < 0)
                    {
                        return -1;
                    }
                    if (j >= html.Length)
                    {
                        return -1;
                    }
                }
                if (c == '>')
                {
                    return j + 1;
                }
            }
        }

        int ParseDoctypeNotation(int i, int declstartpos)
        {
            var both = ScanName(i, declstartpos);
            int j = both.Value;
            char c;
            if (j < 0)
            {
                return -1;
            }
            while (true)
            {
                if (j >= html.Length)
                {
                    return -1;
                }
                c = html[j];
                if (c == '>')
                {
                    return j + 1;
                }
                if (c == '\'' || c == '"')
                {
                    Match m = declareStringLit.MatchAtIndex(html, j);
                    if (!m.Success)
                    {
                        return -1;
                    }
                    j = j + m.Length;
                }
                else
                {
                    both = ScanName(j, declstartpos);
                    j = both.Value;
                    if (j < 0)
                    {
                        return j;
                    }
                }
            }
        }

        int ParseDoctypeEntity(int i, int declstartpos)
        {
            char c;
            int j;
            if (html[i] == '%')
            {
                j = i + 1;
                while (true)
                {
                    if (j >= html.Length)
                    {
                        return -1;
                    }
                    if (char.IsWhiteSpace(html, j))
                    {
                        j += 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                j = i;
            }

            var both = ScanName(j, declstartpos);
            j = both.Value;
            if (j < 0)
            {
                return -1;
            }
            while (true)
            {
                if (j >= html.Length)
                {
                    return -1;
                }
                c = html[j];
                if (c == '\'' || c == '"')
                {
                    Match m = declareStringLit.MatchAtIndex(html, j);
                    if (m.Success)
                    {
                        j = j + m.Length;
                    }
                    else
                    {
                        return -1; // incomplete
                    }
                }
                else if (c == '>')
                {
                    return j + 1;
                }
                else
                {
                    both = ScanName(j, declstartpos);
                    j = both.Value;
                    if (j < 0)
                    {
                        return j;
                    }
                }
            }
        }

        // interneral -- scan past <!ELEMENT declarations
        int ParseDoctypeElement(int i, int declstartpos)
        {
            var both = ScanName(i, declstartpos);
            int j = both.Value;
            if (j < 0)
            {
                return -1;
            }
            // style content model: just skip until '>'
            if (html.Substring(j).Contains('>'))
            {
                return html.IndexOf(">", j, StringComparison.Ordinal) + 1;
            }
            return -1;
        }

        void HandleDeclaration(string text)
        {
            ToStringSubClass(text, new Declaration());
        }

        KeyValuePair<string, int> ScanName(int i, int declstartpos)
        {
            if (i == html.Length)
            {
                return new KeyValuePair<string, int>(null, -1);
            }
            Match m = declnameMatch.MatchAtIndex(html, i);
            if (m.Success)
            {
                string s = m.Groups[0].Value;
                string name = s.Trim();
                if (i + s.Length == html.Length)
                {
                    return new KeyValuePair<string, int>(null, -1);
                }
                return new KeyValuePair<string, int>(name.ToLowerInvariant(), i + m.Length);
            }
            else
            {
                UpdatePosition(declstartpos, i);
                throw new HtmlParseException("Expected name token.");
            }
        }

        int UpdatePosition(int i, int j)
        {
            if (i >= j)
            {
                return j;
            }
            int nlines = html.Substring(i, j - i).Count(c => c == '\n');
            if (nlines > 0)
            {
                lineNumber += nlines;
                int pos = html.LastIndexOf('\n', j - 1, j - i);
                offset = j - (pos + 1);
            }
            else
            {
                offset += j - i;
            }
            return j;
        }

        int ParseMarkedSection(int i)
        {
            var both = ScanName(i + 3, i);
            string name = both.Key;
            int j = both.Value;
            if (j < 0)
            {
                return -1;
            }
            Match match;
            if (new[] { "temp", "cdata", "ignore", "unclude", "rcdata" }.Contains(name))
            {
                match = markedSectionClose.Match(html, i + 3);
            }
            else if (new[] { "if", "else", "endif" }.Contains(name))
            {
                match = msMarkedSectionClose.Match(html, i + 3);
            }
            else
            {
                throw new HtmlParseException("Unknown status keyword in marked section: " + html.Substring(i + 3, j - (i + 3)));
            }
            if (!match.Success)
            {
                return -1;
            }
            j = match.Index;
            return match.Index + match.Length;
        }

        int ParseProcessingInstruction(int i)
        {
            Match match = processingInstructionClose.Match(html, i + 2);
            if (!match.Success)
            {
                return -1;
            }
            int j = match.Index;
            HandleProcessingInstruction(html.Substring(i + 2, j - (i + 2)));
            j = match.Index + match.Length;
            return j;
        }

        void HandleProcessingInstruction(string text)
        {
            ToStringSubClass(text, new ProcessingInstruction());
        }

        void ToStringSubClass(string text, HtmlText p)
        {
            EndData();
            HandleData(text);
            EndData(p);
        }

        void HandleData(string data)
        {
            currentData.Add(data);
        }

        int ParseStartTag(int i)
        {            
            int endPos = CheckForWholeStartTag(i);
            if (endPos < 0)
            {
                return endPos;
            }
            string starttagText = html.Substring(i, endPos - i);

            Match match = tagFind.MatchAtIndex(html, i + 1);
            int k = i + 1 + match.Length;
            lastTag = html.Substring(i + 1, k - (i + 1)).ToLowerInvariant();

            List<TagAttribute> attributes = new List<TagAttribute>();

            while (k < endPos)
            {
                match = attributeFind.MatchAtIndex(html, k);
                if (!match.Success)
                {
                    break;
                }
                string attributeName = match.Groups[1].Value;
                string rest = match.Groups[2].Value;
                string attributeValue = match.Groups[3].Value;
                if (string.IsNullOrEmpty(rest))
                {
                    attributeValue = null;
                }
                else if ((attributeValue[0] == '\'' && attributeValue[attributeValue.Length - 1] == '\'') ||
                        attributeValue[0] == '"' && attributeValue[attributeValue.Length - 1] == '"')
                {
                    attributeValue = attributeValue.Substring(1, attributeValue.Length - 2);
                }
                attributes.Add(new TagAttribute(attributeName, attributeValue));
                k = k + match.Length;
            }

            Tag tag = Tag.Create(lastTag, attributes);

            string end = html.Substring(k, endPos - k).Trim();
            if (end != ">" && end != "/>")
            {
                var both = GetPosition();
                int lineNo = both.Key;
                int off = both.Value;
                if (starttagText.Contains('\n'))
                {
                    lineNo = lineNo + starttagText.Count(x => x == '\n');
                    off = starttagText.Length - starttagText.LastIndexOf('\n');
                }
                else
                {
                    off = off + starttagText.Length;
                }
                throw new HtmlParseException("Junk characters in start tag: " + starttagText);
            }

            if (end.EndsWith("/>", StringComparison.Ordinal))
            {
                HandleStartEndTag(tag);
            }
            else
            {
                HandleStartTag(tag);
                if (CDataContentElements.Contains(tag.TagName))
                {
                    SetCDataMode(true);
                }
            }

            return endPos;
        }

        void HandleStartEndTag(Tag tag)
        {
            HandleStartTag(tag);
            HandleEndTag(tag.TagName);
        }

        KeyValuePair<int, int> GetPosition()
        {
            return new KeyValuePair<int, int>(lineNumber, offset);
        }

        int CheckForWholeStartTag(int i)
        {
            Match m = locateStartTagEnd.MatchAtIndex(html, i);
            if (m.Success)
            {
                int j = i + m.Length;
                if (j >= html.Length)
                {
                    // end of input
                    return -1;
                }
                char next = html[j];
                if (next == '>')
                {
                    return j + 1;
                }
                if (next == '/')
                {
                    if (html.Substring(j, 2) == "/>")
                    {
                        return j + 2;
                    }
                    else
                    {
                        return -1;
                    }
                }
                if (char.IsLetter(next) || next == '=' || next == '/')
                {
                    // end of input in or before attribute value or / from /> ending
                    return -1;
                }
                UpdatePosition(i, j);
                throw new HtmlParseException("Malformed start tag");
            }
            throw new HtmlParseException("Invalid call");
        }

        int ParseEndTag(int i)
        {
            Match match = endEndTag.Match(html, i + 1);
            if (!match.Success)
            {
                return -1;
            }
            int j = match.Index + match.Length;
            match = endTagFind.MatchAtIndex(html, i);
            if (!match.Success)
            {
                throw new HtmlParseException("Bad end tag.");
            }

            string tag = match.Groups[1].Value;
            HandleEndTag(tag.ToLowerInvariant());
            SetCDataMode(false);
            return j;
        }

        int ParseComment(int i)
        {
            return ParseComment(i, true);
        }

        int ParseComment(int i, bool report)
        {
            Match match = commentClose.Match(html, i + 4);
            if (!match.Success)
            {
                return -1;
            }
            if (report)
            {
                int j = match.Index;
                HandleComment(html.Substring(i + 4, j - (i + 4)));
            }
            return match.Index + match.Length;
        }

        void HandleComment(string text)
        {
            ToStringSubClass(text, new Comment());
        }

        void HandleStartTag(Tag tag)
        {
            if (quoteStack.Count > 0)
            {
                //not a real tag
                string attrs = string.Empty;
                foreach (var attrib in tag.Attributes)
                {
                    attrs += string.Format(CultureInfo.InvariantCulture, " {0}=\"{1}\"", attrib.Name, attrib.Value);
                }
                HandleData(string.Format(CultureInfo.InvariantCulture, "<{0}{1}>", tag.TagName, attrs));
            }
            EndData();
            if (!tag.IsSelfClosing)
            {
                SmartPop(tag);
            }

            //Tag tag = Tag.Create(name);
            tag.Parent = currentTag;
            tag.Previous = root.Previous;
            if (root.Previous != null)
            {
                root.Previous.Next = tag;
            }
            root.Previous = tag;
            PushTag(tag);
            if (tag.IsSelfClosing)
            {
                PopTag();
            }
            if (quoteTags.ContainsKey(tag.TagName))
            {
                quoteStack.Push(tag.TagName);
            }
        }

        Tag PopTag()
        {
            tagStack.Pop();
            if (tagStack.Count > 0)
            {
                currentTag = tagStack.Peek();
            }
            return currentTag;
        }

        void PushTag(Tag tag)
        {
            if (currentTag != null)
            {
                currentTag.AddChild(tag);
            }
            tagStack.Push(tag);
            currentTag = tagStack.Peek();
        }

        void SmartPop(Tag tag)
        {
            //"""We need to pop up to the previous tag of this type, unless
            //one of this tag's nesting reset triggers comes between this
            //tag and the previous tag of this type, OR unless this tag is a
            //generic nesting trigger and another generic nesting trigger
            //comes between this tag and the previous tag of this type.

            //Examples:
            // <p>Foo<b>Bar *<p>* should pop to 'p', not 'b'.
            // <p>Foo<table>Bar *<p>* should pop to 'table', not 'p'.
            // <p>Foo<table><tr>Bar *<p>* should pop to 'tr', not 'p'.

            // <li><ul><li> *<li>* should pop to 'ul', not the first 'li'.
            // <tr><table><tr> *<tr>* should pop to 'table', not the first 'tr'
            // <td><tr><td> *<td>* should pop to 'tr', not the first 'td'
            //"""

            IAllowsNestingSelf nestableTag = tag as IAllowsNestingSelf;
            Tag popTo = null;
            bool inclusive = true;
            foreach (Tag t in tagStack)
            {
                if (t.TagName == tag.TagName && nestableTag == null)
                {
                    popTo = tag;
                    break;
                }
                else if (nestableTag != null && nestableTag.NestingBreakers.Contains(t.GetType()) ||
                    (nestableTag == null && tag.ResetsNesting && t.ResetsNesting))
                {
                    popTo = t;
                    inclusive = false;
                    break;
                }
            }
            if (popTo != null)
            {
                PopToTag(popTo.TagName, inclusive);
            }
        }

        void PopToTag(string popTo)
        {
            PopToTag(popTo, true);
        }

        void EndData(HtmlText containerClass)
        {
            if (currentData.Count > 0)
            {
                string data = currentData.Aggregate(new StringBuilder(), (x, y) => x.Append(y)).ToString();

                //TODO: clean this up
                char[] spaceChars = { (char)9, (char)10, (char)12, (char)13, (char)32 };
                if (string.IsNullOrEmpty(new string(data.Where(c => !spaceChars.Contains(c)).ToArray())))
                {
                    if (preserveWhitespaceTags.Intersect(tagStack.Select(tag => tag.TagName)).Count() == 0)
                    {
                        if (data.Contains("\n"))
                        {
                            data = "\n";
                        }
                        else
                        {
                            data = " ";
                        }
                    }
                }
                currentData = new List<string>();
                HtmlText o = containerClass;
                o.Value = data;
                o.Setup(currentTag, root.Previous);
                if (root.Previous != null)
                {
                    root.Previous.Next = o;
                }
                root.Previous = o;
                currentTag.AddChild(o);
            }
        }

        void EndData()
        {
            EndData(new HtmlText());
        }

        void HandleEndTag(string tag)
        {
            UnknownEndTag(tag);
        }

        void UnknownEndTag(string tag)
        {
            if (quoteStack.Count > 0 && quoteStack.Peek() != tag)
            {
                //not a real tag
                HandleData("</" + tag + ">");
                return;
            }
            EndData();
            PopToTag(tag);
            if (quoteStack.Count > 0 && quoteStack.Peek() == tag)
            {
                quoteStack.Pop();
            }
        }

        Tag PopToTag(string tag, bool inclusive)
        {
            if (tag == root.TagName)
            {
                return null;
            }

            int numPops = 0;
            Tag mostRecentTag = null;
            int i = 0;
            foreach (Tag t in tagStack)
            {
                i++;
                if (tag == t.TagName)
                {
                    numPops = i;
                    break;
                }
            }
            if (!inclusive)
            {
                numPops -= 1;
            }
            for (int j = 0; j < numPops; j++)
            {
                mostRecentTag = PopTag();
            }
            return mostRecentTag;
        }
    }
}
