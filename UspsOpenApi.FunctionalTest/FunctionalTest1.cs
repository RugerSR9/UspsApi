using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using UspsOpenApi.Models.Contracts;
using UspsOpenApi.Models.RateAPI;
using UspsOpenApi.Models.RateAPI.Request;
using UspsOpenApi.ase;
using System.Collections.Generic;
using UspsOpenApi.Models.TrackingAPI;
using UspsOpenApi.Models.AddressAPI;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace UspsOpenApi.UnitTest
{
    [TestClass]
    public class FunctionalTest1
    {
        private static readonly TrackingAPI _trackingApi = new TrackingAPI();
        private static readonly AddressAPI _addressApi = new AddressAPI();

        [TestMethod]
        public async Task ValidateAddress()
        {
            Address addr = new Address()
            {
                Address1 = ***REMOVED***,
                City = ***REMOVED***,
                State = "arkansas",
                Zip5 = ***REMOVED***
            };

            addr = await _addressApi.ValidateAddress(addr);

            Assert.IsTrue(addr.OriginalAddress1 != addr.Address2);
        }

        [TestMethod]
        public async Task ZipLookup()
        {
            Address addr = new Address()
            {
                Address1 = ***REMOVED***,
                City = ***REMOVED***,
                State = "arkansas"
            };

            addr = await _addressApi.LookupZipCode(addr);

            Assert.IsTrue(!String.IsNullOrEmpty(addr.Zip5));
        }

        [TestMethod]
        public async Task CityStateLookup()
        {
            ZipCode zip = new ZipCode() { Zip5 = ***REMOVED*** };

            zip = await _addressApi.LookupCityState(zip);

            Assert.IsTrue(!String.IsNullOrEmpty(zip.City));
            Assert.IsTrue(!String.IsNullOrEmpty(zip.State));
        }

        [TestMethod]
        public async Task CityStateLookupError()
        {
            ZipCode zip = new ZipCode() { Zip5 = "7217" };

            zip = await _addressApi.LookupCityState(zip);

            Assert.IsNotNull(zip.Error);
        }

        [TestMethod]
        public async Task Track()
        {
            // not checking for anything, just not expecting a failed request
            await _trackingApi.Track("EJ123456780US");
        }

        [TestMethod]
        public async Task TrackBad()
        {
            TrackInfo doTrack = await _trackingApi.Track("9214896900855555098024");

            Assert.IsNotNull(doTrack.Error);
        }

        [TestMethod]
        public async Task TestListTracking()
        {
            // not checking for anything, just not expecting a failed request
            List<string> testList = new List<string>() { "9214896900873002520012", "9214896900873002520029", "9214896900873002520036" };
            await _trackingApi.Track(testList);
        }

        private static readonly RateAPI _rateApi = new RateAPI();

        [TestMethod]
        public async Task GetLetterRate()
        {
            Package pkg = new Package("72202", "99503")
            {
                Service = Services.FirstClass
            };

            var getRate = await _rateApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 0.45M);
        }

        [TestMethod]
        public async Task GetLetterRateMetered()
        {
            Package pkg = new Package("72202", "99503")
            {
                //ShipDate = DateTime.Now.AddDays(5).ToString()
            };

            var getRate = await _rateApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 0.4M);
        }

        [TestMethod]
        public async Task GetFlatRate()
        {
            Package pkg = new Package("72202", "99503")
            {
                Service = Services.FirstClass,
                FirstClassMailType = FirstClassMailTypes.Flat
            };

            var getRate = await _rateApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 0.5M);
        }

        [TestMethod]
        public async Task GetFlatRateMetered()
        {
            Package pkg = new Package("72202", "99503")
            {
                FirstClassMailType = FirstClassMailTypes.Flat
            };

            var getRate = await _rateApi.GetRates(pkg);

            // Flats don't have metered/commercial pricing
            // Error expected
            Assert.IsNotNull(getRate.Error);
        }

        [TestMethod]
        public async Task GetCertifiedFlatRate()
        {
            Package pkg = new Package("72202", "99503")
            {
                Service = Services.FirstClass,
                FirstClassMailType = FirstClassMailTypes.Flat
            };

            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMail);

            var getRate = await _rateApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 3M);
        }

        [TestMethod]
        public async Task GetCertifiedLetterRate()
        {
            Package pkg = new Package("72202", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMail);

            var getRate = await _rateApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 3M);
        }

        [TestMethod]
        public async Task GetCertificateOfMailingRateAsync()
        {
            Package pkg = new Package("72202", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertificateofMailingForm3817);

            var getRate = await _rateApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 3M);
        }

        [TestMethod]
        public async Task GetCertifiedERRLetterRate()
        {
            Package pkg = new Package("72202", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMail);
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.ReturnReceiptElectronic);

            var getRate = await _rateApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 4M);
        }

        [TestMethod]
        public async Task GetCertifiedRRLetterRate()
        {
            Package pkg = new Package("72202", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMail);
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.ReturnReceipt);

            var getRate = await _rateApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 5M);
        }

        [TestMethod]
        public async Task GetCertifiedRestrictedDeliveryLetterRate()
        {
            Package pkg = new Package("72202", "99503");
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMailRestrictedDelivery);

            var getRate = await _rateApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.IsTrue(getRate.Postage.First().TotalPostage > 6M);
        }
    }
}
