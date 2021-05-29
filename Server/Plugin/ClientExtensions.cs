using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Data;
using Common.Tags;
using DarkRift;
using DarkRift.Server;

namespace Plugin
{
    static class ClientExtensions
    {
        public static bool SendMessage<T>(this IClient client, ToClient tag, T data, SendMode mode = SendMode.Reliable) where T : IDarkRiftSerializable, new()
        {
            return client.SendMessage(Message.Create((ushort)tag, data), mode);
        }

        public static void WaitForMessage<T>(this IClient client, ToServer tag, Action<T> callback) where T : IDarkRiftSerializable, new()
        {
            void subscriber(object sender, MessageReceivedEventArgs ev)
            {
                if ((ToServer)ev.Tag != tag) return;

                client.MessageReceived -= subscriber;
                var data = ev.GetMessage().Deserialize<T>();
                callback(data);
            }

            client.MessageReceived += subscriber;
        }

        public static void RequestCellPlacement(this IClient client, sbyte player, PregameGrid start, Action<PregameGrid> callback)
        {
            client.SendMessage(ToClient.RequestCellPlacement, new RequestCellPlacement{Grid = start, Player = player});
            client.WaitForMessage(ToServer.CellPlacement, callback);
        }

        public static void StartSimulation(this IClient client, sbyte[,] grid, sbyte player)
        {
            client.SendMessage(ToClient.StartSimulation, new StartSimulation{Grid = grid, Player = player});
        }
    }
}
