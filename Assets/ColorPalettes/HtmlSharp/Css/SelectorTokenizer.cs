using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlSharp.Extensions;

namespace HtmlSharp.Css
{
    public class SelectorTokenizer
    {
        static readonly Regex unicode = new Regex("\\\\[0-9a-f]{1,6}(\r\n|[ \n\r\t\f])?");
        static readonly Regex escape = new Regex(string.Format("({0})|\\\\[^\n\r\f0-9a-f]", unicode));
        static readonly Regex nonascii = new Regex("[^\x0-\x7F]");
        static readonly Regex nmchar = new Regex(string.Format("[_a-z0-9-]|({0})|({1})", nonascii, escape));
        static readonly Regex nmstart = new Regex(string.Format("[_a-z]|({0})|({1})", nonascii, escape));
        static readonly Regex ident = new Regex(string.Format("[-]?({0})({1})*", nmstart, nmchar));
        static readonly Regex name = new Regex(string.Format("({0})+", nmchar));
        static readonly Regex num = new Regex(@"[0-9]+|[0-9]*\.[0-9]+");
        static readonly Regex nl = new Regex("\n|\r\n|\r|\f");
        static readonly Regex str1 = new Regex(string.Format("\"([^\n\r\f\\\"]|\\\\({0})|({1})|({2}))*\"", nl, nonascii, escape));
        static readonly Regex str2 = new Regex(string.Format("'([^\n\r\f\\']|\\\\({0})|({1})|({2}))*'", nl, nonascii, escape));
        static readonly Regex str = new Regex(string.Format("({0})|({1})", str1, str2));
        static readonly Regex invalid1 = new Regex(string.Format("\"([^\n\r\f\\\"]|\\\\({0})|({1})|({2}))*", nl, nonascii, escape));
        static readonly Regex invalid2 = new Regex(string.Format("'([^\n\r\f\\']|\\\\({0})|({1})|({2}))*", nl, nonascii, escape));
        static readonly Regex invalid = new Regex(string.Format("({0})|({1})", invalid1, invalid2));
        static readonly Regex w = new Regex("[ \t\r\n\f]*");

        static readonly Regex s = new Regex("[ \t\r\n\f]+");
        static readonly Regex includes = new Regex("~=");

        static readonly Regex dashmatch = new Regex(@"\|=");
        static readonly Regex prefixmatch = new Regex(@"\^=");
        static readonly Regex suffixMatch = new Regex(@"\$=");
        static readonly Regex substringMatch = new Regex(@"\*=");

        static readonly Regex function = new Regex(string.Format(@"({0})\(", ident));
        static readonly Regex hash = new Regex(string.Format("#({0})", name));
        static readonly Regex plus = new Regex(string.Format(@"({0})\+", w));
        static readonly Regex greater = new Regex(string.Format(@"({0})>", w));
        static readonly Regex comma = new Regex(string.Format(@"({0}),", w));
        static readonly Regex tilde = new Regex(string.Format(@"({0})~", w));
        static readonly Regex not = new Regex(@":not\(");
        static readonly Regex atKeyword = new Regex(string.Format("@({0})", ident));
        static readonly Regex percentage = new Regex(string.Format("({0})%", num));
        static readonly Regex dimension = new Regex(string.Format("({0})({1})", num, ident));
        static readonly Regex cdo = new Regex("<!--");
        static readonly Regex cdc = new Regex("-->");

        static readonly Regex uri = new Regex(string.Format(@"url\(({0})({1})({0})\)", w, str));
        static readonly Regex uri2 = new Regex(string.Format(@"url\(({0})([!#$%&*-~]|({1})|({2}))*({0})\)", w, nonascii, escape));
        static readonly Regex unicodeRange = new Regex(@"U\+[0-9a-f?]{1,6}(-[0-9a-f]{1,6})?");
        static readonly Regex comment = new Regex(@"/\*[^*]*\*+([^/*][^*]*\*+)*/");

        static Dictionary<Regex, SelectorTokenType> tokenMap = new Dictionary<Regex, SelectorTokenType>()
        {
            {s, SelectorTokenType.WhiteSpace},
            {includes, SelectorTokenType.Includes},
            {dashmatch, SelectorTokenType.DashMatch},
            {prefixmatch, SelectorTokenType.PrefixMatch},
            {suffixMatch, SelectorTokenType.SuffixMatch},
            {substringMatch,  SelectorTokenType.SubstringMatch},
            {ident, SelectorTokenType.Ident},
            {str, SelectorTokenType.String},
            {function, SelectorTokenType.Function},
            {num, SelectorTokenType.Number},
            {hash, SelectorTokenType.Hash},
            {plus, SelectorTokenType.Plus},
            {greater, SelectorTokenType.Greater},
            {comma, SelectorTokenType.Comma},
            {tilde, SelectorTokenType.Tilde},
            {not, SelectorTokenType.Not},
            {atKeyword, SelectorTokenType.AtKeyword},
            {invalid, SelectorTokenType.Invalid},
            {percentage, SelectorTokenType.Percentage},
            {dimension, SelectorTokenType.Dimension},
            {cdo, SelectorTokenType.CommentOpen},
            {cdc, SelectorTokenType.CommentClose},
            {uri, SelectorTokenType.Uri},
            {uri2, SelectorTokenType.Uri},
            {unicodeRange, SelectorTokenType.UnicodeRange}
        };

        static List<Regex> tokenMatchers = new List<Regex>()
        {
            unicodeRange, uri, uri2, cdc, cdo, dimension, percentage, atKeyword, not, includes, tilde, comma, greater,
            plus, hash, num, function, str, ident, substringMatch, suffixMatch, prefixmatch, dashmatch, s, invalid
        };

        public IEnumerable<SelectorToken> Tokenize(string input)
        {
            int currentPosition = 0;
            while (currentPosition < input.Length)
            {
                var token = tokenMatchers.FirstOrDefault(t => t.MatchAtIndex(input, currentPosition).Success);
                if (token != null)
                {
                    Match m = token.MatchAtIndex(input, currentPosition);
                    yield return new SelectorToken(tokenMap[token], m.Value);
                    currentPosition += m.Length;
                    continue;
                }
                else
                {
                    yield return new SelectorToken(SelectorTokenType.Text, input[currentPosition].ToString());
                    currentPosition++;
                }
            }
        }
    }
}
