namespace Ara3D.Services
{

    public static class RepoSerializerExtensions
    {
        /*
        public class JsonIncludeReadonlyAutoPropertiesResolver : DefaultContractResolver
        {
            // See: https://github.com/JamesNK/Newtonsoft.Json/issues/703
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var jsonProperty = base.CreateProperty(member, memberSerialization);

                if (jsonProperty.Writable == false)
                {
                    // this uses compiler implementation details and may not work for all cases
                    // better to use smarter approach like in BackingFieldResolver from Mono.Reflection library
                    var fieldInfo = jsonProperty.DeclaringType.GetField($"<{jsonProperty.PropertyName}>k__BackingField",
                        BindingFlags.NonPublic | BindingFlags.Instance);

                    if (fieldInfo != null)
                    {
                        jsonProperty.ValueProvider = new ReflectionValueProvider(fieldInfo);
                        jsonProperty.Writable = true;
                    }
                }

                return jsonProperty;
            }
        }
        public static JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ContractResolver = new JsonIncludeReadonlyAutoPropertiesResolver()
        };

        public static JsonText ToJson(this IModel model)
            => JsonConvert.SerializeObject(model.Value, JsonSettings);

        public static JsonText ToJson(this IRepository repo)
            => JsonConvert.SerializeObject(repo.GetValues().ToArray(), JsonSettings);

        public static T LoadFromJson<T>(this T repo, JsonText content) where T: IRepository
        {
            var type = repo.ValueType.MakeArrayType();
            var tmp = (Array)JsonConvert.DeserializeObject(content, type, JsonSettings);

            if (repo.IsSingleton)
            {
                if (tmp.Length != 1) throw new Exception("Singleton repository can only have one item");
                var model = repo.GetSingleModel();
                model.Value = tmp.GetValue(0);
            }
            else
            {
                repo.Clear();
                for (var i = 0; i < tmp.Length; ++i)
                {
                    repo.Add(tmp.GetValue(i));
                }
            }

            return repo;
        }
        */
    }
}
