using System.IO;
using System.Xml.Serialization;

namespace CherwellSDKUtility.Models
{
    [XmlRoot("inspector")]
    public sealed class SummaryClass
    {
        #region Properties

        [XmlElement("summary")]
        public string Summary { get; set; }

        [XmlElement("param")]
        public Param[] Params { get; set; }

        [XmlElement("returns")]
        public string Returns { get; set; }

        #endregion

        #region Constructor

        public SummaryClass()
        {
            Params = null;
        }

        #endregion

        #region Public Methods

        public static SummaryClass FromXmlString(string xmlString)
        {
            var reader = new StringReader(xmlString);
            var serializer = new XmlSerializer(typeof(SummaryClass));
            var instance = (SummaryClass)serializer.Deserialize(reader);

            return instance;
        }

        public string ToXmlString()
        {
            var writer = new StringWriter();
            var serializer = new XmlSerializer(typeof(SummaryClass));
            serializer.Serialize(writer, this);
            return writer.ToString();
        }

        #endregion
    }

    public class Param
    {
        #region Properties

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlText]
        public string Value { get; set; }

        #endregion
    }
}
