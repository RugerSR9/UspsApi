using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace UspsApi.Models.TrackingAPI
{
	[XmlRoot(ElementName = "TrackFieldRequest")]
	public class TrackFieldRequest
	{
		[XmlAttribute(AttributeName = "USERID")]
		public string USERID { get; set; }
		[XmlElement(ElementName = "Revision")]
		public string Revision { get; set; }
		[XmlElement(ElementName = "ClientIp")]
		public string ClientIp { get; set; }
		[XmlElement(ElementName = "SourceId")]
		public string SourceId { get; set; }
		[XmlElement(ElementName = "TrackID")]
		public List<TrackID> TrackID { get; set; } // todo: unsure if this can take a list
	}
}
