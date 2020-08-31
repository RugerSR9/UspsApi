using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace UspsApi.Models.RateAPI.Request
{
	[XmlRoot(ElementName = "SpecialService")]
	public class SpecialService
	{
		[XmlElement(ElementName = "ServiceID")]
		public string ServiceID { get; set; }
		[XmlElement(ElementName = "ServiceName")]
		public string ServiceName { get; set; }
		[XmlElement(ElementName = "Available")]
		public string Available { get; set; }
		[XmlElement(ElementName = "Price")]
		public string Price { get; set; }
		[XmlElement(ElementName = "DeclaredValueRequired")]
		public string DeclaredValueRequired { get; set; }
		[XmlElement(ElementName = "DueSenderRequired")]
		public string DueSenderRequired { get; set; }
	}

	[XmlRoot(ElementName = "SpecialServices")]
	public class SpecialServices
	{
		[XmlElement(ElementName = "SpecialService")]
		public List<SpecialService> SpecialService { get; set; }
	}
}
