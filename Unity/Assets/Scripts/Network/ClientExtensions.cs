using System;
using Common.Tags;
using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;

namespace Network
{
    public static class ClientExtensions
    {
        public static void WaitForMessage<T>(this UnityClient client, ToClient tag, Action<T> callback) where T : IDarkRiftSerializable, new()
        {
            void subscriber(object sender, MessageReceivedEventArgs ev)
            {
                if ((ToClient)ev.Tag != tag) return;

                client.MessageReceived -= subscriber;
                var data = ev.GetMessage().Deserialize<T>();
                callback(data);
            }

            client.MessageReceived += subscriber;
        }
    }
}
