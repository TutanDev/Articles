using System.Text.Json.Serialization;




    [JsonSourceGenerationOptions(
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        GenerationMode = JsonSourceGenerationMode.Default)]
    [JsonSerializable(typeof(CatalogEnvelope))]
    internal partial class SerContext : JsonSerializerContext { }

