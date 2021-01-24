using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MMDHelpers.CSharp.Extensions
{
    public static class Extensions
    {
        public static string ToCurrentPath(this string path) => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);
        public static void WriteToFile(this string Path, List<string> logs)
        {
            using (FileStream file = new FileStream(Path, FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(file))
            {
                foreach (var message in logs)
                {
                    sw.WriteLine(message);
                }

            }
        }
    }
}
