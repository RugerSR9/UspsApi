using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using UspsApi.Models.RateAPI;
using UspsApi.Models.TrackingAPI;
using UspsApi.Models.AddressAPI;
using System.Threading.Tasks;
using System.Configuration;

namespace UspsApi.UnitTest
{
    [TestClass]
    public class FunctionalTest1
    {
        public UspsApi uspsApi;
        public FunctionalTest1()
        {
            // insert your usps api username to enable testing
            uspsApi = new UspsApi("849JMSOF3289");
        }

        [TestMethod]
        public void ValidateAddress()
        {
            string add1 = "500 woodlan stret";
            Address addr = new()
            {
                Address1 = add1,
                City = "litle rock",
                State = "arkansas",
                Zip5 = "72201"
            };

            addr = uspsApi.ValidateAddress(addr);

            Assert.IsTrue(add1 != addr.Address1);
        }

        [TestMethod]
        public async Task ValidateAddressAsync()
        {
            string add1 = "500 woodlane street";
            Address addr = new()
            {
                Address1 = add1,
                City = "little rock",
                State = "arkansas",
                Zip5 = "72201"
            };

            addr = await uspsApi.ValidateAddressAsync(addr);

            Assert.IsTrue(add1 != addr.Address1);
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

            addr = uspsApi.LookupZipCode(addr);

            Assert.IsTrue(!String.IsNullOrEmpty(addr.Zip5));
        }

        [TestMethod]
        public async Task ZipLookupAsync()
        {
            Address addr = new()
            {
                Address1 = "500 woodlane street",
                City = "little rock",
                State = "arkansas"
            };

            addr = await uspsApi.LookupZipCodeAsync(addr);

            Assert.IsTrue(!String.IsNullOrEmpty(addr.Zip5));
        }

        [TestMethod]
        public void CityStateLookup()
        {
            ZipCode zip = new() { Zip5 = "72201" };

            zip = uspsApi.LookupCityState(zip);

            Assert.IsTrue(!String.IsNullOrEmpty(zip.City));
            Assert.IsTrue(!String.IsNullOrEmpty(zip.State));
        }

        [TestMethod]
        public async Task CityStateLookupAsync()
        {
            ZipCode zip = new() { Zip5 = "72201" };

            zip = await uspsApi.LookupCityStateAsync(zip);

            Assert.IsTrue(!String.IsNullOrEmpty(zip.City));
            Assert.IsTrue(!String.IsNullOrEmpty(zip.State));
        }

        [TestMethod]
        public void CityStateLookupError()
        {
            ZipCode zip = new() { Zip5 = "7220" };

            zip = uspsApi.LookupCityState(zip);

            Assert.IsNotNull(zip.Error);
        }

        [TestMethod]
        public async Task CityStateLookupErrorAsync()
        {
            ZipCode zip = new() { Zip5 = "7220" };

            zip = await uspsApi.LookupCityStateAsync(zip);

            Assert.IsNotNull(zip.Error);
        }

        [TestMethod]
        public void Track()
        {
            // not checking for anything, just not expecting a failed request
            uspsApi.Track("EJ123456780US");
        }

        [TestMethod]
        public async Task TrackAsync()
        {
            // not checking for anything, just not expecting a failed request
            await uspsApi.TrackAsync("EJ123456780US");
        }

        [TestMethod]
        public void TrackBad()
        {
            TrackInfo doTrack = uspsApi.Track("1234567891234567891234");

            Assert.IsNotNull(doTrack.Error);
        }

        [TestMethod]
        public async Task TrackBadAsync()
        {
            TrackInfo doTrack = await uspsApi.TrackAsync("1234567891234567891234");

            Assert.IsNotNull(doTrack.Error);
        }

        //// to avoid leaking tracking numbers, you must supply the numbers below for this test
        //[TestMethod]
        //public void TestListTracking()
        //{
        //    // not checking for anything, just not expecting a failed request
        //    List<string> testList = new List<string>() { "1234567891234567891234", "1234567891234567891235", "1234567891234567891236" };
        //    uspsApi.Track(testList);
        //}

        //// to avoid leaking tracking numbers, you must supply the numbers below for this test
        //[TestMethod]
        //public async Task TestListTrackingAsync()
        //{
        //    // not checking for anything, just not expecting a failed request
        //    List<string> testList = new List<string>() { "1234567891234567891234", "1234567891234567891235", "1234567891234567891236" };
        //    await uspsApi.TrackAsync(testList);
        //}

        [TestMethod]
        public void GetLetterRate()
        {
            Models.RateAPI.Request.Package pkg = new("72201", "99503")
            {
                Service = Services.FirstClass
            };

            var getRate = uspsApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 0.45M);
        }

        [TestMethod]
        public async Task GetLetterRateAsync()
        {
            Models.RateAPI.Request.Package pkg = new("72201", "99503")
            {
                Service = Services.FirstClass
            };

            var getRate = await uspsApi.GetRatesAsync(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 0.45M);
        }

