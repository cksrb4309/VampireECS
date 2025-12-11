using System;
using System.Threading;
using UnityEngine;


public static class EventBus<TMessage>
{
    // 내부 델리게이트(멀티캐스트)
    private static Action<TMessage> _handlers;

    // 락 객체: 안전한 구독/발행을 위해 사용
    private static readonly object _lock = new object();

    [RuntimeInitializeOnLoadMethod]
    private static void InitializeOnLoad()
    {
        _handlers = null;
    }
    public static IDisposable Subscribe(Action<TMessage> handler)
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));

        lock (_lock)
        {
            _handlers += handler;
        }

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
        lock (_lock)
        {
            _handlers -= handler;
        }
    }
    public static void Publish(TMessage msg)
    {
        // 로컬 복사 후 호출 (thread-safe snapshot)
        Action<TMessage> snapshot;
        lock (_lock)
        {
            snapshot = _handlers;
        }

        snapshot?.Invoke(msg);
    }
    public static void UnsubscribeAll()
    {
        lock (_lock)
        {
            _handlers = null;
        }
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
