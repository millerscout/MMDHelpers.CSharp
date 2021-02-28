using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace MMDHelpers.CSharp.PerformanceChecks
{
    public static class Ruler
    {
        static Stopwatch sw = null;
        static int Gen0 = 0;
        static int Gen1 = 0;
        static int Gen2 = 0;


        public static void StartMeasuring(bool ignorePastGCCollections)
        {
            if (ignorePastGCCollections)
            {
                Gen0 = GC.CollectionCount(0);
                Gen1 = GC.CollectionCount(1);
                Gen2 = GC.CollectionCount(2);
            }
            sw = Stopwatch.StartNew();

        }
        public static void StopMeasuring()
        {
            sw.Stop();
        }
        public static string Show(bool printToConsole = false)
        {


            var sb = new StringBuilder();

            sb.AppendLine($"-------------------------{DateTime.Now:yy-MM-dd HH:mm:ss}-----------------------");
            sb.AppendLine($"[MMDHelpers.P.C] Gen 0: {GC.CollectionCount(0) - Gen0}");
            sb.AppendLine($"[MMDHelpers.P.C] Gen 1: {GC.CollectionCount(1) - Gen1}");
            sb.AppendLine($"[MMDHelpers.P.C] Gen 2: {GC.CollectionCount(2) - Gen2}");

            sb.AppendLine($"[MMDHelpers.P.C] Memory: {Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024} mb");
            sb.AppendLine($"[MMDHelpers.P.C] Time: {sw.ElapsedMilliseconds} ms");

            var r = sb.ToString();
            if (printToConsole)
            {
                Console.WriteLine(r);
            }

            return r;

        }
        public static void LogToFile()
        {
            FileStream ostrm;
            StreamWriter writer;
            TextWriter oldOut = Console.Out;
            try
            {
                ostrm = new FileStream("./LogToFile.log", FileMode.Append, FileAccess.Write);
                writer = new StreamWriter(ostrm);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot open Redirect.txt for writing");
                Console.WriteLine(e.Message);
                return;
            }

            Console.SetOut(writer);
            Console.WriteLine(Show());
            Console.SetOut(oldOut);

            writer.Close();
            ostrm.Close();
        }
    }
}
