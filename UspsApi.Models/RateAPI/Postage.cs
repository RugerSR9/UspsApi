using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace UspsApi.Models.RateAPI
{
	[XmlRoot(ElementName = "Postage")]
	public class Postage
	{
		[XmlElement(ElementName = "MailService")]
		public string MailService { get; set; }
		[XmlElement(ElementName = "Rate")]
		public string Rate { get; set; }
		[XmlElement(ElementName = "SpecialServices")]
		public SpecialServices SpecialServices { get; set; }
		[XmlAttribute(AttributeName = "CLASSID")]
		public string CLASSID { get; set; }
		[XmlElement(ElementName = "CommitmentDate")]
		public string CommitmentDate { get; set; }
		[XmlElement(ElementName = "CommitmentName")]
		public string CommitmentName { get; set; }
	}

}
