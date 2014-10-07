using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlSharp.Elements;
using HtmlSharp.Extensions;
using System.Collections.Specialized;

namespace HtmlSharp.Css
{
    public class SelectorParser
    {
        SelectorTokenizer tokenizer = new SelectorTokenizer();
        List<SelectorToken> tokens = new List<SelectorToken>();
        int currentPosition = 0;

        SelectorToken CurrentToken { get { return tokens.ElementAtOrDefault(currentPosition); } }
        bool End { get { return currentPosition >= tokens.Count; } }

        public SelectorsGroup Parse(string selector)
        {
            tokens = tokenizer.Tokenize(selector).ToList();

            List<Selector> selectors = new List<Selector>();
            while (!End)
            {
                selectors.Add(ParseSelector());
                if (!End)
                {
                    Consume(SelectorTokenType.Comma);
                }
            }

            return new SelectorsGroup(selectors);
        }

        string Consume(SelectorTokenType tokenType)
        {
            Expect(tokenType);
            string token = CurrentToken.Text;
            currentPosition++;
            return token;
        }

        void Consume(string tokenText)
        {
            Expect(tokenText);
            currentPosition++;
        }

        private void Expect(string tokenText)
        {
            if (CurrentToken == null)
            {
                ParseErrorIfEnd();
            }
            else if (CurrentToken.Text != tokenText)
            {
                ParseError(string.Format(
                    "Expected token {0} but found {1}", tokenText, CurrentToken.TokenType));
            }
        }

        void Expect(SelectorTokenType tokenType)
        {
            ParseErrorIfEnd();
            if (CurrentToken.TokenType != tokenType)
            {
                ParseError(string.Format(
                    "Expected token {0} but found {1}", tokenType, CurrentToken.TokenType));
            }
        }

        void ParseError(string message)
        {
            throw new FormatException(message);
        }

        void ParseErrorIfEnd()
        {
            if (End)
            {
                ParseError("Unexpected end of css selector");
            }
        }

        private Selector ParseSelector()
        {
            List<SimpleSelectorSequence> simpleSelectorSequences = new List<SimpleSelectorSequence>();
            List<Combinator> combinators = new List<Combinator>();

            Combinator combinator = null;
            do
            {
                simpleSelectorSequences.Add(ParseSimpleSelectorSequence());

                combinator = ParseCombinator();
                if (combinator != null)
                {
                    combinators.Add(combinator);
                }
            }
            while (combinator != null);

            return new Selector(simpleSelectorSequences, combinators);
        }

        private Combinator ParseCombinator()
        {
            Combinator combinator = null;
            if (!End)
            {
                Dictionary<SelectorTokenType, Combinator> lookup = new Dictionary<SelectorTokenType, Combinator>()
                {
                    { SelectorTokenType.Plus, new AdjacentSiblingCombinator() },
                    { SelectorTokenType.Greater, new ChildCombinator() },
                    { SelectorTokenType.Tilde, new GeneralSiblingCombinator() },
                    { SelectorTokenType.WhiteSpace, new DescendantCombinator() }
                };

                if (lookup.TryGetValue(CurrentToken.TokenType, out combinator))
                {
                    currentPosition++;
                    SkipWhiteSpace();
                }
            }
            return combinator;
        }

        private SimpleSelectorSequence ParseSimpleSelectorSequence()
        {
            ParseErrorIfEnd();

            TypeSelector typeSelector = ParseTypeSelector() ?? ParseUniversalSelector();

            List<IFilter> filters = new List<IFilter>();
            while (true)
            {
                if (End)
                {
                    break;
                }
                IFilter filterSelector = ParseIDFilter() ?? ParseClassFilter() ??
                    ParseAttributeFilter() ?? ParsePseudoFilter() ?? ParseNegationFilter();
                if (filterSelector != null)
                {
                    filters.Add(filterSelector);
                }
                else
                {
                    break;
                }
            }
            if (typeSelector == null)
            {
                if (filters.Count == 0)
                {
                    ParseError("Expected hash, class, attrib, pseudo, or negation");
                }
                typeSelector = new UniversalSelector();
            }
            return new SimpleSelectorSequence(typeSelector, filters);
        }

