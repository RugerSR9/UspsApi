using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace UspsOpenApi.Models.AddressAPI
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
