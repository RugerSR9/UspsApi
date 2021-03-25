using System.Collections.Generic;
using System.Xml.Serialization;

namespace UspsOpenApi.Models.RateAPI
{
    [XmlRoot(ElementName = "ExtraService")]
	public class ExtraService
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
	}

	[XmlRoot(ElementName = "ExtraServices")]
	public class ExtraServices
	{
		[XmlElement(ElementName = "ExtraService")]
		public List<ExtraService> ExtraService { get; set; }
	}
}
