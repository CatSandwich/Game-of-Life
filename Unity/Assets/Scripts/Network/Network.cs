using System;
using Common.Data;
using Common.Tags;
using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using InfoObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class Network : MonoBehaviour
    {
        public static Network Inst;
        public UnityClient Client;

        public bool SendMessage<T>(ToServer tag, T data, SendMode mode = SendMode.Reliable) where T : IDarkRiftSerializable, new()
        {
            return Client.SendMessage(Message.Create((ushort) tag, data), mode);
        }
        
        // Start is called before the first frame update
        private void Start()
        {
            if (Inst != null)
            {
                Destroy(gameObject);
                return;
            }
            Inst = this;
            DontDestroyOnLoad(gameObject);

            Client.MessageReceived += _messageReceived;
        }

        private static EventHandler<MessageReceivedEventArgs> _messageReceived = (sender, ev) =>
        {
            switch ((ToClient) ev.Tag)
            {
                case ToClient.Ping:
                {
                    Debug.Log($"Received ping. Message: {ev.GetMessage().Deserialize<Common.Data.Ping>().Message}");
                    Inst.SendMessage(ToServer.Pong, new Common.Data.Ping("Pong"));
                    break;
                }
                case ToClient.Pong:
                {
                    Debug.Log($"Received pong. Message: {ev.GetMessage().Deserialize<Common.Data.Ping>().Message}");
                    break;
                }
                case ToClient.RequestCellPlacement:
                {
                    new GameObject()
                        .AddComponent<InfoRequestCellPlacement>()
                        .Set(ev.GetMessage().Deserialize<RequestCellPlacement>());
                    SceneManager.LoadScene("CellPlacement");
                    break;
                }
                case ToClient.StartSimulation:
                {
                    new GameObject()
                        .AddComponent<InfoStartSimulation>()
                        .Set(ev.GetMessage().Deserialize<StartSimulation>());
                    SceneManager.LoadScene("StartSimulation");
                    break;
                }
            }
        };
    }
}
