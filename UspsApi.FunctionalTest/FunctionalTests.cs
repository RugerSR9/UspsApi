using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using UspsApi.Models.RateAPI;
using UspsApi.Models.RateAPI.Request;
using UspsApi.Models.TrackingAPI;
using UspsApi.Models.AddressAPI;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace UspsApi.UnitTest
{
    [TestClass]
    public class FunctionalTests
    {
        public UspsApi _uspsApi = new UspsApi("849JMSOF3289");

        [TestMethod]
        public void ThrowAnError()
        {
            Address addr = new()
            {
                Address1 = "101 not right",
                City = "abertackey",
                State = "pitsmouth",
                Zip5 = "12323",
                Zip4 = null
            };

            try {
                addr = _uspsApi.ValidateAddress(addr);
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void ValidateAddress()
        {
            Address addr = new()
            {
                Address1 = "500 woodlan stret",
                City = "little rock",
                State = "arkansas",
                Zip5 = "72201"
            };

            addr = _uspsApi.ValidateAddress(addr);

            Assert.IsTrue(addr.OriginalAddress1 != addr.Address2);
        }

        [TestMethod]
        public void ZipLookup()
        {
            Address addr = new()
            {
                Address1 = "500 woodlane street",
                City = "little rock",
                State = "arkansas"
            };

            addr = _uspsApi.LookupZipCode(addr);

            Assert.IsTrue(!String.IsNullOrEmpty(addr.Zip5));
        }

        [TestMethod]
        public void CityStateLookup()
        {
            ZipCode zip = new() { Zip5 = "72201" };

            zip = _uspsApi.LookupCityState(zip);

            Assert.IsTrue(!String.IsNullOrEmpty(zip.City));
            Assert.IsTrue(!String.IsNullOrEmpty(zip.State));
        }

        [TestMethod]
        public void CityStateLookupError()
        {
            ZipCode zip = new() { Zip5 = "7220" };

            zip = _uspsApi.LookupCityState(zip);

            Assert.IsNotNull(zip.Error);
        }

        [TestMethod]
        public void Track()
        {
            // not checking for anything, just not expecting a failed request
            _uspsApi.Track("EJ123456780US");
        }

        [TestMethod]
        public void TrackBad()
        {
            TrackInfo doTrack = _uspsApi.Track("1234567891234567891234");

            Assert.IsNotNull(doTrack.Error);
        }

        [TestMethod]
        public void TrackList()
        {
            List<string> testList = new List<string>() { "9374869903506981400472", "9405508205496818069977", "9200190230197647487054" };
            var results = _uspsApi.Track(testList);

            results.ForEach(o =>
            {
                Assert.IsTrue(!string.IsNullOrEmpty(o.Status));
            });
        }

        [TestMethod]
        public void GetLetterRate()
        {
            Package pkg = new("72201", "99503")
            {
                Service = Services.FirstClass
            };

            var getRate = _uspsApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 0.45M);
        }

        [TestMethod]
        public void GetLetterRateMetered()
        {
            Package pkg = new("72201", "99503")
            {
                //ShipDate = DateTime.Now.AddDays(5).ToString()
            };

            var getRate = _uspsApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 0.4M);
        }

        [TestMethod]
        public void GetFlatRate()
        {
            Package pkg = new("72201", "99503")
            {
                Service = Services.FirstClass,
                FirstClassMailType = FirstClassMailTypes.Flat
            };

            var getRate = _uspsApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 0.5M);
        }

        [TestMethod]
        public void GetFlatRateMetered()
        {
            Package pkg = new("72201", "99503")
            {
                FirstClassMailType = FirstClassMailTypes.Flat
            };

            var getRate = _uspsApi.GetRates(pkg);

            // Flats don't have metered/commercial pricing
            // Error expected
            Assert.IsNotNull(getRate.Error);
        }

        [TestMethod]
        public void GetCertifiedFlatRate()
        {
            Package pkg = new("72201", "99503")
            {
                Service = Services.FirstClass,
                FirstClassMailType = FirstClassMailTypes.Flat
            };

            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMail);

            var getRate = _uspsApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 3M);
        }

        [TestMethod]
        public void GetCertifiedLetterRate()
        {
            Package pkg = new("72201", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMail);

            var getRate = _uspsApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 3M);
        }

        [TestMethod]
        public void GetCertificateOfMailingRateAsync()
        {
            Package pkg = new("72201", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertificateofMailingForm3665);

            var getRate = _uspsApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 2.01M);
        }

        [TestMethod]
        public void GetCertifiedERRLetterRate()
        {
            Package pkg = new("72201", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMail);
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.ReturnReceiptElectronic);

            var getRate = _uspsApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 4M);
        }

        [TestMethod]
        public void GetCertifiedRRLetterRate()
        {
            Package pkg = new("72201", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMail);
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.ReturnReceipt);

            var getRate = _uspsApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 5M);
        }

        [TestMethod]
        public void GetCertifiedRestrictedDeliveryLetterRate()
        {
            Package pkg = new("72201", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMailRestrictedDelivery);

            var getRate = _uspsApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 6M);
        }
    }

    [TestClass]
    public class FunctionalTestsAsync
    {
        public UspsApi _uspsApi = new UspsApi("849JMSOF3289");

        [TestMethod]
        public async Task ValidateAddress()
        {
            string add1 = "500 woodlane street";
            Address addr = new()
            {
                Address1 = add1,
                City = "little rock",
                State = "arkansas",
                Zip5 = "72201"
            };

            addr = await _uspsApi.ValidateAddressAsync(addr);

            Assert.IsTrue(add1 != addr.Address1);
        }

        [TestMethod]
        public async Task ZipLookup()
        {
            Address addr = new()
            {
                Address1 = "500 woodlane street",
                City = "little rock",
                State = "arkansas"
            };

            addr = await _uspsApi.LookupZipCodeAsync(addr);

            Assert.IsTrue(!String.IsNullOrEmpty(addr.Zip5));
        }

        [TestMethod]
        public async Task CityStateLookup()
        {
            ZipCode zip = new() { Zip5 = "72201" };

            zip = await _uspsApi.LookupCityStateAsync(zip);

            Assert.IsTrue(!String.IsNullOrEmpty(zip.City));
            Assert.IsTrue(!String.IsNullOrEmpty(zip.State));
        }

        [TestMethod]
        public async Task CityStateLookupError()
        {
            ZipCode zip = new() { Zip5 = "7220" };

            zip = await _uspsApi.LookupCityStateAsync(zip);

            Assert.IsNotNull(zip.Error);
        }

        [TestMethod]
        public async Task Track()
        {
            // not checking for anything, just not expecting a failed request
            await _uspsApi.TrackAsync("EJ123456780US");
        }

        [TestMethod]
        public async Task TrackBad()
        {
            TrackInfo doTrack = await _uspsApi.TrackAsync("1234567891234567891234");

            Assert.IsNotNull(doTrack.Error);
        }

        [TestMethod]
        public async Task TrackList()
        {
            List<string> testList = new List<string>() { "9374869903506981400472", "9405508205496818069977", "9200190230197647487054" };
            var results = await _uspsApi.TrackAsync(testList);

            results.ForEach(o =>
            {
                Assert.IsTrue(!string.IsNullOrEmpty(o.Status));
            });
        }

        [TestMethod]
        public async Task GetLetterRate()
        {
            Package pkg = new("72201", "99503")
            {
                Service = Services.FirstClass
            };

            var getRate = await _uspsApi.GetRatesAsync(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 0.45M);
        }

        [TestMethod]
        public async Task GetLetterRateMetered()
        {
            Package pkg = new("72201", "99503")
            {
                //ShipDate = DateTime.Now.AddDays(5).ToString()
            };

            var getRate = await _uspsApi.GetRatesAsync(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 0.4M);
        }

        [TestMethod]
        public async Task GetFlatRate()
        {
            Package pkg = new("72201", "99503")
            {
                Service = Services.FirstClass,
                FirstClassMailType = FirstClassMailTypes.Flat
            };

            var getRate = await _uspsApi.GetRatesAsync(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 0.5M);
        }

        [TestMethod]
        public async Task GetFlatRateMetered()
        {
            Package pkg = new("72201", "99503")
            {
                FirstClassMailType = FirstClassMailTypes.Flat
            };

            var getRate = await _uspsApi.GetRatesAsync(pkg);

            // Flats don't have metered/commercial pricing
            // Error expected
            Assert.IsNotNull(getRate.Error);
        }

        [TestMethod]
        public async Task GetCertifiedFlatRate()
        {
            Package pkg = new("72201", "99503")
            {
                Service = Services.FirstClass,
                FirstClassMailType = FirstClassMailTypes.Flat
            };

            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMail);

            var getRate = await _uspsApi.GetRatesAsync(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 3M);
        }

        [TestMethod]
        public async Task GetCertifiedLetterRate()
        {
            Package pkg = new("72201", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMail);

            var getRate = await _uspsApi.GetRatesAsync(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 3M);
        }

        [TestMethod]
        public async Task GetCertificateOfMailingRateAsync()
        {
            Package pkg = new("72201", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertificateofMailingForm3665);

            var getRate = await _uspsApi.GetRatesAsync(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 2.01M);
        }

        [TestMethod]
        public async Task GetCertifiedERRLetterRate()
        {
            Package pkg = new("72201", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMail);
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.ReturnReceiptElectronic);

            var getRate = await _uspsApi.GetRatesAsync(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 4M);
        }

        [TestMethod]
        public async Task GetCertifiedRRLetterRate()
        {
            Package pkg = new("72201", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMail);
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.ReturnReceipt);

            var getRate = await _uspsApi.GetRatesAsync(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 5M);
        }

        [TestMethod]
        public async Task GetCertifiedRestrictedDeliveryLetterRate()
        {
            Package pkg = new("72201", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMailRestrictedDelivery);

            var getRate = await _uspsApi.GetRatesAsync(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 6M);
        }
    }
}
