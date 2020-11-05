using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace Demo.Infrastructure
{
    public class JsonInfrastructure
    {
        public class SisoJsonDefaultContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(
                MemberInfo member,
                MemberSerialization memberSerialization)
            {
                //TODO: Maybe cache
                var prop = base.CreateProperty(member, memberSerialization);

                if (!prop.Writable)
                {
                    var property = member as PropertyInfo;
                    if (property != null)
                    {
                        var hasPrivateSetter = property.GetSetMethod(true) != null;
                        prop.Writable = hasPrivateSetter;
                    }
                }

                return prop;
            }
        }

        public static JsonSerializerSettings JsonSettings = new JsonSerializerSettings()
        {
            ContractResolver = new SisoJsonDefaultContractResolver(),
            TypeNameHandling = TypeNameHandling.All
        };
    }
}
