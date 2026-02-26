using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// SignalBus managed by the SystemLocator. Use SignalBus.Instance to access the service.
/// Example: SignalBus.Instance.Subscribe&lt;T&gt;(handler); SignalBus.Instance.Publish(new T());
/// </summary>
public class SignalBus : IService
{
    public static SignalBus Instance => SystemLocator.Instance.SignalBus;

    readonly Dictionary<Type, Delegate> handlers = new Dictionary<Type, Delegate>();
    readonly object sync = new object();

    public bool IsReady { get; private set; } = true;

    public void Subscribe<T>(Action<T> handler)
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));
        lock (sync)
        {
            var key = typeof(T);
            if (handlers.TryGetValue(key, out var existing))
            {
                handlers[key] = Delegate.Combine(existing, handler);
            }
            else
            {
                handlers[key] = handler;
            }
        }
    }

    public void Unsubscribe<T>(Action<T> handler)
    {
        if (handler == null) return;
        lock (sync)
        {
            var key = typeof(T);
            if (!handlers.TryGetValue(key, out var existing)) return;
            var removed = Delegate.Remove(existing, handler);
            if (removed == null)
                handlers.Remove(key);
            else
                handlers[key] = removed;
        }
    }

    public void Publish<T>(T signal)
    {
        Delegate d;
        lock (sync)
        {
            handlers.TryGetValue(typeof(T), out d);
        }

        if (d == null) return;

        var list = d.GetInvocationList();
        foreach (var item in list)
        {
            if (item is Action<T> action)
            {
                try { action(signal); }
                catch (Exception ex) { Debug.LogException(ex); }
            }
        }
    }

    public Action<T> BindUnityEvent<T>(UnityEvent<T> unityEvent)
    {
        if (unityEvent == null) throw new ArgumentNullException(nameof(unityEvent));
        Action<T> handler = (t) => unityEvent.Invoke(t);
        Subscribe(handler);
        return handler;
    }
}

