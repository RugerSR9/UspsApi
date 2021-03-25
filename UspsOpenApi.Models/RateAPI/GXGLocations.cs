﻿using System.Xml.Serialization;

namespace UspsOpenApi.Models.RateAPI
{
    [XmlRoot(ElementName = "GXGLocations")]
	public class GXGLocations
	{
		[XmlElement(ElementName = "PostOffice")]
		public PostOffice PostOffice { get; set; }
	}
}
