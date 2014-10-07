using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlSharp.Css
{
    public enum SelectorTokenType
    {
        WhiteSpace,
        Includes,
        DashMatch,
        PrefixMatch,
        SuffixMatch,
        SubstringMatch,
        Ident,
        String,
        Function,
        Number,
        Hash,
        Plus,
        Greater,
        Comma,
        Tilde,
        Not,
        AtKeyword,
        Invalid,
        Percentage,
        Dimension,
        CommentOpen,
        CommentClose,
        Uri,
        UnicodeRange,
        Text
    }
}
