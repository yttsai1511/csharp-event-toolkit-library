using System.Collections.Generic;

namespace System.Event
{
    public sealed class EventToolkit
    {
        private readonly struct EventKey : IEquatable<EventKey>
        {
            public string EventName { get; }
            public Type EventType { get; }

            public EventKey(string name, Type type)
            {
                EventName = name;
                EventType = type;
            }

            public bool Equals(EventKey other)
            {
                return EventName == other.EventName && EventType == other.EventType;
            }
        }

        private readonly Dictionary<EventKey, Delegate> _events = new Dictionary<EventKey, Delegate>();
        private readonly object _lock = new object();

        /// <summary>
        /// Registers a delegate to an event.
        /// </summary>
        /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
        /// <param name="eventName">The unique name of the event.</param>
        /// <param name="del">The delegate to register.</param>
        /// <returns>True if registration succeeded; otherwise, false.</returns>
        public bool Register<TDelegate>(string eventName, TDelegate del)
            where TDelegate : Delegate
        {
            if (string.IsNullOrEmpty(eventName) || del == null)
            {
                return false;
            }

            var key = new EventKey(eventName, typeof(TDelegate));

            lock (_lock)
            {
                if (_events.TryGetValue(key, out var value))
                {
                    _events[key] = Delegate.Combine(value, del);
                }
                else
                {
                    _events.Add(key, del);
                }
            }
            return true;
        }

        /// <summary>
        /// Unregisters all delegates associated with the given event name.
        /// </summary>
        /// <param name="eventName">The name of the event to unregister.</param>
        /// <returns>True if unregistration succeeded; otherwise, false.</returns>
        public bool Unregister(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return false;
            }

            var keysToRemove = new List<EventKey>();

            lock (_lock)
            {
                foreach (var kvp in _events)
                {
                    if (kvp.Key.EventName == eventName)
                    {
                        keysToRemove.Add(kvp.Key);
                    }
                }

                foreach (var key in keysToRemove)
                {
                    _events.Remove(key);
                }
            }
            return true;
        }

        /// <summary>
        /// Unregisters a specific delegate from an event.
        /// </summary>
        /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="del">The delegate to unregister.</param>
        /// <returns>True if unregistration succeeded; otherwise, false.</returns>
        public bool Unregister<TDelegate>(string eventName, TDelegate del)
            where TDelegate : Delegate
        {
            if (string.IsNullOrEmpty(eventName) || del == null)
            {
                return false;
            }

            var key = new EventKey(eventName, typeof(TDelegate));

            lock (_lock)
            {
                if (_events.TryGetValue(key, out var value))
                {
                    _events[key] = Delegate.Remove(value, del);
                }
            }
            return true;
        }

