using System.Collections.Generic;
using System.Xml.Serialization;
using UspsOpenApi.Models.RateAPI.Request;

namespace UspsOpenApi.Models.RateAPI
{
    [XmlRoot(ElementName = "RateV4Request")]
	public class RateV4Request
	{
		[XmlElement(ElementName = "Revision")]
		public string Revision { get; set; }
		[XmlElement(ElementName = "Package")]
		public List<Package> Package { get; set; }
		[XmlAttribute(AttributeName = "USERID")]
		public string USERID { get; set; }
	}
}
