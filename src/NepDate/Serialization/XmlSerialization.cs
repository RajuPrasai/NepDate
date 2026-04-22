using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NepDate.Serialization
{
    /// <summary>
    /// Provides XML serialization support for the <see cref="NepaliDate"/> struct,
    /// allowing it to be serialized and deserialized with XML serializers.
    /// </summary>
    public class NepaliDateXmlSerializer : IXmlSerializable
    {
        private NepaliDate _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="NepaliDateXmlSerializer"/> class.
        /// </summary>
        public NepaliDateXmlSerializer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NepaliDateXmlSerializer"/> class with the specified Nepali date.
        /// </summary>
        /// <param name="value">The Nepali date to serialize.</param>
        public NepaliDateXmlSerializer(NepaliDate value)
        {
            _value = value;
        }

        /// <summary>
        /// Gets the <see cref="NepaliDate"/> value.
        /// </summary>
        public NepaliDate Value => _value;

        /// <summary>
        /// This method is reserved and should not be used.
        /// </summary>
        /// <returns>Always returns null.</returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> stream from which the object is deserialized.</param>
        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            if (reader.IsEmptyElement)
            {
                reader.Read();
                _value = default;
                return;
            }

            reader.ReadStartElement();
            string dateValue = reader.ReadContentAsString();

            if (string.IsNullOrEmpty(dateValue))
            {
                _value = default;
            }
            else if (NepaliDate.TryParse(dateValue, out var date))
            {
                _value = date;
            }
            else
            {
                throw new InvalidOperationException($"Could not convert '{dateValue}' to a valid NepaliDate");
            }

            reader.ReadEndElement();
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> stream to which the object is serialized.</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteString($"{_value.Year:D4}-{_value.Month:D2}-{_value.Day:D2}");
        }
    }
}