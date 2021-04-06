using System.Xml.Serialization;

namespace UspsApi.Models.PickupAPI
{
    [XmlRoot(ElementName = "CarrierPickupInquiryRequest")]
	public class CarrierPickupInquiryRequest
	{
		[XmlElement(ElementName = "FirmName")]
		public string FirmName { get; set; }
		[XmlElement(ElementName = "SuiteOrApt")]
		public string SuiteOrApt { get; set; }
		[XmlElement(ElementName = "Address2")]
		public string Address2 { get; set; }
		[XmlElement(ElementName = "Urbanization")]
		public string Urbanization { get; set; }
		[XmlElement(ElementName = "City")]
		public string City { get; set; }
		[XmlElement(ElementName = "State")]
		public string State { get; set; }
		[XmlElement(ElementName = "ZIP5")]
		public string ZIP5 { get; set; }
		[XmlElement(ElementName = "ZIP4")]
		public string ZIP4 { get; set; }
		[XmlElement(ElementName = "ConfirmationNumber")]
		public string ConfirmationNumber { get; set; }
		[XmlAttribute(AttributeName = "USERID")]
		public string USERID { get; set; }
	}
}
