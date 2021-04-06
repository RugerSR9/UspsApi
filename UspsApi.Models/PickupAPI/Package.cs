using System.Xml.Serialization;

namespace UspsApi.Models.PickupAPI
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
