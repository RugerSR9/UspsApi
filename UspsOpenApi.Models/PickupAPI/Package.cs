using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace UspsOpenApi.Models.PickupAPI
{
	[XmlRoot(ElementName = "Package")]
	public class Package
	{
		[XmlElement(ElementName = "ServiceType")]
		public string ServiceType { get; set; }
		[XmlElement(ElementName = "Count")]
		public string Count { get; set; }
	}
}
