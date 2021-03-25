using System.Collections.Generic;
using System.Xml.Serialization;
using UspsOpenApi.Models.RateAPI.Response;

namespace UspsOpenApi.Models.RateAPI
{
    [XmlRoot(ElementName = "IntlRateV2Response")]
	public class IntlRateV2Response
	{
		[XmlElement(ElementName = "Package")]
		public List<Package> Package { get; set; }
		[XmlElement(ElementName = "Error")]
		public List<Error> Error { get; set; }
	}
}
