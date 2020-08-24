using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace UspsApi.Models.TrackingAPI
{
	[XmlRoot(ElementName = "PTSPODRESULT")]
	public class PTSPODRESULT
	{
		[XmlElement(ElementName = "ResultText")]
		public string ResultText { get; set; }
		[XmlElement(ElementName = "ReturnCode")]
		public string ReturnCode { get; set; }
	}

}
