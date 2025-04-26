using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Ara3D.Utils
{
    public static class XmlUtil
    {
        public static XDocument LoadXml(this FilePath filePath)
            => XDocument.Load(filePath);

        public static FilePath SaveXml(this XDocument document, FilePath filePath)
        {
            document.Save(filePath);
            return filePath;
        }

        public static XDocument ApplyModification(this XDocument document, Action<XDocument> modification)
        {
            modification(document);
            return document;
        }

        public static FilePath ModifyXmlDocument(this FilePath srcFilePath, Action<XDocument> modification, FilePath destFilePath = null)
            => srcFilePath.LoadXml().ApplyModification(modification).SaveXml(destFilePath ?? srcFilePath);

        public static XDocument SetAttribute(this XDocument document, string elementName, string attributeName,
            string newValue)
            => document.SetAttributesWhere(e => e.Name == elementName, attributeName, newValue);

        // https://stackoverflow.com/questions/17928524/change-an-attribute-value-of-a-xml-file-in-c-sharp
        public static XDocument SetAttributesWhere(this XDocument document, Func<XElement, bool> predicate, string attributeName, string newValue)
        {
            foreach (var element in document.Descendants().Where(predicate))
            {
                var attr = element.Attribute(attributeName);
                if (attr != null)
                    attr.Value = newValue;
            }
            return document;
        }

        public static bool HasAttributeWith(this XElement e, string name, string value)
            => e.Attribute(name)?.Value == value;
    }
}
