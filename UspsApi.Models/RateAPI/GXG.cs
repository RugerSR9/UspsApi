using System.Xml.Serialization;

namespace UspsApi.Models.RateAPI
{
    [XmlRoot(ElementName = "GXG")]
	public class GXG
	{
		[XmlElement(ElementName = "POBoxFlag")]
		public string POBoxFlag { get; set; }
		[XmlElement(ElementName = "GiftFlag")]
		public string GiftFlag { get; set; }
	}
}