        [TestMethod]
        public void GetLetterRateMetered()
        {
            Models.RateAPI.Request.Package pkg = new("72201", "99503")
            {
                //ShipDate = DateTime.Now.AddDays(5).ToString()
            };

            var getRate = uspsApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 0.4M);
        }

        [TestMethod]
        public async Task GetLetterRateMeteredAsync()
        {
            Models.RateAPI.Request.Package pkg = new("72201", "99503")
            {
                //ShipDate = DateTime.Now.AddDays(5).ToString()
            };

            var getRate = await uspsApi.GetRatesAsync(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 0.4M);
        }

        [TestMethod]
        public void GetFlatRate()
        {
            Models.RateAPI.Request.Package pkg = new("72201", "99503")
            {
                Service = Services.FirstClass,
                FirstClassMailType = FirstClassMailTypes.Flat
            };

            var getRate = uspsApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 0.5M);
        }

        [TestMethod]
        public async Task GetFlatRateAsync()
        {
            Models.RateAPI.Request.Package pkg = new("72201", "99503")
            {
                Service = Services.FirstClass,
                FirstClassMailType = FirstClassMailTypes.Flat
            };

            var getRate = await uspsApi.GetRatesAsync(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 0.5M);
        }

        [TestMethod]
        public void GetFlatRateMetered()
        {
            Models.RateAPI.Request.Package pkg = new("72201", "99503")
            {
                FirstClassMailType = FirstClassMailTypes.Flat
            };

            var getRate = uspsApi.GetRates(pkg);

            // Flats don't have metered/commercial pricing
            // Error expected
            Assert.IsNotNull(getRate.Error);
        }

        [TestMethod]
        public async Task GetFlatRateMeteredAsync()
        {
            Models.RateAPI.Request.Package pkg = new("72201", "99503")
            {
                FirstClassMailType = FirstClassMailTypes.Flat
            };

            var getRate = await uspsApi.GetRatesAsync(pkg);

            // Flats don't have metered/commercial pricing
            // Error expected
            Assert.IsNotNull(getRate.Error);
        }

        [TestMethod]
        public void GetCertifiedFlatRate()
        {
            Models.RateAPI.Request.Package pkg = new("72201", "99503")
            {
                Service = Services.FirstClass,
                FirstClassMailType = FirstClassMailTypes.Flat
            };

            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMail);

            var getRate = uspsApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 3M);
        }

        [TestMethod]
        public async Task GetCertifiedFlatRateAsync()
        {
            Models.RateAPI.Request.Package pkg = new("72201", "99503")
            {
                Service = Services.FirstClass,
                FirstClassMailType = FirstClassMailTypes.Flat
            };

            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMail);

            var getRate = await uspsApi.GetRatesAsync(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 3M);
        }

        [TestMethod]
        public void GetCertifiedLetterRate()
        {
            Models.RateAPI.Request.Package pkg = new("72201", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMail);

            var getRate = uspsApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 3M);
        }

        [TestMethod]
        public async Task GetCertifiedLetterRateAsync()
        {
            Models.RateAPI.Request.Package pkg = new("72201", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMail);

            var getRate = await uspsApi.GetRatesAsync(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 3M);
        }

        [TestMethod]
        public void GetCertificateOfMailingRate()
        {
            Models.RateAPI.Request.Package pkg = new("72201", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertificateofMailingForm3665);

            var getRate = uspsApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 2.01M);
        }

        [TestMethod]
        public async Task GetCertificateOfMailingRateAsync()
        {
            Models.RateAPI.Request.Package pkg = new("72201", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertificateofMailingForm3665);

            var getRate = await uspsApi.GetRatesAsync(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 2.01M);
        }

        [TestMethod]
        public void GetCertifiedERRLetterRate()
        {
            Models.RateAPI.Request.Package pkg = new("72201", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMail);
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.ReturnReceiptElectronic);

            var getRate = uspsApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 4M);
        }

        [TestMethod]
        public async Task GetCertifiedERRLetterRateAsync()
        {
            Models.RateAPI.Request.Package pkg = new("72201", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMail);
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.ReturnReceiptElectronic);

            var getRate = await uspsApi.GetRatesAsync(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 4M);
        }

        [TestMethod]
        public void GetCertifiedRRLetterRate()
        {
            Models.RateAPI.Request.Package pkg = new("72201", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMail);
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.ReturnReceipt);

            var getRate = uspsApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 5M);
        }

        [TestMethod]
        public async Task GetCertifiedRRLetterRateAsync()
        {
            Models.RateAPI.Request.Package pkg = new("72201", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMail);
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.ReturnReceipt);

            var getRate = await uspsApi.GetRatesAsync(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 5M);
        }

        [TestMethod]
        public void GetCertifiedRestrictedDeliveryLetterRate()
        {
            Models.RateAPI.Request.Package pkg = new("72201", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMailRestrictedDelivery);

            var getRate = uspsApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 6M);
        }

        [TestMethod]
        public async Task GetCertifiedRestrictedDeliveryLetterRateAsync()
        {
            Models.RateAPI.Request.Package pkg = new("72201", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMailRestrictedDelivery);

            var getRate = await uspsApi.GetRatesAsync(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 6M);
        }
    }
}
