using System;
using System.Collections.Generic;
using System.Text;

namespace Ara3D.Utils
{
    /// <summary>
    /// Used as the base class for building code strings
    /// that require indenting: like code, JSON, or XML.
    /// </summary>
    public class CodeBuilder<T> where T : CodeBuilder<T>
    {
        public StringBuilder sb { get; set; } = new StringBuilder();
        public bool AtNewLine { get; private set; }
        public int IndentLevel { get; set; }

        public T Indent()
        {
            IndentLevel++;
            return this as T;
        }

        public T Dedent()
        {
            IndentLevel--;
            return this as T;
        }

        public string Indentation()
        {
            return new string(' ', IndentLevel * 4);
        }

        public T Write(char c)
        {
            return Write(c.ToString());
        }

        public T Write(string s)
        {
            if (string.IsNullOrEmpty(s))
                return this as T;
            if (AtNewLine)
            {
                sb.Append(Indentation());
                AtNewLine = false;
            }
            sb.Append(s);
            return this as T;
        }

        public T WriteLine()
        {
            AtNewLine = true;
            sb.AppendLine();
            return this as T;
        }

        public T WriteLine(string s)
        {
            return Write(s).WriteLine();
        }

        public T Parenthesize(Func<T, T> f)
        {
            return f(Write("(") as T).Write(")");
        }

        public T WriteCommaList<TElement>(IEnumerable<TElement> elements, Func<T, TElement, T> fElement)
        {
            return WriteList(elements, fElement, w => w.Write(", "));
        }

        public T WriteList<TElement>(IEnumerable<TElement> elements, Func<T, TElement, T> fElement, Func<T, T> fSeparator = null)
        {
            var first = true;
            var r = this as T;
            foreach (var element in elements)
            {
                if (!first)
                {
                    r = fSeparator?.Invoke(r) ?? r;
                }
                else
                {
                    first = false;
                }

                r = fElement(r, element);
            }
            return r;
        }

        public T Brace(Func<T, T> f)
            => f(Write("{").Indent().WriteLine()).Dedent().WriteLine("}");
       
        public T WriteIf(bool condition, Func<T, T> f)
            => condition ? f(this as T) : this as T;

        public override string ToString()
            => sb.ToString();
       
        public T WriteSpace()
            => Write(" ");

        public T WriteStartBlock(string delim = "{")
            => WriteLine(delim).Indent();

        public T WriteEndBlock(string delim = "}")
            => Dedent().WriteLine(delim);
    }

    public class CodeBuilder : CodeBuilder<CodeBuilder>
    { }
}