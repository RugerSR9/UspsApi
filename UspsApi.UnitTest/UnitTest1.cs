using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UspsApi.Models.RateAPI;
using UspsApi.Models.RateAPI.Request;
using UspsApi.Models.TrackingAPI;
using UspsApiBase;

namespace UspsApi.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        private static TrackingAPI _trackingApi = new TrackingAPI();
        private static RateAPI _rateApi = new RateAPI();

        [TestMethod]
        public void TestMethod1()
        {
            TrackInfo doTrack = _trackingApi.Track("9214896900873001098024");

            Assert.IsTrue(doTrack.Status == "Delivered");
        }

        [TestMethod]
        public void TestListTracking()
        {
            List<string> testList = new List<string>() { "9214896900873002520012", "9214896900873002520029", "9214896900873002520036" };
            List<TrackInfo> doTrack = _trackingApi.Track(testList);

            foreach (var item in doTrack)
            {
                Console.WriteLine(item.Status);
            }
        }

        [TestMethod]
        public void GetLetterRate()
        {
            Package pkg = new Package()
            {
                ID = "0",
                ZipDestination = "99503",
                ZipOrigination = "72202",
                Pounds = 0,
                Ounces = 1,
                Container = Containers.Envelope,
                Service = Services.FirstClass,
                FirstClassMailType = FirstClassMailTypes.Letter,
                Machinable = "True"
            };

            var getRate = _rateApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.AreEqual(getRate.Postage.First().TotalPostage, 0.55M);
        }

        [TestMethod]
        public void GetLetterRateMetered()
        {
            Package pkg = new Package()
            {
                ID = "0",
                ZipDestination = "99503",
                ZipOrigination = "72202",
                Pounds = 0,
                Ounces = 1,
                Container = Containers.Envelope,
                Service = Services.FirstClassCommercial,
                FirstClassMailType = FirstClassMailTypes.Letter
            };

            var getRate = _rateApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.AreEqual(getRate.Postage.First().TotalPostage, 0.5M);
        }

        [TestMethod]
        public void GetFlatRate()
        {
            Package pkg = new Package()
            {
                ID = "0",
                ZipDestination = "99503",
                ZipOrigination = "72202",
                Pounds = 0,
                Ounces = 1,
                Container = Containers.Envelope,
                Service = Services.FirstClass,
                FirstClassMailType = FirstClassMailTypes.Flat,
                Machinable = "True"
            };

            var getRate = _rateApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.AreEqual(getRate.Postage.First().TotalPostage, 1M);
        }

        [TestMethod]
        public void GetFlatRateMetered()
        {
            Package pkg = new Package()
            {
                ID = "0",
                ZipDestination = "99503",
                ZipOrigination = "72202",
                Pounds = 0,
                Ounces = 1,
                Container = Containers.Envelope,
                Service = Services.FirstClassCommercial,
                FirstClassMailType = FirstClassMailTypes.Flat,
                Machinable = "True"
            };

            var getRate = _rateApi.GetRates(pkg);

            // Flats don't have metered/commercial pricing
            // Error expected
            Assert.IsNotNull(getRate.Error);
        }

        [TestMethod]
        public void GetCertifiedFlatRate()
        {
            Package pkg = new Package()
            {
                ID = "0",
                ZipDestination = "99503",
                ZipOrigination = "72202",
                Pounds = 0,
                Ounces = 1,
                Container = Containers.Envelope,
                Service = Services.FirstClass,
                FirstClassMailType = FirstClassMailTypes.Flat,
                Machinable = "True"
            };

            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMail);

            var getRate = _rateApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.AreEqual(getRate.Postage.First().TotalPostage, 4.55M);
        }

        [TestMethod]
        public void GetCertifiedLetterRate()
        {
            Package pkg = new Package()
            {
                ID = "0",
                ZipDestination = "99503",
                ZipOrigination = "72202",
                Pounds = 0,
                Ounces = 1,
                Container = Containers.Envelope,
                Service = Services.FirstClassCommercial,
                FirstClassMailType = FirstClassMailTypes.Letter
            };

            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMail);

            var getRate = _rateApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.AreEqual(getRate.Postage.First().TotalPostage, 4.05M);
        }

        [TestMethod]
        public void GetCertifiedERRLetterRate()
        {
            Package pkg = new Package()
            {
                ID = "0",
                ZipDestination = "99503",
                ZipOrigination = "72202",
                Pounds = 0,
                Ounces = 1,
                Container = Containers.Envelope,
                Service = Services.FirstClassCommercial,
                FirstClassMailType = FirstClassMailTypes.Letter
            };

            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMail);
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.ReturnReceiptElectronic);

            var getRate = _rateApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.AreEqual(getRate.Postage.First().TotalPostage, 5.75M);
        }

        [TestMethod]
        public void GetCertifiedRRLetterRate()
        {
            Package pkg = new Package()
            {
                ID = "0",
                ZipDestination = "99503",
                ZipOrigination = "72202",
                Pounds = 0,
                Ounces = 1,
                Container = Containers.Envelope,
                Service = Services.FirstClassCommercial,
                FirstClassMailType = FirstClassMailTypes.Letter
            };

            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMail);
            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.ReturnReceipt);

            var getRate = _rateApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.AreEqual(getRate.Postage.First().TotalPostage, 6.90M);
        }

        [TestMethod]
        public void GetCertifiedRestrictedDeliveryLetterRate()
        {
            Package pkg = new Package()
            {
                ID = "0",
                ZipDestination = "99503",
                ZipOrigination = "72202",
                Pounds = 0,
                Ounces = 1,
                Container = Containers.Envelope,
                Service = Services.FirstClassCommercial,
                FirstClassMailType = FirstClassMailTypes.Letter
            };

            pkg.SpecialServices.SpecialService.Add(SpecialServiceIds.CertifiedMailRestrictedDelivery);

            var getRate = _rateApi.GetRates(pkg);
            Console.WriteLine("Postage: $" + getRate.Postage.First().TotalPostage);
            Assert.AreEqual(getRate.Postage.First().TotalPostage, 9.50M);
        }
    }
}
