using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Satellite
{
  public class Commands : WebSocketBehavior
  {
    public List<string> convergence_name;
    public String satellite_name = "";
    public bool waiting_confirmation = false;
    protected override void OnMessage (MessageEventArgs e)
    {
        if (e.Data.StartsWith("COMMAND"))
        {
          if(waiting_confirmation){
            if(false){//if(trusted)
                Send("Running CMD");
                //output orbital change request
            }else{ 
                // waiting for second confirmation or one time pass 
                waiting_confirmation = true;
                convergence_name = new List<string>();
                convergence_name.InsertRange(0,e.Data.Split(" ")[1].Split(","));
                Send("Getting Confirmation OR send override");
            }
          }else{
            if(e.Data.Split(" ").Length>2){
              //process pad overide
            }
          }
        }else if(e.Data.StartsWith("SECONDREQUEST")){
            var msg = "SECOND intercept {sat name} {time}";
            Send("OK");
        }else if(e.Data.StartsWith("SECONDRESPONSE")){
            Send("OK");
            if(e.Data.Split(" ")[1]=="TRUE"){
            //      take action ordered
            }else{
            //      reject and degrade trust model 
            }
        } else if(e.Data.StartsWith("GROUNDCONNECTED")){
            //tell ground if any actions failed
            // add connection to trust model
            if(convergence_name.Count>0){
              Send($"CONVERGENCE {String.Join(",",convergence_name)} {satellite_name}");
              convergence_name=new List<string>();
            }else{
              Send("OK");
            }
        }
    }
  }

  public class Program
  {
    public static void Main (string[] args)
    {
      var wssv = new WebSocketServer ("ws://localhost:1234");
      wssv.AddWebSocketService<Commands> ("/Commands");
      wssv.Start ();
      Console.ReadKey (true);
      wssv.Stop ();
    }
  }
}