using System;
using System.Xml.Serialization;

namespace UspsApi.Models.AddressAPI
{
    [XmlRoot(ElementName = "Address")]
    public class Address
    {
        [XmlIgnore]
        public int AddressDetailId { get; set; }
        [XmlIgnore]
        public string OriginalAddress1 { get; set; }
        [XmlIgnore]
        public string OriginalAddress2 { get; set; }
        [XmlIgnore]
        public string OriginalCity { get; set; }
        [XmlIgnore]
        public string OriginalState { get; set; }
        [XmlIgnore]
        public string OriginalZip { get; set; }

        [XmlElement(ElementName = "FirmName")]
        public string FirmName { get; set; } = "";
        [XmlElement(ElementName = "Address1")]
        public string Address1 { get; set; } = "";
        [XmlElement(ElementName = "Address2")]
        public string Address2 { get; set; } = "";
        [XmlElement(ElementName = "City")]
        public string City { get; set; } = "";
        [XmlElement(ElementName = "CityAbbreviation")]
        public string CityAbbreviation { get; set; }
        [XmlElement(ElementName = "State")]
        public string State { get; set; } = "";
        [XmlElement(ElementName = "Zip5")]
        public string Zip5 { get; set; } = "";
        [XmlElement(ElementName = "Zip4")]
        public string Zip4 { get; set; } = "";
        [XmlElement(ElementName = "DeliveryPoint")]
        public string DeliveryPoint { get; set; }
        [XmlElement(ElementName = "CarrierRoute")]
        public string CarrierRoute { get; set; }
        [XmlElement(ElementName = "Footnotes")]
        public string Footnotes { get; set; }
        [XmlElement(ElementName = "DPVConfirmation")]
        public string DPVConfirmation { get; set; }
        [XmlElement(ElementName = "DPVCMRA")]
        public string DPVCMRA { get; set; }
        [XmlElement(ElementName = "DPVFootnotes")]
        public string DPVFootnotes { get; set; }
        [XmlElement(ElementName = "Business")]
        public string Business { get; set; }
        [XmlElement(ElementName = "CentralDeliveryPoint")]
        public string CentralDeliveryPoint { get; set; }
        [XmlElement(ElementName = "Vacant")]
        public string Vacant { get; set; }
        [XmlAttribute(AttributeName = "ID")]
        public string ID { get; set; } = DateTime.Now.Ticks.ToString();
        [XmlElement(ElementName = "Error")]
        public Error Error { get; set; }
    }
}
