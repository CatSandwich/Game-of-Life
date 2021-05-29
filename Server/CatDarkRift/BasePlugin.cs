using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DarkRift;
using DarkRift.Server;

namespace CatDarkRift
{
    public abstract class BasePlugin : Plugin
    {
        private readonly Dictionary<ushort, List<MessageHandler>> _methods = new Dictionary<ushort, List<MessageHandler>>();

        // Add handler to respective list, creating a new one if necessary
        private void _addHandler(ushort tag, MessageHandler handler)
        {
            if (_methods.TryGetValue(tag, out var list)) list.Add(handler);
            _methods.Add(tag, new List<MessageHandler>(new []{handler}));
        }

        protected BasePlugin(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            // Get Message.Deserialize<>() for later binding
            var unboundDeserialize = typeof(Message).GetMethod("Deserialize");
            if (unboundDeserialize is null)
                throw new ArgumentException($"Could not find method 'Deserialize' in type {nameof(Message)}");

            var methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            
            // foreach method in the plugin
            foreach (var m in methods)
            {
                // foreach MessageHandlerAttribute on the method
                foreach (var handler in m.GetCustomAttributes().OfType<MessageHandlerAttribute>())
                {
                    // Add the handler to the dictionary
                    _addHandler(handler.Tag, new MessageHandler(m, unboundDeserialize.MakeGenericMethod(handler.Type), handler.Type));
                }
            }

            ClientManager.ClientConnected += _clientConnected;
        }

        private void _clientConnected(object sender, ClientConnectedEventArgs ev)
        {
            ev.Client.MessageReceived += _messageReceived;
        }

        private void _messageReceived(object sender, MessageReceivedEventArgs ev)
        {
            if (!_methods.TryGetValue(ev.Tag, out var handlers)) return;
            
            // foreach handler with the received tag
            foreach(var handler in handlers) 
                // Build the argument list, call the method with it
                handler.Method.Invoke(this, handler.BuildArguments(ev).ToArray());
        }
    }

    internal class MessageHandler
    {
        public MethodInfo Method;
        public MethodInfo Deserialize;
        public Type DataType;

        public MessageHandler(MethodInfo method, MethodInfo deserialize, Type dataType)
        {
            Method = method;
            Deserialize = deserialize;
            DataType = dataType;
            _argumentBuilders = new List<Func<MessageReceivedEventArgs, object>>();
            
            // Creates a list of functions that build an argument list
            // Allows messing up the signature (or intentionally leaving out unused data)
            foreach (var type in method.GetParameters().Select(p => p.ParameterType))
            {
                if (type == typeof(MessageReceivedEventArgs)) 
                    _argumentBuilders.Add(ev => ev);

                else if (type == typeof(IClient))
                    _argumentBuilders.Add(ev => ev.Client);
                
                else if (type == DataType) 
                    _argumentBuilders.Add(ev => deserialize.Invoke(ev.GetMessage(), null));
                
                // Any other type
                else throw new ArgumentException($"Invalid parameter type {type} in method {method.Name}");
            }
        }
        
        public IEnumerable<object> BuildArguments(MessageReceivedEventArgs ev) => _argumentBuilders.Select(b => b.Invoke(ev));
        
        private readonly List<Func<MessageReceivedEventArgs, object>> _argumentBuilders;

        public void Deconstruct(out MethodInfo method, out MethodInfo deserialize)
        {
            method = Method;
            deserialize = Deserialize;
        }
    }
}
