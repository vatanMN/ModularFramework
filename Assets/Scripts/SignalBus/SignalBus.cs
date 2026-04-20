
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ModularFW.Core.Locator;

namespace ModularFW.Core.Signal
{

/// <summary>
/// SignalBus managed by the SystemLocator. Use SignalBus.Instance to access the service.
/// Example: SignalBus.Instance.Subscribe&lt;T&gt;(handler); SignalBus.Instance.Publish(new T());
/// </summary>
    public class SignalBus : IService, ModularFW.Core.ISignalBus
{
    public static SignalBus Instance => SystemLocator.Instance.SignalBus;

    readonly Dictionary<Type, Delegate> handlers = new Dictionary<Type, Delegate>();
    readonly object sync = new object();

    public bool IsReady { get; private set; } = true;

    /// <summary>
    /// Subscribe to signal type T. The exact same handler instance must later be passed to
    /// <see cref="Unsubscribe{T}"/>. If you are using a lambda, use
    /// <see cref="SubscribeTracked{T}"/> instead — each lambda expression creates a new
    /// delegate instance and <see cref="Unsubscribe{T}"/> will silently fail to remove it.
    /// </summary>
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

    /// <summary>
    /// Subscribe and receive a disposable token. Disposing the token unsubscribes safely,
    /// even when the handler was a lambda. Recommended over Subscribe+Unsubscribe for
    /// anonymous methods and for MonoBehaviours that manage subscriptions in OnDestroy.
    /// <code>
    /// _sub = SignalBus.Instance.SubscribeTracked&lt;MySignal&gt;(s => Handle(s));
    /// // later, e.g. in OnDestroy:
    /// _sub?.Dispose();
    /// </code>
    /// </summary>
    public IDisposable SubscribeTracked<T>(Action<T> handler)
    {
        Subscribe(handler);
        return new Subscription(() => Unsubscribe(handler));
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

    private sealed class Subscription : IDisposable
    {
        private Action _unsubscribe;
        internal Subscription(Action unsubscribe) => _unsubscribe = unsubscribe;
        public void Dispose() { _unsubscribe?.Invoke(); _unsubscribe = null; }
    }
    }
}

