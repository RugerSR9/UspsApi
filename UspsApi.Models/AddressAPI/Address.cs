using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using UsaCommonModel.Mail;

namespace UspsApi.Models.AddressAPI
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
        [XmlElement(ElementName = "Zip4")]
        public string Zip4 {
            get {
                if (MailZip.Contains('-'))
                    return MailZip.Split('-')[1];
                else
                    return null;
            }
            set { }
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

        #region IAddressDetail
        public string MailName { get; set; }
        public string MailName2 { get; set; }
        public string MailAddress1 { get; set; }
        public string MailAddress2 { get; set; }
        public string MailCity { get; set; }
        public string MailState { get; set; }
        public string MailZip { get; set; }
        public string MailCityStateZip
        {
            get => MailCity + ", " + MailState + " " + MailZip;
            set => throw new NotImplementedException();
        }
        public int AddressDetailId { get; set; }
        public int? BatchId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OriginalAddress1 { get; set; }
        public string OriginalAddress2 { get; set; }
        public string OriginalCity { get; set; }
        public string OriginalState { get; set; }
        public string OriginalZip { get; set; }
        public string OriginalCityStateZip { get; set; }
        #endregion
    }
}
