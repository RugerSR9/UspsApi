using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using UsaCommonModel.Mail;

namespace UspsOpenApi.Models.AddressAPI
{
    [XmlRoot(ElementName = "Address")]
    public class Address : iAddressDetail
    {
        [XmlElement(ElementName = "FirmName")]
        public string FirmName { get => MailName; set => MailName = value; }
        [XmlElement(ElementName = "Address1")]
        public string Address1 { get => MailAddress1; set => MailAddress1 = value; }
        [XmlElement(ElementName = "Address2")]
        public string Address2 { get => MailAddress2; set => MailAddress2 = value; }
        [XmlElement(ElementName = "City")]
        public string City { get => MailCity; set => MailCity = value; }
        [XmlElement(ElementName = "CityAbbreviation")]
        public string CityAbbreviation { get; set; }
        [XmlElement(ElementName = "State")]
        public string State { get => MailState; set => MailState = value; }
        [XmlElement(ElementName = "Zip5")]
        public string Zip5
        {
            get => MailZip.PadLeft(5, '0').Substring(0, 5);
            set => MailZip = value;
        }

        internal string _zip4 { get; set; } = "";
        [XmlElement(ElementName = "Zip4")]
        public string Zip4 {
            get {
                if (MailZip.Contains('-'))
                    return MailZip.Split('-')[1];
                else
                    return _zip4;
            }
            set { _zip4 = value; }
        }
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
        public string ID { get => AddressDetailId.ToString(); set => AddressDetailId = Convert.ToInt32(value); }
        [XmlElement(ElementName = "Error")]
        public Error Error { get; set; }

        #region IAddressDetail
        [XmlIgnore]
        public string MailName { get; set; } = "";
        [XmlIgnore]
        public string MailName2 { get; set; }
        [XmlIgnore]
        public string MailAddress1 { get; set; } = "";
        [XmlIgnore]
        public string MailAddress2 { get; set; } = "";
        [XmlIgnore]
        public string MailCity { get; set; } = "";
        [XmlIgnore]
        public string MailState { get; set; } = "";
        [XmlIgnore]
        public string MailZip { get; set; } = "";
        [XmlIgnore]
        public string MailCityStateZip
        {
            get => MailCity + ", " + MailState + " " + MailZip;
            set => throw new NotImplementedException();
        }
        [XmlIgnore]
        public int AddressDetailId { get; set; }
        [XmlIgnore]
        public int? BatchId { get; set; }
        [XmlIgnore]
        public string FirstName { get; set; }
        [XmlIgnore]
        public string LastName { get; set; }
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
        [XmlIgnore]
        public string OriginalCityStateZip { get; set; }
        #endregion
    }
}
