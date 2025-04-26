namespace Ara3D.Utils
{
    public class HtmlAttribute
    {
        public readonly string Name;
        public readonly string Value;

        public HtmlAttribute(string name, string value)
            => (Name,Value) = (name,value.Trim());

        public static implicit operator (string, string)(HtmlAttribute attr)
            => (attr.Name, attr.Value);

        public static implicit operator HtmlAttribute((string, string) nameValueTuple)
            => new HtmlAttribute(nameValueTuple.Item1, nameValueTuple.Item2);

        public override string ToString()
            => HtmlExtensions.ToHtmlAttribute(Name, Value);
    }
}