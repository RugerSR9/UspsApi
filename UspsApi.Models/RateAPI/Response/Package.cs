using System.Collections.Generic;
using System.Xml.Serialization;

namespace UspsApi.Models.RateAPI.Response
{
    [XmlRoot(ElementName = "Package")]
	public class Package
	{
		[XmlAttribute(AttributeName = "ID")]
		public string ID { get; set; }
	
		[XmlElement(ElementName = "ZipOrigination")]
		public string ZipOrigination { get; set; }

		[XmlElement(ElementName = "ZipDestination")]
		public string ZipDestination { get; set; }

		[XmlElement(ElementName = "Pounds")]
		public string Pounds { get; set; }

		[XmlElement(ElementName = "Ounces")]
		public string Ounces { get; set; }

		[XmlElement(ElementName = "FirstClassMailType")]
		public string FirstClassMailType { get; set; }

		[XmlElement(ElementName = "Machinable")]
		public string Machinable { get; set; }

		[XmlElement(ElementName = "Zone")]
		public string Zone { get; set; }

		[XmlElement(ElementName = "Postage")]
		public Postage Postage { get; set; }

		[XmlElement(ElementName = "Container")]
		public string Container { get; set; }

		[XmlElement(ElementName = "Restriction")]
		public Restriction Restriction { get; set; }

		[XmlElement(ElementName = "Service")]
		public string Service { get; set; }

		[XmlElement(ElementName = "Width")]
		public string Width { get; set; }

		[XmlElement(ElementName = "Length")]
		public string Length { get; set; }

		[XmlElement(ElementName = "Height")]
		public string Height { get; set; }

		[XmlElement(ElementName = "Girth")]
		public string Girth { get; set; }

		[XmlElement(ElementName = "Error")]
        public Error Error { get; set; }

		[XmlElement(ElementName = "MailType")]
		public string MailType { get; set; }

		[XmlElement(ElementName = "GXG")]
		public GXG GXG { get; set; }

		[XmlElement(ElementName = "ValueOfContents")]
		public string ValueOfContents { get; set; }

		[XmlElement(ElementName = "Country")]
		public string Country { get; set; }

		[XmlElement(ElementName = "Size")]
		public string Size { get; set; }

		[XmlElement(ElementName = "OriginZip")]
		public string OriginZip { get; set; }

		[XmlElement(ElementName = "CommercialFlag")]
		public string CommercialFlag { get; set; }

		[XmlElement(ElementName = "AcceptanceDateTime")]
		public string AcceptanceDateTime { get; set; }

		[XmlElement(ElementName = "DestinationPostalCode")]
		public string DestinationPostalCode { get; set; }

		[XmlElement(ElementName = "ExtraServices")]
		public ExtraService ExtraServices { get; set; }

		//[XmlElement(ElementName = "SpecialServices")]
		//public List<SpecialService> SpecialServices { get; set; }

		[XmlElement(ElementName = "Prohibitions")]
		public string Prohibitions { get; set; }

		[XmlElement(ElementName = "Restrictions")]
		public string Restrictions { get; set; }

		[XmlElement(ElementName = "Observations")]
		public string Observations { get; set; }

		[XmlElement(ElementName = "CustomsForms")]
		public string CustomsForms { get; set; }

		[XmlElement(ElementName = "ExpressMail")]
		public string ExpressMail { get; set; }

		[XmlElement(ElementName = "AreasServed")]
		public string AreasServed { get; set; }

		[XmlElement(ElementName = "AdditionalRestrictions")]
		public string AdditionalRestrictions { get; set; }
	}
}
