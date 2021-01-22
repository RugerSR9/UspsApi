using System.Collections.Generic;
using System.Xml.Serialization;

namespace UspsOpenApi.Models.RateAPI.Request
{
    [XmlType(TypeName = "Request.SpecialServices")]
    public class SpecialServices
    {
        [XmlElement(ElementName = "SpecialService")]
        public List<int> SpecialService { get; set; } = new List<int>();
    }
}
