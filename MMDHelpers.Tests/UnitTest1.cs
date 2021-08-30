using NUnit.Framework;
using MMDHelpers.CSharp.Extensions;
using MMDHelpers.CSharp;
using System;

namespace MMDHelpers.Test
{
    public class Tests
    {
        [TestCase(@"C:\project\MMDHelpers.CSharp\MMDHelpers.CSharp.Performance.Grpc\bin\Release\MMDHelpers.CSharp.Performance.Grpc.1.0.1.nupkg", ".nupkg", true)]
        [TestCase(@"C:\project\MMDHelpers.CSharp\MMDHelpers.CSharp.Performance.Grpc\bin\Release\MMDHelpers.CSharp.Performance.Grpc.1.0.1.nupkg", ".nupkG", false)]
        [TestCase(@"C:\project\MMDHelpers.CSharp\MMDHelpers.CSharp.Performance.Grpc\bin\Release\MMDHelpers.CSharp.Performance.Grpc.1.0.1.nupkg", ".Nupkg", false)]
        [TestCase(@"C:\project\MMDHelpers.CSharp\MMDHelpers.CSharp.Performance.Grpc\bin\Release\MMDHelpers.CSharp.Performance.Grpc.1.0.1.upkg", ".nupkg", false)]
        public void Test1(string text, string pattern, bool result)
        {
            Assert.AreEqual(result, text.EndsWithCaseSensitive(pattern));
        }

        [TestCase]
        public void BufferedTet()
        {
            var maxQty = 200;
            var perBuffer = 10;
            var bF = new BufferedProcess<object>(Convert.ToUInt32(perBuffer), 2);

            bF.onBufferReached += BF_onBufferReached;

            for (int i = 0; i < maxQty; i++)
            {
                bF.SelectBufferReturnsIndexItem();

                bF.bufferedList[bF.currentIndexBuffer][bF.CurrentItemInBufer] = i;
            }
            bF.SelectBufferReturnsIndexItem();
            Assert.AreEqual(calls, maxQty / perBuffer);

        }
        public int calls = 0;
        private void BF_onBufferReached(int bufferIndex)
        {
            calls++;
        }
    }
}