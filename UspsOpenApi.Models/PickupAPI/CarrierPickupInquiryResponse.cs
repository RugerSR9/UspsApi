using System.Collections.Generic;
using System.Xml.Serialization;

namespace UspsOpenApi.Models.PickupAPI
{
    [XmlRoot(ElementName = "CarrierPickupInquiryResponse")]
	public class CarrierPickupInquiryResponse
	{
		[XmlElement(ElementName = "FirstName")]
		public string FirstName { get; set; }
		[XmlElement(ElementName = "LastName")]
		public string LastName { get; set; }
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
		[XmlElement(ElementName = "Phone")]
		public string Phone { get; set; }
		[XmlElement(ElementName = "Extension")]
		public string Extension { get; set; }
		[XmlElement(ElementName = "Package")]
		public List<Package> Package { get; set; }
		[XmlElement(ElementName = "EstimatedWeight")]
		public string EstimatedWeight { get; set; }
		[XmlElement(ElementName = "PackageLocation")]
		public string PackageLocation { get; set; }
		[XmlElement(ElementName = "SpecialInstructions")]
		public string SpecialInstructions { get; set; }
		[XmlElement(ElementName = "ConfirmationNumber")]
		public string ConfirmationNumber { get; set; }
		[XmlElement(ElementName = "DayOfWeek")]
		public string DayOfWeek { get; set; }
		[XmlElement(ElementName = "Date")]
		public string Date { get; set; }
	}
}
