﻿using System.Xml.Serialization;

namespace UspsOpenApi.Models.TrackingAPI
{
    [XmlRoot(ElementName = "TrackID")]
	public class TrackID
	{
		[XmlAttribute(AttributeName = "ID")]
		public string ID { get; set; }
	}
}
