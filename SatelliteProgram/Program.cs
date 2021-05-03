using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;
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
namespace Satellite
{
  public class Commands : WebSocketBehavior
  {
    public String initial_grnd;
    public String satellite_name = "CALSPHERE 1";
    public TrustModel trust_model = new TrustModel();      
    protected override void OnMessage (MessageEventArgs e)
    {
        if (e.Data.StartsWith("COMMAND"))
        {
          initial_grnd = e.Data.Split("*")[1];
          if (trust_model.GetTrust(initial_grnd) > 0.5) { // if (trusted)
            Send("Running CMD");
            //output orbital change request
          } else { 
            // waiting for second confirmation or one time pass 
            if (e.Data.Split("*").Length>4) {
              // process pad overide if pad fail maybe degnate trust
              // file path may require tweaking to reach OneTimePad/Message.txt.
              byte[] originalBytes = Encoding.Unicode.GetBytes("../../../../OneTimePad/Message.txt");
              // generate the pad
              byte[] pad = OneTimePad.Program.GeneratePad(size: originalBytes.Length, seed: 1);
              // encrypted message
              byte[] encryptedBytes = Encoding.Unicode.GetBytes("../../../../OneTimePad/Message-encrypted.txt");
              // decrypt the message
              byte[] decrypted = OneTimePad.Program.Decrypt(encryptedBytes, pad);
              // compare the original message and decrypted message
              if(decrypted == originalBytes)
              {
              // The pad is accepted, so trust and accept override
              }
              else
              {
              // The pad is NOT accepted, no lower trust and deny override
              }
            } else {
              Send("Awaiting Confirmation OR send override");
              Sessions.Broadcast($"SECOND*{e.Data.Split("*")[1]}*{e.Data.Split("*")[2]}*{satellite_name}");  
            }
          }
        } else if(e.Data.StartsWith("SECONDRESPONSE")) {
          Send("OK");
          initial_grnd = e.Data.Split("*")[1];
          if (e.Data.Split("*")[2]=="TRUE") {
            // execute action and improve trust
            trust_model.Improve(initial_grnd);
            Console.WriteLine("CONFIRMED SECOND CHOICE");
          } else {
            // reject and degrade trust
            trust_model.Degrade(initial_grnd);
            Console.WriteLine("FAILS SECOND CHOICE");
          }
        } else if (e.Data.StartsWith("GROUNDCONNECTED")) {
          //tell ground if any actions failed
          // add connection to trust model
          string station_name = e.Data.Split("*")[1];
          if(station_name!="e1"&station_name!="e2"){
            trust_model.AddStation(station_name);
          }
          Send("READY*"+satellite_name);
        }
    }
  }

  public class Program
  {
    public List<string> convergence_name;
    public String satellite_name = "CALSPHERE 2";
    public String pending_station_name = ""; 
    public bool waiting_confirmation = false;
    public static void Main (string[] args)
    {
      var wssv = new WebSocketServer ("ws://localhost:1234");
      wssv.AddWebSocketService<Commands>("/Commands");
      wssv.Start();
      Console.ReadKey (true);
      wssv.Stop ();
    }
  }

  public class TrustModel {
    Dictionary<string, double> trust_pool = new Dictionary<string, double>();
    public TrustModel() {
      trust_pool.Add("e1", 1);
      trust_pool.Add("e2", 0.4);
    }

    public void AddStation(string key) {
      trust_pool.Add(key, 0.5);
    }
    public double GetTrust(string key) {
      return trust_pool[key];
    }

    public void Degrade(string key) {
      Console.WriteLine(key);
      if (trust_pool[key] == 1) {
        return;
      } else {
        trust_pool[key] -= 0.1;
      }
    }

    public void Improve(string key) {
      if (trust_pool[key] == 1) {
        return;
      } else {
        trust_pool[key] += 0.1;
      }
    }
  }
}