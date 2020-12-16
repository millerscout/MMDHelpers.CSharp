using System;
using System.IO;
using System.Reflection;

namespace MMDHelpers.CSharp.Extensions
{
    public static class Extensions
    {
        public static string ToCurrentPath(this string path) => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);
    }
}
