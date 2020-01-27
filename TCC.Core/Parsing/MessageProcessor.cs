using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TCC.Exceptions;
using TCC.Utils;
using TeraPacketParser;

namespace TCC.Parsing
{
    public class MessageProcessor
    {
        private ConcurrentDictionary<Type, List<Delegate>> _hooks { get; }

        private readonly object _lock = new object();

        public MessageProcessor()
        {
            _hooks = new ConcurrentDictionary<Type, List<Delegate>>();
        }

        public void Hook<T>(Action<T> action)
        {
            lock (_lock)
            {
                if (!_hooks.TryGetValue(typeof(T), out _)) _hooks[typeof(T)] = new List<Delegate>();
                if (!_hooks[typeof(T)].Contains(action)) _hooks[typeof(T)].Add(action);
            }
        }
        public void Unhook<T>(Action<T> action)
        {
            lock (_lock)
            {
                if (!_hooks.TryGetValue(typeof(T), out var handlers)) return;
                handlers.Remove(action);
            }
        }
        public void Handle(ParsedMessage msg)
        {
            if (!_hooks.TryGetValue(msg.GetType(), out var handlers) || handlers == null) return;
            lock (_lock)
            {
                //Log.All($"Handling {msg.GetType().Name}");
                handlers.ForEach(del =>
                {
                    try
                    {
                        del.DynamicInvoke(msg);
                    }
                    catch (Exception e)
                    {
                        Log.F($"Error while executing callback for {msg.GetType()}.\n{e}\n{e.InnerException}");
                        throw new MessageProcessException($"Error while executing callback for {msg.GetType()}", e);
                    }
                });
            }
        }
    }
}