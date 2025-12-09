using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Utils
{
    public interface IKeyRotator
    {
        string ServiceName { get; }
        string GetNextKey();
    }

    public class KeyRotator : IKeyRotator
    {
        private readonly List<string> _keys;
        private int _index = -1;
        private readonly object _lock = new();

        public string ServiceName { get; }

        public KeyRotator(List<string> keys, string serviceName)
        {
            _keys = keys ?? throw new ArgumentNullException(nameof(keys));
            ServiceName = serviceName;
        }

        public string GetNextKey()
        {
            lock (_lock)
            {
                _index = (_index + 1) % _keys.Count;
                return _keys[_index];
            }
        }
    }
}
