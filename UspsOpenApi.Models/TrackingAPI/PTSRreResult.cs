using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace UspsOpenApi.Models.TrackingAPI
{
	[XmlRoot(ElementName = "PTSRRERESULT")]
	public class PTSRRERESULT
	{
		[XmlElement(ElementName = "ResultText")]
		public string ResultText { get; set; }
		[XmlElement(ElementName = "ReturnCode")]
		public string ReturnCode { get; set; }
	}
}
