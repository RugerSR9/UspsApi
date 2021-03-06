using System.Collections.Generic;
using System.Xml.Serialization;

namespace UspsApi.Models.AddressAPI
{
    [XmlRoot(ElementName = "ZipCodeLookupRequest")]
	public class ZipCodeLookupRequest
	{
		[XmlElement(ElementName = "Address")]
		public List<Address> Address { get; set; }
		[XmlAttribute(AttributeName = "USERID")]
		public string USERID { get; set; }
	}

}