        /// <summary>
        /// Clears all registered events and their associated delegates.
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _events.Clear();
            }
        }

        #region Invoke Methods
        
        /// <summary>
        /// Invokes an event.
        /// </summary>
        /// <param name="eventName">The name of the event to invoke.</param>
        /// <returns>True if the event was successfully invoked; otherwise, false.</returns>
        public bool Invoke(string eventName)
        {
            var key = new EventKey(eventName, typeof(Action));

            if (!_events.TryGetValue(key, out var value))
            {
                return false;
            }

            if (value is Action act)
            {
                act.Invoke();
            }
            return true;
        }

        public bool Invoke<T>(string eventName, T arg)
        {
            var key = new EventKey(eventName, typeof(Action<T>));

            if (!_events.TryGetValue(key, out var value))
            {
                return false;
            }

            if (value is Action<T> act)
            {
                act.Invoke(arg);
            }
            return true;
        }

        public bool Invoke<T1, T2>(string eventName, T1 arg1, T2 arg2)
        {
            var key = new EventKey(eventName, typeof(Action<T1, T2>));

            if (!_events.TryGetValue(key, out var value))
            {
                return false;
            }

            if (value is Action<T1, T2> act)
            {
                act.Invoke(arg1, arg2);
            }
            return true;
        }

        public bool Invoke<T1, T2, T3>(string eventName, T1 arg1, T2 arg2, T3 arg3)
        {
            var key = new EventKey(eventName, typeof(Action<T1, T2, T3>));

            if (!_events.TryGetValue(key, out var value))
            {
                return false;
            }

            if (value is Action<T1, T2, T3> act)
            {
                act.Invoke(arg1, arg2, arg3);
            }
            return true;
        }

        public bool Invoke<T1, T2, T3, T4>(string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var key = new EventKey(eventName, typeof(Action<T1, T2, T3, T4>));

            if (!_events.TryGetValue(key, out var value))
            {
                return false;
            }

            if (value is Action<T1, T2, T3, T4> act)
            {
                act.Invoke(arg1, arg2, arg3, arg4);
            }
            return true;
        }
        
        #endregion

        #region InvokeFunc Methods

        /// <summary>
        /// Invokes a function delegate and collects the results.
        /// </summary>
        /// <typeparam name="TResult">The return type of the function delegate.</typeparam>
        /// <param name="eventName">The unique name of the event to invoke.</param>
        /// <param name="results">A list to store the results of the function invocations.</param>
        /// <returns>True if the event was successfully invoked and results were collected; otherwise, false.</returns>
        public bool InvokeFunc<TResult>(string eventName, List<TResult> results)
        {
            var key = new EventKey(eventName, typeof(Func<TResult>));

            if (!_events.TryGetValue(key, out var value))
            {
                return false;
            }

            Delegate[] funcs = value.GetInvocationList();

            foreach (var a_obj in funcs)
            {
                if (a_obj is Func<TResult> func)
                {
                    TResult result = func.Invoke();
                    results.Add(result);
                }
            }
            return true;
        }

        public bool InvokeFunc<T, TResult>(string eventName, List<TResult> results, T arg)
        {
            var key = new EventKey(eventName, typeof(Func<T, TResult>));

            if (!_events.TryGetValue(key, out var value))
            {
                return false;
            }

            Delegate[] funcs = value.GetInvocationList();

            foreach (var a_obj in funcs)
            {
                if (a_obj is Func<T, TResult> func)
                {
                    TResult result = func.Invoke(arg);
                    results.Add(result);
                }
            }
            return true;
        }

        public bool InvokeFunc<T1, T2, TResult>(string eventName, List<TResult> results, T1 arg1, T2 arg2)
        {
            var key = new EventKey(eventName, typeof(Func<T1, T2, TResult>));

            if (!_events.TryGetValue(key, out var value))
            {
                return false;
            }

            Delegate[] funcs = value.GetInvocationList();

            foreach (var a_obj in funcs)
            {
                if (a_obj is Func<T1, T2, TResult> func)
                {
                    TResult result = func.Invoke(arg1, arg2);
                    results.Add(result);
                }
            }
            return true;
        }

        public bool InvokeFunc<T1, T2, T3, TResult>(string eventName, List<TResult> results, T1 arg1, T2 arg2, T3 arg3)
        {
            var key = new EventKey(eventName, typeof(Func<T1, T2, T3, TResult>));

            if (!_events.TryGetValue(key, out var value))
            {
                return false;
            }

            Delegate[] funcs = value.GetInvocationList();

            foreach (var a_obj in funcs)
            {
                if (a_obj is Func<T1, T2, T3, TResult> func)
                {
                    TResult result = func.Invoke(arg1, arg2, arg3);
                    results.Add(result);
                }
            }
            return true;
        }

        public bool InvokeFunc<T1, T2, T3, T4, TResult>(string eventName, List<TResult> results, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var key = new EventKey(eventName, typeof(Func<T1, T2, T3, T4, TResult>));

            if (!_events.TryGetValue(key, out var value))
            {
                return false;
            }

            Delegate[] funcs = value.GetInvocationList();

            foreach (var a_obj in funcs)
            {
                if (a_obj is Func<T1, T2, T3, T4, TResult> func)
                {
                    TResult result = func.Invoke(arg1, arg2, arg3, arg4);
                    results.Add(result);
                }
            }
            return true;
        }
        
        #endregion
    }
}