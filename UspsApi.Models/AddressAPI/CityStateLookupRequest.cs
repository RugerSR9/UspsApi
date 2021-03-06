using System.Collections.Generic;
using System.Xml.Serialization;

namespace UspsApi.Models.AddressAPI
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
