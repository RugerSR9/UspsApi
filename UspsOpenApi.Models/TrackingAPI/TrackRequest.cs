using System.Collections.Generic;
using System.Xml.Serialization;

namespace UspsOpenApi.Models.TrackingAPI
{
    [XmlRoot(ElementName = "TrackRequest")]
	public class TrackRequest
	{
		[XmlElement(ElementName = "TrackID")]
		public List<TrackID> TrackID { get; set; }
		[XmlAttribute(AttributeName = "USERID")]
		public string USERID { get; set; }
	}
}
