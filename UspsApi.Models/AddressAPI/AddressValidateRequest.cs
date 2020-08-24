using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace UspsApi.Models.AddressAPI
{
	[XmlRoot(ElementName = "AddressValidateRequest")]
	public class AddressValidateRequest
	{
		[XmlElement(ElementName = "Revision")]
		public string Revision { get; set; }
		[XmlElement(ElementName = "Address")]
		public List<Address> Address { get; set; }
		[XmlAttribute(AttributeName = "USERID")]
		public string USERID { get; set; }
	}
}
