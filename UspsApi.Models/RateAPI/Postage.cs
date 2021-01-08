using System.Xml.Serialization;

namespace UspsApi.Models.RateAPI
{
    [XmlRoot(ElementName = "Postage")]
	public class Postage
	{
		[XmlAttribute(AttributeName = "CLASSID")]
		public string CLASSID { get; set; }

		[XmlElement(ElementName = "MailService")]
		public string MailService { get; set; }

		[XmlElement(ElementName = "Rate")]
		public string Rate { get; set; }

		[XmlElement(ElementName = "TotalPostage")]
		public decimal TotalPostage { get; set; }

		[XmlElement(ElementName = "SpecialServices")]
		public UspsApi.Models.RateAPI.Response.SpecialServices SpecialServices { get; set; }

		[XmlElement(ElementName = "CommitmentDate")]
		public string CommitmentDate { get; set; }

		[XmlElement(ElementName = "CommitmentName")]
		public string CommitmentName { get; set; }
	}

}
