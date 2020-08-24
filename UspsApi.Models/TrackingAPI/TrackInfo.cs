using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace UspsApi.Models.TrackingAPI
{
	[XmlRoot(ElementName = "TrackInfo")]
	public class TrackInfo
	{
		[XmlElement(ElementName = "TrackSummary")]
		public TrackSummary TrackSummary { get; set; }
		[XmlElement(ElementName = "TrackDetail")]
		public List<TrackDetail> TrackDetail { get; set; }
		[XmlAttribute(AttributeName = "ID")]
		public string ID { get; set; }
	}
}
