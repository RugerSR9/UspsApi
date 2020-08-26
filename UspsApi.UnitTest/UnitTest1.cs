using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using UspsApi.Models.TrackingAPI;
using UspsApiBase;

namespace UspsApi.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        private static TrackingAPI _trackingApi = new TrackingAPI();

        [TestMethod]
        public void TestMethod1()
        {
            TrackID trackThis = new TrackID()
            {
                ID = "9214896900873001098024"
            };

            TrackInfo doTrack = _trackingApi.Track(trackThis);

            Assert.IsTrue(doTrack.Status == "Delivered");
        }

        [TestMethod]
        public async Task TestListTracking()
        {
            List<TrackID> testList = new List<TrackID>();
            TrackID trackThis = new TrackID();
            trackThis.ID = "9214896900873001098024";
            testList.Add(trackThis);
            trackThis = new TrackID();
            trackThis.ID = "9207190439100127720086";
            testList.Add(trackThis);

            List<TrackInfo> doTrack = await _trackingApi.Track(testList);
        }
    }
}
