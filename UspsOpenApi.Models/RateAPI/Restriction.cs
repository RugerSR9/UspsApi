﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace UspsOpenApi.Models.RateAPI
{
	[XmlRoot(ElementName = "Restriction")]
	public class Restriction
	{
		[XmlElement(ElementName = "Restrictions")]
		public string Restrictions { get; set; }
	}
}