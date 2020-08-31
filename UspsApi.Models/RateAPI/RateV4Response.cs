using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using UspsApi.Models.RateAPI.Response;

namespace UspsApi.Models.RateAPI
{
	[XmlRoot(ElementName = "RateV4Response")]
	public class RateV4Response
	{
		[XmlElement(ElementName = "Package")]
		public List<Package> Package { get; set; }
		[XmlElement(ElementName = "Error")]
		public List<Error> Error { get; set; }
	}
}
