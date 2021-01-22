﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using UspsOpenApi.Models.RateAPI;

namespace UspsOpenApi.Models.AddressAPI
{
	[XmlRoot(ElementName = "AddressValidateResponse")]
	public class AddressValidateResponse
	{
		[XmlElement(ElementName = "Address")]
		public List<Address> Address { get; set; }
		[XmlElement(ElementName = "Error")]
		public Error Error { get; set; }
	}
}