        private IFilter ParseNegationFilter()
        {
            IFilter filter = null;
            if(CurrentToken.TokenType == SelectorTokenType.Not)
            {
                Consume(SelectorTokenType.Not);
                SkipWhiteSpace();
                ParseErrorIfEnd();
                filter = ParseNegationArgument();
                if (filter == null)
                {
                    ParseError("Expected negation filter");
                }
                SkipWhiteSpace();
                Consume(")");
            }
            return filter;
        }

        private IFilter ParseNegationArgument()
        {
            IFilter filter = null;
            TypeSelector typeSelector = ParseTypeSelector() ?? ParseUniversalSelector();
            if (typeSelector != null)
            {
                filter = new NegationTypeFilter(typeSelector);
            }
            IFilter negationFilter = ParseIDFilter() ?? ParseClassFilter() ??
                ParseAttributeFilter() ?? ParsePseudoFilter();
            if (negationFilter != null)
            {
                filter = new NegationFilter(negationFilter);
            }
            return filter;
        }

        private IFilter ParsePseudoFilter()
        {
            IFilter selector = null;
            if (CurrentToken.Text == ":")
            {
                currentPosition++;
                ParseErrorIfEnd();
                if (CurrentToken.Text == ":")
                {
                    ParseError(":: selectors do not apply to static html, so were not implemented.");
                }
                else if (CurrentToken.TokenType == SelectorTokenType.Ident)
                {
                    var filterLookup = new Dictionary<string, IFilter>()
                    {
                        { "root", new RootFilter() },
                        { "first-child", new FirstChildFilter() },
                        { "last-child", new LastChildFilter() },
                        { "first-of-type", new FirstOfTypeFilter() },
                        { "last-of-type", new LastOfTypeFilter() },
                        { "only-child", new OnlyChildFilter() },
                        { "only-of-type", new OnlyOfTypeFilter() },
                        { "empty", new EmptyFilter() },
                        { "enabled", new EnabledFilter() },
                        { "disabled", new DisabledFilter() },
                        { "checked", new CheckedFilter() }
                    };

                    if (filterLookup.ContainsKey(CurrentToken.Text))
                    {
                        selector = filterLookup[CurrentToken.Text];
                        currentPosition++;
                    }
                    else
                    {
                        ParseError("Unsupported or invalid pseudo selector :" + filterLookup);
                    }
                }
                else if (CurrentToken.TokenType == SelectorTokenType.Function)
                {
                    var filterLookup = new Dictionary<string, Func<Expression, IFilter>>()
                    {
                        { "nth-child(", e => new NthChildFilter(e) },
                        { "nth-last-child(", e => new NthLastChildFilter(e) },
                        { "nth-of-type(", e => new NthOfTypeFilter(e) },
                        { "nth-last-of-type(", e => new NthLastOfTypeFilter(e) }
                    };

                    if (filterLookup.ContainsKey(CurrentToken.Text))
                    {
                        string text = CurrentToken.Text;
                        currentPosition++;
                        ParseErrorIfEnd();
                        SkipWhiteSpace();
                        ParseErrorIfEnd();
                        Expression expression = ParseExpression();
                        if (expression == null)
                        {
                            ParseError("Unable to parse expression");
                        }
                        selector = filterLookup[text](expression);
                    }
                    else if (CurrentToken.Text == "lang(")
                    {
                        currentPosition++;
                        ParseErrorIfEnd();
                        SkipWhiteSpace();
                        ParseErrorIfEnd();
                        if (CurrentToken.TokenType == SelectorTokenType.Ident)
                        {
                            selector = new LangFilter(CurrentToken.Text);
                        }
                        else if (CurrentToken.TokenType == SelectorTokenType.String)
                        {
                            selector = new LangFilter(CurrentToken.Text.Substring(1, CurrentToken.Text.Length - 2));
                        }

                    }
                    else
                    {
                        ParseError("Unrecognized pseudo function");
                    }
                    currentPosition++;
                    Consume(")");
                }
            }
            return selector;
        }

