using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using UspsOpenApi.Models.RateAPI;

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
