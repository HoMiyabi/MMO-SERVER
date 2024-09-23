using System.Collections.Generic;

namespace Kirara
{
    public class TypeStore
    {
        private Dictionary<string, object> dict = new();

        public void Set<T>(T value)
        {
            string key = typeof(T).FullName;
            dict[key] = value;
        }

        public T Get<T>()
        {
            string key = typeof(T).FullName;
            if (dict.TryGetValue(key, out object value))
            {
                return (T)value;
            }
            return default;
        }
    }
}