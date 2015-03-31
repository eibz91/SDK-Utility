using System;
using System.IO;
using System.Xml.Serialization;

namespace CherwellSDKUtility.Models
{
    [XmlRoot("BusinessObject")]
    public sealed class CherwellBusinessObject
    {
        #region Properties

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("RecID")]
        public string RecID { get; set; }

        [XmlElement("FieldList", Type = typeof(CherwellFieldList))]
        public CherwellFieldList FieldList { get; set; }

        #endregion

        #region Constructor

        public CherwellBusinessObject()
        {
            FieldList = null;
        }

        #endregion

        #region Public Methods

        /// <summary>Create CherwellBusinessObject from XML string - Retrieved via Cherwell API.</summary>
        /// <param name="xmlString">XML String to be deserialized.</param>
        /// <returns>CherwellBusinessObject object.</returns>
        public static CherwellBusinessObject FromXmlString(string xmlString)
        {
            var reader = new StringReader(xmlString);
            var serializer = new XmlSerializer(typeof(CherwellBusinessObject));
            var instance = (CherwellBusinessObject)serializer.Deserialize(reader);

            return instance;
        }

        /// <summary>Create XML string from CherwellBusinessObject object - To be used in Cherwell API Create/Update methods.</summary>
        /// <returns>XML String representation of CherwellBusinessObject.</returns>
        public string ToXmlString()
        {
            var writer = new StringWriter();
            var serializer = new XmlSerializer(typeof(CherwellBusinessObject));
            serializer.Serialize(writer, this);
            return writer.ToString();
        }

        #endregion
    }

    [Serializable]
    public class CherwellFieldList
    {
        #region Properties

        [XmlElement("Field", Type = typeof(CherwellField))]
        public CherwellField[] Fields { get; set; }

        #endregion

        #region Constructor

        public CherwellFieldList()
        {
            Fields = null;
        }

        #endregion
    }

    [Serializable]
    public class CherwellField
    {
        #region Properties

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlText]
        public string Value { get; set; }

        #endregion

        #region Constructor

        public CherwellField()
        {
        }

        #endregion
    }
}
