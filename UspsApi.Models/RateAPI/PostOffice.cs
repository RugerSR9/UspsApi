using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace UspsApi.Models.RateAPI
{
	[XmlRoot(ElementName = "PostOffice")]
	public class PostOffice
	{
		[XmlElement(ElementName = "Name")]
		public string Name { get; set; }
		[XmlElement(ElementName = "Address")]
		public string Address { get; set; }
		[XmlElement(ElementName = "City")]
		public string City { get; set; }
		[XmlElement(ElementName = "State")]
		public string State { get; set; }
		[XmlElement(ElementName = "ZipCode")]
		public string ZipCode { get; set; }
		[XmlElement(ElementName = "RetailGXGCutOffTime")]
		public string RetailGXGCutOffTime { get; set; }
		[XmlElement(ElementName = "SaturDayCutOffTime")]
		public string SaturDayCutOffTime { get; set; }
	}
}
