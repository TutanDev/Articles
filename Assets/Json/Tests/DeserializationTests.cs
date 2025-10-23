using System.IO;
using System.Text;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;
using Newtonsoft.Json;
using System.Buffers;

namespace TutanDev.Json.Tests
{
    public class DeserializationTests
    {
        private static byte[] s_smallBytes = null!;
        private const string SMALL = "benchmark-semicomplex-small";
        private const string MEDIUM = "benchmark-semicomplex-medium";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var ta = Resources.Load<TextAsset>(MEDIUM);
            Assert.IsNotNull(ta, $"Resources.Load<TextAsset>(\"{MEDIUM}\") returned null. " +
                                 $"Place the json at Assets/Resources/{MEDIUM}.json");
            s_smallBytes = ta.bytes;
            Assert.Greater(s_smallBytes.Length, 0, "Empty test payload");
        }

        // ---------- System.Text.Json (bytes + source-gen) ----------
        [Test, Performance]
        public void Deserialize_STJ()
        {
            Measure.Method(() =>
            {
                var env = System.Text.Json.JsonSerializer.Deserialize(s_smallBytes, SerContext.Default.CatalogEnvelope)!;
            }).WarmupCount(10)
              .MeasurementCount(100)
              .IterationsPerMeasurement(1)
              .GC()
              .Run();
        }

        // ---------- Newtonsoft (string baseline) ----------
        [Test, Performance]
        public void Deserialize_Newtonsoft_String()
        {
            var json = Encoding.UTF8.GetString(s_smallBytes);
            var serializer = CreateNewtonsoftSerializer();

            Measure.Method(() =>
            {
                using var sr = new StringReader(json);
                using var jr = new JsonTextReader(sr) { CloseInput = false };
                var env = serializer.Deserialize<CatalogEnvelope>(jr)!;
            }).WarmupCount(10)
              .MeasurementCount(100)
              .IterationsPerMeasurement(1)
              .GC()
              .Run();
        }

        // ---------- Newtonsoft (bytes + stream; apples-to-apples) ----------
        [Test, Performance]
        public void Deserialize_Newtonsoft_Bytes()
        {
            var serializer = CreateNewtonsoftSerializer();
            var pool = new NewtonsoftCharArrayPool(); 

            Measure.Method(() =>
            {
                using var ms = new MemoryStream(s_smallBytes, writable: false);
                using var sr = new StreamReader(ms, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: false);
                using var jr = new JsonTextReader(sr) { CloseInput = false, ArrayPool = pool };
                var env = serializer.Deserialize<CatalogEnvelope>(jr)!;
            }).WarmupCount(10)
              .MeasurementCount(100)
              .IterationsPerMeasurement(1)
              .GC()
              .Run();
        }

        // Helpers
        private static Newtonsoft.Json.JsonSerializer CreateNewtonsoftSerializer()
        {
            return Newtonsoft.Json.JsonSerializer.Create(new JsonSerializerSettings
            {
                DateParseHandling = DateParseHandling.None,
                FloatParseHandling = FloatParseHandling.Double,
                Culture = System.Globalization.CultureInfo.InvariantCulture
            });
        }

        private sealed class NewtonsoftCharArrayPool : Newtonsoft.Json.IArrayPool<char>
        {
            public char[] Rent(int minimumLength) => ArrayPool<char>.Shared.Rent(minimumLength);
            public void Return(char[] array) => ArrayPool<char>.Shared.Return(array);
        }
    }
}
