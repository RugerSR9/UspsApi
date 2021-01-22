using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace UspsOpenApi.Models.RateAPI
{
	[XmlRoot(ElementName = "Service")]
	public class Service
	{
		[XmlElement(ElementName = "Pounds")]
		public string Pounds { get; set; }
		[XmlElement(ElementName = "Ounces")]
		public string Ounces { get; set; }
		[XmlElement(ElementName = "Machinable")]
		public string Machinable { get; set; }
		[XmlElement(ElementName = "MailType")]
		public string MailType { get; set; }
		[XmlElement(ElementName = "Container")]
		public string Container { get; set; }
		[XmlElement(ElementName = "Width")]
		public string Width { get; set; }
		[XmlElement(ElementName = "Length")]
		public string Length { get; set; }
		[XmlElement(ElementName = "Height")]
		public string Height { get; set; }
		[XmlElement(ElementName = "Girth")]
		public string Girth { get; set; }
		[XmlElement(ElementName = "Country")]
		public string Country { get; set; }
		[XmlElement(ElementName = "Postage")]
		public string Postage { get; set; }
		[XmlElement(ElementName = "ExtraServices")]
		public ExtraServices ExtraServices { get; set; }
		[XmlElement(ElementName = "ValueOfContents")]
		public string ValueOfContents { get; set; }
		[XmlElement(ElementName = "SvcCommitments")]
		public string SvcCommitments { get; set; }
		[XmlElement(ElementName = "SvcDescription")]
		public string SvcDescription { get; set; }
		[XmlElement(ElementName = "MaxDimensions")]
		public string MaxDimensions { get; set; }
		[XmlElement(ElementName = "MaxWeight")]
		public string MaxWeight { get; set; }
		[XmlElement(ElementName = "GXGLocations")]
		public GXGLocations GXGLocations { get; set; }
		[XmlAttribute(AttributeName = "ID")]
		public string ID { get; set; }
		[XmlElement(ElementName = "GuaranteeAvailability")]
		public string GuaranteeAvailability { get; set; }
		[XmlElement(ElementName = "InsComment")]
		public string InsComment { get; set; }
	}
}
