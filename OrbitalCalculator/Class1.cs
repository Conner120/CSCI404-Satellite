using System;
using System.Collections.Generic;
using SGPdotNET.CoordinateSystem;
using SGPdotNET.Observation;
using SGPdotNET.TLE;
using SGPdotNET.Util;
using MPI;
using System.Threading;  
using System.Threading.Tasks;
namespace OrbitalCalculator.Services
{
    public static class Collisions
    {
        public static List<Intersects> FindCollisions(List<Satellite> bodies,Satellite your_satilites, double max_look_ahead,int scale=120){
            List<Intersects> test = new List<Intersects>(); 
            DateTime[] times = new DateTime[(int)max_look_ahead*scale];
            List<EciCoordinate> target_positions = new List<EciCoordinate>();
            for (int i = 0; i < times.Length; i++)
            {
                times[i] = DateTime.UtcNow;
                times[i] = times[i].AddSeconds((int)(i*(3600/scale)));
                target_positions.Add(your_satilites.Predict(times[i]));
            }
            Console.WriteLine($"Samples: {times.Length} ∆t: {times[2].Subtract(times[1])}");
            List<Thread> ts = new List<Thread>();
            foreach (var item in bodies)
            {
                Thread t = new Thread(()=>{
                    test.AddRange(t_find(target_positions,target_positions.Count,times,item,10));
                });  
                ts.Add(t);
                t.Start(); 

            }
            foreach (Thread item in ts)
            {
                item.Join();
            }
            return test;
        }
        public static List<Intersects> t_find(List<EciCoordinate> positions,int location_count, DateTime[] times, Satellite target,double range){
            List<Intersects> intercepts = new List<Intersects>();
            for (int i = 0; i < times.Length; i++)
            {
                try
                {
                    EciCoordinate target_cords = target.Predict(times[i]);
                    double[] diffs = {positions[i].Position.X-target_cords.Position.X,positions[i].Position.Y-target_cords.Position.Y,positions[i].Position.Z-target_cords.Position.Z};
                    diffs[0]=Math.Abs(diffs[0]);
                    diffs[1]=Math.Abs(diffs[1]);
                    diffs[2]=Math.Abs(diffs[2]);
                    if((diffs[0]<range)&(diffs[1]<range)&(diffs[2]<range)){
                        intercepts.Add(new Intersects(target.Name,times[i]));
                    }
                }
                catch (System.Exception)
                {
                    Console.WriteLine($"error with: {target.Name}");
                    return intercepts;
                }
            }
            return intercepts;
        }
    }
    public class Intersects{
        public string satilite_name = "test";
        public DateTime time = new DateTime();
        public Intersects(string name, DateTime t){
            satilite_name=name;
            time=t;
        }
    }

}
