using Google.Protobuf.Reflection;
using Proto;

namespace Kirara;

public static class ProtoHelper
{
    public static readonly Dictionary<string, MessageDescriptor> fullNameToDescriptor = new();

    static ProtoHelper()
    {
        foreach (MessageDescriptor descriptor in MessageReflection.Descriptor.MessageTypes)
        {
            fullNameToDescriptor.Add(descriptor.FullName, descriptor);
        }
    }
}