using System;
using System.Threading;
using UnityEngine;
public static class EventBus<TMessage>
{
    // 내부 델리게이트(멀티캐스트)
    private static Action<TMessage> _handlers;

    [RuntimeInitializeOnLoadMethod]
    private static void InitializeOnLoad()
    {
        _handlers = null;
    }
    public static IDisposable Subscribe(Action<TMessage> handler)
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));

#pragma warning disable UDR0005 // Domain Reload Analyzer
        _handlers += handler;
#pragma warning restore UDR0005 // Domain Reload Analyzer

        return new Subscription(() => Unsubscribe(handler));
    }
    public static void SubscribeOnce(Action<TMessage> handler)
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));

        Action<TMessage> wrapper = null;

        wrapper = (msg) =>
        {
            try { handler(msg); }
            finally { Unsubscribe(wrapper); }
        };

        Subscribe(wrapper);
    }
    public static void Unsubscribe(Action<TMessage> handler)
    {
        if (handler == null) return;

        _handlers -= handler;
    }
    public static void Publish(TMessage msg)
    {
        Action<TMessage> snapshot;

        snapshot = _handlers;

        snapshot?.Invoke(msg);
    }
    public static void UnsubscribeAll()
    {
        _handlers = null;
    }
    private sealed class Subscription : IDisposable
    {
        private Action _disposeAction;
        private int _disposed; // 0 = false, 1 = true

        public Subscription(Action disposeAction)
        {
            _disposeAction = disposeAction ?? throw new ArgumentNullException(nameof(disposeAction));
        }
        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                try { _disposeAction(); }
                finally { _disposeAction = null; }
            }
        }
    }
}
