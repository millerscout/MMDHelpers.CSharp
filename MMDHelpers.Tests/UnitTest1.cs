using NUnit.Framework;
using MMDHelpers.CSharp.Extensions;
using MMDHelpers.CSharp;
using System;
using System.Threading.Tasks;

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
        public void BufferedTest()
        {
            var maxQty = 200;
            var perBuffer = 2;
            var bF = new BufferedProcess<object>(Convert.ToUInt32(perBuffer), 3);

            bF.onBufferReached += BF_onBufferReached;


            var task = Task.Run(() =>
            {
                for (int i = 0; i < maxQty; i++)
                {
                    var bufferTag= bF.SelectBufferReturnsIndexItem();

                    bF.bufferedList[bufferTag.currentIndexBuffer][bufferTag.CurrentItemInBufer] = i;
                }
                bF.SelectBufferReturnsIndexItem();
            });

            void BF_onBufferReached(int bufferIndex)
            {
                calls++;
                bF.CompleteBuffer(bufferIndex);
            }
            task.Wait();
            Assert.AreEqual(calls, maxQty / perBuffer);

        }
        public int calls = 0;

    }
}