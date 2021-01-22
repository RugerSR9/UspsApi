﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace UspsOpenApi.Models.AddressAPI
{
	[XmlRoot(ElementName = "ZipCodeLookupResponse")]
	public class ZipCodeLookupResponse
	{
		[XmlElement(ElementName = "Address")]
		public List<Address> Address { get; set; }
	}
}