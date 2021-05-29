using System;
using CatDarkRift;
using Common.Data;
using Common.Tags;
using Common.Util;
using DarkRift.Server;
using Plugin.Game;
using Plugin.Maps;
using Ping = Common.Data.Ping;

namespace Plugin
{
    public class GameOfLifePlugin : BasePlugin
    {
        public static GameOfLifePlugin Inst;
        public override Version Version => new Version(1, 0, 0);
        public override bool ThreadSafe => true;

        internal Room Room = new Room(new NineByNine());
        
        public GameOfLifePlugin(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            Inst = this;
            ClientManager.ClientConnected += ClientConnected;
        }

        #region Event Subscribers

        public static event EventHandler<ClientConnectedEventArgs> ClientConnected = (sender, ev) =>
        {
            Inst.Room.AddPlayer(ev.Client);
            if (Inst.Room.Players.Count == 2)
            {
                Inst.Room.Dispatch();
            }

            ev.Client.SendMessage(ToClient.Ping, new Ping("Ping!"));
        };

        [MessageHandler((ushort) ToServer.Ping, typeof(Ping))]
        private void _pingHandler(IClient client, Ping ping)
        {
            Console.WriteLine($"Pinged - responding Pong! Message: {ping.Message}");
            client.SendMessage(ToClient.Pong, new Ping("Pong!"));
        }

        [MessageHandler((ushort)ToServer.CellPlacement, typeof(PregameGrid))]
        private void _pingHandler(PregameGrid grid)
        {
            Console.WriteLine("Grid!!!");
            Console.WriteLine(grid.Grid.Width());
            Console.WriteLine(grid.Grid.Height());
        }

        #endregion
    }
}
