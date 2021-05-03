using System;
using WebSocketSharp;
using System.Threading;
using SGPdotNET.CoordinateSystem;
using SGPdotNET.Observation;
using SGPdotNET.TLE;
using SGPdotNET.Util;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;namespace Ground
{
  public class Program
  {
    public static void Main (string[] args)
    {
      var satellites = new List<Satellite>();
        try
        {
            // Open the text file using a stream reader.

                var file = new StreamReader("OrbitalPositionTest/data/all.tle").ReadToEnd(); // big string
                var lines = file.Split(new char[] {'\n'});           // big array
                var count = lines.Length;
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
        Console.WriteLine(satellites.Count);
        using (var ws = new WebSocket ("ws://localhost:1234/Commands")) {
          ws.OnMessage += (sender, e) => {
            Console.WriteLine ("Commands says: " + e.Data);
            if(e.Data.StartsWith("READY")){
              ws.Send($"COMMAND*{args[1]}*LES-5,CALSPHERE 2*1,0,0");//accel x
            }else if(e.Data.StartsWith($"SECOND*{args[1]}")){

            }else if(e.Data.StartsWith("SECOND")){
              //find collisions small list 
              if(true){//if trusted
                var convergence_names = e.Data.Split("*")[2].Split(",");
                var convergences = OrbitalCalculator.Services.Collisions.FindCollisions(satellites.Where(x=>convergence_names.ToList().Contains(x.Name.Trim())).ToList(),satellites.Find(x=>x.Name.Trim()==e.Data.Split("*")[3]),2,60);
                Console.WriteLine(convergences.Count);
                foreach (var item in convergences)
                {
                    Console.WriteLine($"{item.satilite_name}U{e.Data.Split("*")[3]}@{item.time}");
                }
                if(convergences.Count!=0){
                  ws.Send($"SECONDRESPONSE*{args[1]}*TRUE");
                }else{
                  ws.Send($"SECONDRESPONSE*{args[1]}*FALSE");
                }
              }
            }
          };
          ws.Connect ();
          Console.WriteLine(String.Join(",",args));
          if(args[0]=="0"){
            ws.Send ($"GROUNDCONNECTED*{args[1]}");
          }
          while(true){
            Thread.Sleep(25);
          }
        }
    }
  }
}