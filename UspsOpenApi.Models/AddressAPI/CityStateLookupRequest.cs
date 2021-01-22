using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace UspsOpenApi.Models.AddressAPI
{
	[XmlRoot(ElementName = "CityStateLookupRequest")]
	public class CityStateLookupRequest
	{
		[XmlElement(ElementName = "ZipCode")]
		public List<ZipCode> ZipCode { get; set; }
		[XmlAttribute(AttributeName = "USERID")]
		public string USERID { get; set; }
	}
}