        private Expression ParseExpression()
        {
            bool negative = false;
            Expression expression = null;
            if (CurrentToken.Text == "-")
            {
                negative = true;
                currentPosition++;
            }
            if (CurrentToken.TokenType == SelectorTokenType.Dimension || CurrentToken.Text == "n" || CurrentToken.Text == "-n")
            {
                string dimension = CurrentToken.Text;
                int dim;
                string negativeSecond = null;

                Match m = Regex.Match(CurrentToken.Text, @"(\d+n)-(\d+)");
                if (m.Success)
                {
                    dimension = m.Groups[1].Value;
                    negativeSecond = m.Groups[2].Value;
                }

                if (int.TryParse(dimension.Substring(0, dimension.Length - 1), out dim) || CurrentToken.Text == "n" || CurrentToken.Text == "-n")
                {
                    if (CurrentToken.Text == "n")
                    {
                        dim = 1;
                    }
                    else if (CurrentToken.Text == "-n")
                    {
                        dim = -1;
                    }
                    dim = negative ? dim * -1 : dim;
                    currentPosition++;
                    if (CurrentToken.TokenType == SelectorTokenType.Plus)
                    {
                        currentPosition++;
                        if (CurrentToken.TokenType == SelectorTokenType.Number)
                        {
                            //xn+b
                            int num;
                            if (int.TryParse(CurrentToken.Text, out num))
                            {
                                expression = new NumericExpression(dim, num);
                            }
                            else
                            {
                                //parse error
                            }
                        }
                    }
                    else if (negativeSecond != null)
                    {
                        //xn-b
                        int num;
                        if (int.TryParse(negativeSecond, out num))
                        {
                            expression = new NumericExpression(dim, -num);
                            currentPosition--;
                        }
                        else
                        {
                            //parse error
                        }
                    }
                    else if (CurrentToken.Text == ")")
                    {
                        expression = new NumericExpression(dim, 0);
                        currentPosition--;
                    }
                }
                else
                {
                    //parse error
                }
            }
            else if (CurrentToken.TokenType == SelectorTokenType.Number)
            {
                //b
                int num;
                if (int.TryParse(CurrentToken.Text, out num))
                {
                    expression = new NumericExpression(0, num * (negative ? -1 : 1));
                }
                else
                {
                    //parse error
                }
            }
            else if (CurrentToken.TokenType == SelectorTokenType.Ident)
            {
                //odd or even
                if (CurrentToken.Text == "odd")
                {
                    expression = new OddExpression();
                }
                else if (CurrentToken.Text == "even")
                {
                    expression = new OddExpression();
                }
                else
                {
                    //parse error
                }
            }
            else if (CurrentToken.TokenType == SelectorTokenType.String)
            {
                string withoutQuotes = CurrentToken.Text.Substring(1, CurrentToken.Text.Length - 2);
                if (CurrentToken.Text == "odd")
                {
                    expression = new OddExpression();
                }
                else if (CurrentToken.Text == "even")
                {
                    expression = new OddExpression();
                }
                else
                {
                    //parse error
                }
            }

            return expression;
        }

