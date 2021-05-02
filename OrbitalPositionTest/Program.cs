using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SGPdotNET.CoordinateSystem;
using SGPdotNET.Observation;
using SGPdotNET.TLE;
using SGPdotNET.Util;
using System.IO;
class Program
{
    static void Main(string[] args)
    {
        int row = 0;
        var watch = new System.Diagnostics.Stopwatch();
        var satellites = new List<Satellite>();
        try
        {
            // Open the text file using a stream reader.

                var file = new StreamReader("OrbitalPositionTest/data/all.tle").ReadToEnd(); // big string
                var lines = file.Split(new char[] {'\n'});           // big array
                var count = lines.Count();
                for (int i = 0; i < count; i+=3)
                {
                    var l1 = lines[i];
                    var l2 = lines[i+1];
                    var l3 = lines[i+2];
                    satellites.Add(new Satellite(l1,l2,l3));
                }
                // Read the stream as a string, and write the string to the console.
        }
        catch (IOException e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }
            if (row == 0 || row >= 25)
                ResetConsole();
            watch.Reset();
            // string input = Console.ReadLine();
            var tle1 = "INTEGRAL";
            var tle2 = "1 27540U 02048A   21109.41033731  .00000165  00000-0  00000+0 0  9998";
            var tle3 = "2 27540  62.5459  89.0166 8994899 298.7327   1.6996  0.37571334 19548";
            watch.Start();
            // Create a satellite from the TLEs
            var sat = new Satellite(tle1, tle2, tle3);
            // Set up our ground station location
            // Create a ground station
            // Observe the satellite     
            var collions = OrbitalCalculator.Services.Collisions.FindCollisions(satellites,sat,2,60);
            watch.Stop();
            foreach (var item in collions)
            {
                Console.WriteLine($"{item.satilite_name}U{sat.Name}@{item.time}");
            }
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms With: {satellites.Count}");
            row += 1;
        return;

        // Declare a ResetConsole local method
        void ResetConsole()
        {
            // if (row > 0)
            // {
            //     Console.WriteLine("Press any key to continue...");
            //     Console.ReadKey();
            // }
            // Console.Clear();
            // Console.WriteLine($"{Environment.NewLine}Press <Enter> only to exit; otherwise, enter a string and press <Enter>:{Environment.NewLine}");
            // row = 3;
        }
    }
}