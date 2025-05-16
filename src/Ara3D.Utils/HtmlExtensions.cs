namespace Ara3D.Utils
{
    public static class HtmlExtensions
    {
        public static string EscapeCommonHtmlEntities(this string html)
            => html.Replace("<", "&lt;").Replace("&", "&amp;");

        public static string EscapeAttributeValueText(this string html)
            => html.EscapeCommonHtmlEntities().Replace("\"", "&quot;").Replace("\'", "&apos;");

        public static string ToHtmlAttribute(string name, string value)
            => $"{name} = '{value.EscapeAttributeValueText()}'";
    }
}