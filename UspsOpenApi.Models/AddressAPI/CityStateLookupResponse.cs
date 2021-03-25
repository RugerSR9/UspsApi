using System.Collections.Generic;
using System.Xml.Serialization;

namespace UspsOpenApi.Models.AddressAPI
{
    [XmlRoot(ElementName = "CityStateLookupResponse")]
	public class CityStateLookupResponse
	{
		[XmlElement(ElementName = "ZipCode")]
		public List<ZipCode> ZipCode { get; set; }
		[XmlElement(ElementName = "Error")]
		public List<Error> Error { get; set; }
	}
}
