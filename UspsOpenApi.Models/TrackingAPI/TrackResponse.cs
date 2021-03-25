using System.Collections.Generic;
using System.Xml.Serialization;

namespace UspsOpenApi.Models.TrackingAPI
{
    [XmlRoot(ElementName = "TrackResponse")]
	public class TrackResponse
	{
		[XmlElement(ElementName = "TrackInfo")]
		public List<TrackInfo> TrackInfo { get; set; }
	}
}
