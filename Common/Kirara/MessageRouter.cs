using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Proto;
using Serilog;

namespace Kirara;

public class MessageRouter : Singleton<MessageRouter>
{
    private struct MessageUnit(Connection conn, string messageName, IMessage message)
    {
        public readonly Connection conn = conn;
        public readonly string messageName = messageName;
        public readonly IMessage message = message;
    }

    private readonly ConcurrentQueue<MessageUnit> messageQueue = new();
    private readonly ConcurrentDictionary<string, Delegate> messageNameToAction = new();

    private readonly AutoResetEvent messageAvailableEvent = new(false);
    private readonly CancellationTokenSource cts = new();
    private int workerCount;

    public delegate void MessageHandler<T>(Connection conn, T message) where T : IMessage;

    public void AddMessage(Connection conn, IMessage message)
    {
        // Log.Information("Adding message {@Message}", message);
        messageQueue.Enqueue(new MessageUnit(conn, message.Descriptor.FullName, message));
        messageAvailableEvent.Set();
    }

    public void Subscribe<T>(MessageHandler<T> handler) where T : IMessage
    {
        string name = GetName<T>();
        if (name == null)
        {
            Log.Warning($"Can't get name. T={typeof(T)}");
            return;
        }

        if (!messageNameToAction.TryAdd(name, handler))
        {
            messageNameToAction[name] = Delegate.Combine(messageNameToAction[name], handler);
        }
    }

    public void Unsubscribe<T>(MessageHandler<T> handler) where T : IMessage
    {
        string name = GetName<T>();
        if (name == null)
        {
            Log.Warning($"Can't get name. T={typeof(T)}");
            return;
        }

        if (messageNameToAction.TryGetValue(name, out var handlers))
        {
            handlers = Delegate.Remove(handlers, handler);
            if (handlers is null)
            {
                messageNameToAction.TryRemove(name, out _);
            }
            else
            {
                messageNameToAction[name] = handlers;
            }
        }
    }

    private static string GetName<T>() where T : IMessage
    {
        var propertyInfo = typeof(T).GetProperty("Descriptor", BindingFlags.Static | BindingFlags.Public);
        var descriptor = propertyInfo?.GetValue(null) as MessageDescriptor;
        return descriptor?.FullName;
    }

    public void Start(int threadCount)
    {
        for (int i = 0; i < threadCount; i++)
        {
            new Thread(() => Work(cts.Token)).Start();
        }
    }

    public void Stop()
    {
        cts.Cancel();
        while (workerCount > 0)
        {
            messageAvailableEvent.Set();
            Thread.Sleep(1);
        }
        messageQueue.Clear();
    }

    private void Work(CancellationToken token)
    {
        Interlocked.Increment(ref workerCount);
        Log.Information("消息路由工作线程启动");
        try
        {
            while (!token.IsCancellationRequested)
            {
                if (messageQueue.TryDequeue(out var messageUnit))
                {
                    ProcessMessage(messageUnit);
                }
                else
                {
                    messageAvailableEvent.WaitOne();
                }
            }
        }
        catch (Exception ex)
        {
            Log.Information(ex.Message + ex.StackTrace);
        }
        finally
        {
            Log.Information("工作线程结束");
            Interlocked.Decrement(ref workerCount);
        }
    }

    private void ProcessMessage(MessageUnit messageUnit)
    {
        if (messageNameToAction.TryGetValue(messageUnit.messageName, out var action))
        {
            action.DynamicInvoke(messageUnit.conn, messageUnit.message);
        }
    }
}