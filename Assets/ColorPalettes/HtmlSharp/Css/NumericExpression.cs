using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlSharp.Css
{
    public class NumericExpression : Expression
    {
        int n;
        int b;

        public NumericExpression(int n, int b)
        {
            this.n = n;
            this.b = b;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                NumericExpression t = (NumericExpression)obj;
                return n == t.n && b == t.b;
            }
        }

        public override int GetHashCode()
        {
            return n.GetHashCode() ^ b.GetHashCode();
        }

        public override IEnumerable<int> GetValues()
        {
            if (n == 0)
            {
                yield return b;
            }
            else
            {
                for (int i = 0; ; i++)
                {
                    yield return n * i + b;
                }
            }
        }
    }
}
