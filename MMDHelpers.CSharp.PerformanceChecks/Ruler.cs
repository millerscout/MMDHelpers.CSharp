using System;
using System.Diagnostics;

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
        public static void Show()
        {

            Console.WriteLine($"Gen 0: {GC.CollectionCount(0) - Gen0}");
            Console.WriteLine($"Gen 1: {GC.CollectionCount(1) - Gen1}");
            Console.WriteLine($"Gen 2: {GC.CollectionCount(2) - Gen2}");

            Console.WriteLine($"Memory: {Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024} mb");


        }

    }
}
