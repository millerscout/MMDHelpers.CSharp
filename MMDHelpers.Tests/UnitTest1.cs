using NUnit.Framework;
using MMDHelpers.CSharp.Extensions;

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
    }
}