using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace UspsApi.Models.RateAPI.Request
{
    // NOTE: Due to the poor nature of the USPS API, these need to be in a specific order for them to be parsed properly.
    // There may still be undiscovered cases where these properties are not in the correct sequence order.
    [XmlRoot(ElementName = "Package")]
    public class Package
    {
        public Package() {}

        /// <summary>
        /// Quick construct a 1 ounce First Class Envelope Letter based on orig and dest zip.
        /// </summary>
        /// <param name="originationZip"></param>
        /// <param name="destinationZip"></param>
        public Package(string originationZip, string destinationZip, int id = -1)
        {
            ID = (id == -1) ? "0" : id.ToString();
            ZipOrigination = originationZip;
            ZipDestination = destinationZip;
            Pounds = 0;
            Ounces = 1;
            FirstClassMailType = FirstClassMailTypes.Letter;
            Container = Containers.Envelope;
            Service = Services.FirstClassCommercial;
            Machinable = "True";
        }

        [XmlElement(ElementName = "Service")]
        public string Service { get; set; }

        [XmlElement(ElementName = "FirstClassMailType")]
        public string FirstClassMailType { get; set; }

        [Required]
        [XmlElement(ElementName = "ZipOrigination")]
        [RegularExpression("/\\d{5}/", ErrorMessage = "Zip code in 'ZipOrigination' did not match the expected format.")]
        public string ZipOrigination { get; set; }

        [Required]
        [XmlElement(ElementName = "ZipDestination")]
        [RegularExpression("/\\d{5}/", ErrorMessage = "Zip code in 'ZipDestination' did not match the expected format.")]
        public string ZipDestination { get; set; }


        [XmlElement(ElementName = "Pounds")]
        [Range(0, 70)]
        public decimal Pounds { get; set; }

        [XmlElement(ElementName = "Ounces")]
        [Range(0, 1120)]
        public decimal Ounces { get; set; }

        [XmlElement(ElementName = "Container")]
        public string Container { get; set; }


        [XmlElement(ElementName = "Zone")]
        public string Zone { get; set; }

        [XmlElement(ElementName = "Postage")]
        public List<Postage> Postage { get; set; }

        [XmlAttribute(AttributeName = "ID")]
        public string ID { get; set; }

        [XmlElement(ElementName = "Restriction")]
        public Restriction Restriction { get; set; }

        [XmlElement(ElementName = "Width")]
        public string Width { get; set; }

        [XmlElement(ElementName = "Length")]
        public string Length { get; set; }

        [XmlElement(ElementName = "Height")]
        public string Height { get; set; }

        [XmlElement(ElementName = "Girth")]
        public string Girth { get; set; }

        [XmlElement(ElementName = "Error")]
        public Error Error { get; set; }

        [XmlElement(ElementName = "MailType")]
        public string MailType { get; set; }

        [XmlElement(ElementName = "GXG")]
        public GXG GXG { get; set; }

        [XmlElement(ElementName = "ValueOfContents")]
        public string ValueOfContents { get; set; }

        [XmlElement(ElementName = "Country")]
        public string Country { get; set; }

        [XmlElement(ElementName = "Size")]
        public string Size { get; set; }

        [XmlElement(ElementName = "OriginZip")]
        public string OriginZip { get; set; }

        [XmlElement(ElementName = "CommercialFlag")]
        public string CommercialFlag { get; set; }

        [XmlElement(ElementName = "AcceptanceDateTime")]
        public string AcceptanceDateTime { get; set; }

        [XmlElement(ElementName = "DestinationPostalCode")]
        public string DestinationPostalCode { get; set; }

        [XmlElement(ElementName = "ExtraServices")]
        public ExtraService ExtraServices { get; set; }

        [XmlElement(ElementName = "SpecialServices")]
        public SpecialServices SpecialServices { get; set; } = new SpecialServices();

        public bool ShouldSerializeSpecialServices()
        {
            return SpecialServices.SpecialService.Count > 0;
        }

        [XmlElement(ElementName = "Prohibitions")]
        public string Prohibitions { get; set; }

        [XmlElement(ElementName = "Restrictions")]
        public string Restrictions { get; set; }

        [XmlElement(ElementName = "Observations")]
        public string Observations { get; set; }

        [XmlElement(ElementName = "CustomsForms")]
        public string CustomsForms { get; set; }

        [XmlElement(ElementName = "ExpressMail")]
        public string ExpressMail { get; set; }

        [XmlElement(ElementName = "AreasServed")]
        public string AreasServed { get; set; }

        [XmlElement(ElementName = "AdditionalRestrictions")]
        public string AdditionalRestrictions { get; set; }

        [XmlIgnore]
        [XmlElement(ElementName = "GroundOnly")]
        public bool GroundOnly { get; set; }

        [XmlElement(ElementName = "SortBy")]
        public string SortBy { get; set; }

        [XmlElement(ElementName = "Machinable")]
        public string Machinable { get; set; }

        [XmlIgnore]
        [XmlElement(ElementName = "ReturnLocations")]
        public bool ReturnLocations { get; set; }

        [XmlElement(ElementName = "ShipDate")]
        public string ShipDate { get => DateTime.Parse(shipdate).ToString("yyyy-MM-dd"); set => shipdate = value; }
        private string shipdate = DateTime.Now.ToString();
    }
}
