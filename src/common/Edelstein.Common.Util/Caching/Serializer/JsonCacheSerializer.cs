﻿using System.Text.Json;
using System.Threading.Tasks;
using Edelstein.Protocol.Util.Caching.Serializer;

namespace Edelstein.Common.Util.Caching.Serializer
{
    public class JsonCacheSerializer : ICacheSerializer
    {
        public Task<T> Deserialize<T>(string data)
            => Task.FromResult(JsonSerializer.Deserialize<T>(data));

        public Task<string> Serialize<T>(T obj)
            => Task.FromResult(JsonSerializer.Serialize(obj));
    }
}