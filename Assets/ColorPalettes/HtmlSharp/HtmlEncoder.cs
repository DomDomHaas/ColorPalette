using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace HtmlSharp
{
    public class HtmlEncoder
    {
        Dictionary<string, char> entities = new Dictionary<string, char>()
        {
            {"quot", '\"'},
            {"amp", '&'},
            {"lt", '<'},
            {"gt", '>'},
            {"nbsp", ' '},
            {"iexcl", '¡'},
            {"cent", '¢'},
            {"pound", '£'},
            {"curren", '¤'},
            {"yen", '¥'},
            {"brvbar", '¦'},
            {"sect", '§'},
            {"uml", '¨'},
            {"copy", '©'},
            {"ordf", 'ª'},
            {"laquo", '«'},
            {"not", '¬'},
            {"shy", '­'},
            {"reg", '®'},
            {"macr", '¯'},
            {"deg", '°'},
            {"plusmn", '±'},
            {"sup2", '²'},
            {"sup3", '³'},
            {"acute", '´'},
            {"micro", 'µ'},
            {"para", '¶'},
            {"middot", '·'},
            {"cedil", '¸'},
            {"sup1", '¹'},
            {"ordm", 'º'},
            {"raquo", '»'},
            {"frac14", '¼'},
            {"frac12", '½'},
            {"frac34", '¾'},
            {"iquest", '¿'},
            {"Agrave", 'À'},
            {"Aacute", 'Á'},
            {"Acirc", 'Â'},
            {"Atilde", 'Ã'},
            {"Auml", 'Ä'},
            {"Aring", 'Å'},
            {"AElig", 'Æ'},
            {"Ccedil", 'Ç'},
            {"Egrave", 'È'},
            {"Eacute", 'É'},
            {"Ecirc", 'Ê'},
            {"Euml", 'Ë'},
            {"Igrave", 'Ì'},
            {"Iacute", 'Í'},
            {"Icirc", 'Î'},
            {"Iuml", 'Ï'},
            {"ETH", 'Ð'},
            {"Ntilde", 'Ñ'},
            {"Ograve", 'Ò'},
            {"Oacute", 'Ó'},
            {"Ocirc", 'Ô'},
            {"Otilde", 'Õ'},
            {"Ouml", 'Ö'},
            {"times", '×'},
            {"Oslash", 'Ø'},
            {"Ugrave", 'Ù'},
            {"Uacute", 'Ú'},
            {"Ucirc", 'Û'},
            {"Uuml", 'Ü'},
            {"Yacute", 'Ý'},
            {"THORN", 'Þ'},
            {"szlig", 'ß'},
            {"agrave", 'à'},
            {"aacute", 'á'},
            {"acirc", 'â'},
            {"atilde", 'ã'},
            {"auml", 'ä'},
            {"aring", 'å'},
            {"aelig", 'æ'},
            {"ccedil", 'ç'},
            {"egrave", 'è'},
            {"eacute", 'é'},
            {"ecirc", 'ê'},
            {"euml", 'ë'},
            {"igrave", 'ì'},
            {"iacute", 'í'},
            {"icirc", 'î'},
            {"iuml", 'ï'},
            {"eth", 'ð'},
            {"ntilde", 'ñ'},
            {"ograve", 'ò'},
            {"oacute", 'ó'},
            {"ocirc", 'ô'},
            {"otilde", 'õ'},
            {"ouml", 'ö'},
            {"divide", '÷'},
            {"oslash", 'ø'},
            {"ugrave", 'ù'},
            {"uacute", 'ú'},
            {"ucirc", 'û'},
            {"uuml", 'ü'},
            {"yacute", 'ý'},
            {"thorn", 'þ'},
            {"yuml", 'ÿ'},
            {"OElig", '\u0152'},
            {"oelig", '\u0153'},
            {"Scaron", '\u0160'},
            {"scaron", '\u0161'},
            {"Yuml", '\u0178'},
            {"fnof", '\u0192'},
            {"circ", '\u02c6'},
            {"tilde", '\u02dc'},
            {"Alpha", '\u0391'},
            {"Beta", '\u0392'},
            {"Gamma", '\u0393'},
            {"Delta", '\u0394'},
            {"Epsilon", '\u0395'},
            {"Zeta", '\u0396'},
            {"Eta", '\u0397'},
            {"Theta", '\u0398'},
            {"Iota", '\u0399'},
            {"Kappa", '\u039a'},
            {"Lambda", '\u039b'},
            {"Mu", '\u039c'},
            {"Nu", '\u039d'},
            {"Xi", '\u039e'},
            {"Omicron", '\u039f'},
            {"Pi", '\u03a0'},
            {"Rho", '\u03a1'},
            {"Sigma", '\u03a3'},
            {"Tau", '\u03a4'},
            {"Upsilon", '\u03a5'},
            {"Phi", '\u03a6'},
            {"Chi", '\u03a7'},
            {"Psi", '\u03a8'},
            {"Omega", '\u03a9'},
            {"alpha", '\u03b1'},
            {"beta", '\u03b2'},
            {"gamma", '\u03b3'},
            {"delta", '\u03b4'},
            {"epsilon", '\u03b5'},
            {"zeta", '\u03b6'},
            {"eta", '\u03b7'},
            {"theta", '\u03b8'},
            {"iota", '\u03b9'},
            {"kappa", '\u03ba'},
            {"lambda", '\u03bb'},
            {"mu", '\u03bc'},
            {"nu", '\u03bd'},
            {"xi", '\u03be'},
            {"omicron", '\u03bf'},
            {"pi", '\u03c0'},
            {"rho", '\u03c1'},
            {"sigmaf", '\u03c2'},
            {"sigma", '\u03c3'},
            {"tau", '\u03c4'},
            {"upsilon", '\u03c5'},
            {"phi", '\u03c6'},
            {"chi", '\u03c7'},
            {"psi", '\u03c8'},
            {"omega", '\u03c9'},
            {"thetasym", '\u03d1'},
            {"upsih", '\u03d2'},
            {"piv", '\u03d6'},
            {"ensp", '\u2002'},
            {"emsp", '\u2003'},
            {"thinsp", '\u2009'},
            {"zwnj", '\u200c'},
            {"zwj", '\u200d'},
            {"lrm", '\u200e'},
            {"rlm", '\u200f'},
            {"ndash", '\u2013'},
            {"mdash", '\u2014'},
            {"lsquo", '\u2018'},
            {"rsquo", '\u2019'},
            {"sbquo", '\u201a'},
            {"ldquo", '\u201c'},
            {"rdquo", '\u201d'},
            {"bdquo", '\u201e'},
            {"dagger", '\u2020'},
            {"Dagger", '\u2021'},
            {"bull", '\u2022'},
            {"hellip", '\u2026'},
            {"permil", '\u2030'},
            {"prime", '\u2032'},
            {"Prime", '\u2033'},
            {"lsaquo", '\u2039'},
            {"rsaquo", '\u203a'},
            {"oline", '\u203e'},
            {"frasl", '\u2044'},
            {"euro", '\u20ac'},
            {"image", '\u2111'},
            {"weierp", '\u2118'},
            {"real", '\u211c'},
            {"trade", '\u2122'},
            {"alefsym", '\u2135'},
            {"larr", '\u2190'},
            {"uarr", '\u2191'},
            {"rarr", '\u2192'},
            {"darr", '\u2193'},
            {"harr", '\u2194'},
            {"crarr", '\u21b5'},
            {"lArr", '\u21d0'},
            {"uArr", '\u21d1'},
            {"rArr", '\u21d2'},
            {"dArr", '\u21d3'},
            {"hArr", '\u21d4'},
            {"forall", '\u2200'},
            {"part", '\u2202'},
            {"exist", '\u2203'},
            {"empty", '\u2205'},
            {"nabla", '\u2207'},
            {"isin", '\u2208'},
            {"notin", '\u2209'},
            {"ni", '\u220b'},
            {"prod", '\u220f'},
            {"sum", '\u2211'},
            {"minus", '\u2212'},
            {"lowast", '\u2217'},
            {"radic", '\u221a'},
            {"prop", '\u221d'},
            {"infin", '\u221e'},
            {"ang", '\u2220'},
            {"and", '\u2227'},
            {"or", '\u2228'},
            {"cap", '\u2229'},
            {"cup", '\u222a'},
            {"int", '\u222b'},
            {"there4", '\u2234'},
            {"sim", '\u223c'},
            {"cong", '\u2245'},
            {"asymp", '\u2248'},
            {"ne", '\u2260'},
            {"equiv", '\u2261'},
            {"le", '\u2264'},
            {"ge", '\u2265'},
            {"sub", '\u2282'},
            {"sup", '\u2283'},
            {"nsub", '\u2284'},
            {"sube", '\u2286'},
            {"supe", '\u2287'},
            {"oplus", '\u2295'},
            {"otimes", '\u2297'},
            {"perp", '\u22a5'},
            {"sdot", '\u22c5'},
            {"lceil", '\u2308'},
            {"rceil", '\u2309'},
            {"lfloor", '\u230a'},
            {"rfloor", '\u230b'},
            {"lang", '\u2329'},
            {"rang", '\u232a'},
            {"loz", '\u25ca'},
            {"spades", '\u2660'},
            {"clubs", '\u2663'},
            {"hearts", '\u2665'},
            {"diams", '\u2666'}
        };

        public string Decode(string html)
        {
            char[] endingChars = new char[] { ';', '&' };
            StringBuilder output = new StringBuilder();
            if (html != null)
            {
                if (html.IndexOf('&') < 0)
                {
                    output.Append(html);
                }
                else
                {
                    int n = html.Length;
                    for (int i = 0; i < n; i++)
                    {
                        char value = html[i];
                        if (value == '&')
                        {
                            int index = html.IndexOfAny(endingChars, i + 1);
                            if ((index > 0) && (html[index] == ';'))
                            {
                                string entity = html.Substring(i + 1, (index - i) - 1);
                                if ((entity.Length > 1) && (entity[0] == '#'))
                                {
                                    try
                                    {
                                        if ((entity[1] == 'x') || (entity[1] == 'X'))
                                        {
                                            value = (char)((ushort)int.Parse(entity.Substring(2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture));
                                        }
                                        else
                                        {
                                            value = (char)((ushort)int.Parse(entity.Substring(1), CultureInfo.InvariantCulture));
                                        }
                                        i = index;
                                    }
                                    catch (FormatException)
                                    {
                                        i++;
                                    }
                                    catch (ArgumentException)
                                    {
                                        i++;
                                    }
                                }
                                else
                                {
                                    i = index;
                                    if (entities.ContainsKey(entity))
                                    {
                                        value = entities[entity];
                                    }
                                    else
                                    {
                                        output.Append('&');
                                        output.Append(entity);
                                        output.Append(';');
                                        continue;
                                    }
                                }
                            }
                        }
                        output.Append(value);
                    }
                }
            }
            return output.ToString();
        }
   }
}
