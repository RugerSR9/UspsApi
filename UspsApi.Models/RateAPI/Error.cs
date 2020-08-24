﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace UspsApi.Models.RateAPI
{
	[XmlRoot(ElementName = "Error")]
	public class Error
	{
		[XmlElement(ElementName = "Number")]
		public string Number { get; set; }
		[XmlElement(ElementName = "Source")]
		public string Source { get; set; }
		[XmlElement(ElementName = "Description")]
		public string Description { get; set; }
		[XmlElement(ElementName = "HelpFile")]
		public string HelpFile { get; set; }
		[XmlElement(ElementName = "HelpContext")]
		public string HelpContext { get; set; }
	}

}
