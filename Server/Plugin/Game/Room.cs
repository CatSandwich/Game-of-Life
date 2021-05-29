using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Data;
using Common.Simulation;
using Common.Tags;
using DarkRift.Server;
using Plugin.Maps;

namespace Plugin.Game
{
    internal class Room
    {
        public Map Map;
        public List<Player> Players = new List<Player>();

        public ushort InputRequired = 0;
        public object InputRequiredLocker = new object();

        public Room(Map map)
        {
            Map = map;
        }

        public void AddPlayer(IClient client)
        {
            Players.Add(new Player{Client = client, Value = Convert.ToSByte(Players.Count + 1)});
        }
        
        public void Dispatch()
        {
            lock(InputRequiredLocker) InputRequired = 0;
            foreach (var player in Players)
            {
                lock (InputRequiredLocker) InputRequired++;
                player.Client.RequestCellPlacement(player.Value, Map.Grid, grid =>
                {
                    Console.WriteLine($"Input received from player {player.Value}");
                    Map.Grid.CopyUserInput(player.Value, grid);
                    lock (InputRequiredLocker)
                    {
                        InputRequired--;
                        if (InputRequired == 0) Run();
                    }
                });
            }
        }
        
        public void Run()
        {
            var sim = Map.Grid.ToSimulation();

            foreach (var player in Players)
            {
                player.Client.StartSimulation(sim.Grid, player.Value);
            }
            
            sim.SimulationComplete += (sender, args) =>
            {
                foreach (var player in Players)
                {
                    player.Client.SendMessage(ToClient.SimulationResults,
                        new SimulationResults
                        {
                            Cause = args.Cause, 
                            Elapsed = args.State.Elapsed, 
                            Iteration = args.State.Iteration
                        });
                }
            };
        }
    }
}
