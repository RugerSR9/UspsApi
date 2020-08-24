using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace UspsApi.Models.PickupAPI
{
	[XmlRoot(ElementName = "CarrierPickupAvailabilityResponse")]
	public class CarrierPickupAvailabilityResponse
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
		[XmlElement(ElementName = "DayOfWeek")]
		public string DayOfWeek { get; set; }
		[XmlElement(ElementName = "Date")]
		public string Date { get; set; }
		[XmlElement(ElementName = "CarrierRoute")]
		public string CarrierRoute { get; set; }
	}
}
