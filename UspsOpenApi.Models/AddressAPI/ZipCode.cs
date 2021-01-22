using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace UspsOpenApi.Models.AddressAPI
{
	[XmlRoot(ElementName = "ZipCode")]
	public class ZipCode
	{
		[XmlElement(ElementName = "Zip5")]
		public string Zip5 { get; set; }
		[XmlElement(ElementName = "City")]
		public string City { get; set; }
		[XmlElement(ElementName = "State")]
		public string State { get; set; }
		[XmlAttribute(AttributeName = "ID")]
		public string ID { get; set; }
		[XmlElement(ElementName = "Error")]
        public Error Error { get; set; }
    }
}
