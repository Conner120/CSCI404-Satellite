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
    public String satellite_name = "CALSPHERE 1";
    protected override void OnMessage (MessageEventArgs e)
    {
        if (e.Data.StartsWith("COMMAND"))
        {
            if(false){//if(trusted)
                Send("Running CMD");
                //output orbital change request
            }else{ 
                // waiting for second confirmation or one time pass 
              Send("Awaiting Confirmation OR send override");
              Sessions.Broadcast($"SECOND*{e.Data.Split("*")[1]}*{e.Data.Split("*")[2]}*{satellite_name}");  
              if(e.Data.Split("*").Length>4){
                //process pad overide
              }
            }
        }else if(e.Data.StartsWith("SECONDRESPONSE")){
            Send("OK");
            if(e.Data.Split("*")[1]=="TRUE"){
            //      take action ordered
            }else{
            //      reject and degrade trust model 
            }
        } else if(e.Data.StartsWith("GROUNDCONNECTED")){
            //tell ground if any actions failed
            // add connection to trust model
            string station_name = e.Data.Split("*")[1];
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
}