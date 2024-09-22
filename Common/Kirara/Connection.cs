using System.Buffers.Binary;
using System.Net.Sockets;
using Google.Protobuf;
using Proto;

namespace Kirara;

public class Connection : TypeStore
{
    public event Action<Connection, IMessage> Received;
    public event Action<Connection> Disconnected;

    public readonly Socket socket;

    private readonly byte[] receiveBuffer;
    private readonly int receiveBufferSize = 1024 * 1024;

    private readonly CancellationTokenSource cts;

    public Connection(Socket socket)
    {
        this.socket = socket;
        receiveBuffer = new byte[receiveBufferSize];
        cts = new CancellationTokenSource();
        Receive(cts.Token);
    }

    public void Close()
    {
        cts.Cancel();
        socket.Close();
    }

    public void Send(IMessage message)
    {
        var packet = new Packet()
        {
            MessageName = message.Descriptor.FullName,
            Message = message.ToByteString(),
        };

        int size = packet.CalculateSize();
        byte[] sendBytes = new byte[sizeof(int) + size];
        BitConverter.TryWriteBytes(new Span<byte>(sendBytes, 0, sizeof(int)), size);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(sendBytes, 0, sizeof(int));
        }
        packet.WriteTo(new Span<byte>(sendBytes, sizeof(int), size));

        socket.SendAsync(sendBytes, SocketFlags.None, cts.Token);
    }

    private async Task Receive(CancellationToken token)
    {
        int receivedCount = 0;
        while (!cts.IsCancellationRequested)
        {
            int receiveCount;
            while (receivedCount < sizeof(int))
            {
                receiveCount = await socket.ReceiveAsync(
                    new ArraySegment<byte>(receiveBuffer, receivedCount, receiveBuffer.Length - receivedCount),
                    token);
                if (receiveCount == 0)
                {
                    Close();
                    Disconnected?.Invoke(this);
                    return;
                }
                receivedCount += receiveCount;
            }

            int size = BinaryPrimitives.ReadInt32BigEndian(new ReadOnlySpan<byte>(receiveBuffer, 0, sizeof(int)));
            while (receivedCount < sizeof(int) + size)
            {
                receiveCount = await socket.ReceiveAsync(
                    new ArraySegment<byte>(receiveBuffer, receivedCount, receiveBuffer.Length - receivedCount),
                    token);
                if (receiveCount == 0)
                {
                    Close();
                    Disconnected?.Invoke(this);
                    return;
                }
                receivedCount += receiveCount;
            }

            var packet = Packet.Parser.ParseFrom(new Span<byte>(receiveBuffer, sizeof(int), size));
            var message = ProtoHelper.fullNameToDescriptor[packet.MessageName].Parser.ParseFrom(packet.Message);
            Received?.Invoke(this, message);

            Array.Copy(receiveBuffer, sizeof(int) + size,
                receiveBuffer, 0, receivedCount - sizeof(int) - size);
            receivedCount -= sizeof(int) + size;
        }
    }
}