        private IFilter ParseAttributeFilter()
        {
            IFilter selector = null;

            if (CurrentToken.Text == "[")
            {
                currentPosition++;
                SkipWhiteSpace();
                SelectorNamespacePrefix ns = ParseNamespacePrefix();
                string attributeType = Consume(SelectorTokenType.Ident);
                SkipWhiteSpace();
                var filterLookup = new Dictionary<SelectorToken, Func<string, IFilter>>()
                {
                    { new  SelectorToken(SelectorTokenType.PrefixMatch, "^="), 
                        text => new AttributePrefixFilter(attributeType, text) },
                    { new SelectorToken(SelectorTokenType.SuffixMatch, "$="),
                        text => new AttributeSuffixFilter(attributeType, text) },
                    { new SelectorToken(SelectorTokenType.SubstringMatch, "*="),
                        text => new AttributeSubstringFilter(attributeType, text) },
                    { new SelectorToken(SelectorTokenType.Text, "="),
                        text => new AttributeExactFilter(attributeType, text) },
                    { new SelectorToken(SelectorTokenType.Includes, "~="),
                        text => new AttributeIncludesFilter(attributeType, text) },
                    { new SelectorToken(SelectorTokenType.DashMatch, "|="),
                        text => new AttributeDashFilter(attributeType, text) }
                };
                if (filterLookup.ContainsKey(CurrentToken))
                {
                    SelectorToken token = CurrentToken;
                    currentPosition++;
                    SkipWhiteSpace();
                    if (CurrentToken.TokenType == SelectorTokenType.Ident)
                    {
                        selector = filterLookup[token](CurrentToken.Text);
                    }
                    else if (CurrentToken.TokenType == SelectorTokenType.String)
                    {
                        selector = filterLookup[token](CurrentToken.Text.Substring(1, CurrentToken.Text.Length - 2));
                    }
                    else
                    {
                        ParseError("Unexpected token type for attribute matcher");
                    }
                    currentPosition++;
                    SkipWhiteSpace();
                }
                else
                {
                    Expect("]");
                }

                if (CurrentToken.Text == "]" && selector == null)
                {
                    selector = new AttributeFilter(attributeType);
                    currentPosition++;
                }
                else if (CurrentToken.Text == "]" && selector != null)
                {
                    currentPosition++;
                }
                else
                {
                    //parse error lolz
                }
            }

            return selector;
        }

        private void SkipWhiteSpace()
        {
            if (CurrentToken.TokenType == SelectorTokenType.WhiteSpace)
            {
                currentPosition++;
            }
        }

        private IFilter ParseClassFilter()
        {
            IFilter selector = null;
            if (CurrentToken.Text == ".")
            {
                currentPosition++;
                ParseErrorIfEnd();
                if (CurrentToken.TokenType == SelectorTokenType.Ident)
                {
                    selector = new ClassFilter(CurrentToken.Text);
                }

                currentPosition++;
            }
            return selector;
        }

        private IFilter ParseIDFilter()
        {
            IFilter selector = null;
            if (CurrentToken.TokenType == SelectorTokenType.Hash)
            {
                selector = new IDFilter(CurrentToken.Text.Substring(1));
                currentPosition++;
            }
            return selector;
        }

        private TypeSelector ParseUniversalSelector()
        {
            TypeSelector selector = null;
            SelectorNamespacePrefix prefix = ParseNamespacePrefix();

            if (CurrentToken != null && CurrentToken.Text == "*")
            {
                selector = new UniversalSelector(prefix);
                currentPosition++;
            }

            return selector;
        }

        private TypeSelector ParseTypeSelector()
        {
            TypeSelector selector = null;
            SelectorNamespacePrefix prefix = ParseNamespacePrefix();

            if (CurrentToken != null && CurrentToken.TokenType == SelectorTokenType.Ident)
            {
                selector = new TypeSelector(CurrentToken.Text, prefix);
                currentPosition++;
            }

            return selector;
        }

        private SelectorNamespacePrefix ParseNamespacePrefix()
        {
            SelectorNamespacePrefix prefix = null;

            if (CurrentToken != null)
            {
                if (CurrentToken.TokenType == SelectorTokenType.Ident)
                {
                    string ident = CurrentToken.Text;
                    currentPosition++;
                    if (CurrentToken == null)
                    {
                        currentPosition--;
                    }
                    else if (CurrentToken.Text == "|")
                    {
                        prefix = new SelectorNamespacePrefix(ident);
                        currentPosition++;
                    }
                    else
                    {
                        currentPosition--;
                    }
                }
                else if (CurrentToken.Text == "*")
                {
                    currentPosition++;
                    if (CurrentToken == null)
                    {
                        currentPosition--;
                    }
                    else if (CurrentToken.Text == "|")
                    {
                        prefix = new SelectorNamespacePrefix("*");
                        currentPosition++;
                    }
                    else
                    {
                        currentPosition--;
                    }
                }
                else if (CurrentToken.Text == "|")
                {
                    //TODO: is this supposed to mean universal selector?
                    prefix = new SelectorNamespacePrefix("");
                    currentPosition++;
                }
            }
            return prefix;
        }
    }


}
