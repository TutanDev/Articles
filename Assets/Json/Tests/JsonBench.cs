using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;

using System.Globalization;
using static JsonBench;






#if UNITY_EDITOR
using UnityEditor;
#endif

public static class JsonBench
{
#if UNITY_EDITOR
    [MenuItem("Benchmarks/Run JSON Bench (STJ bytes)")]
    private static void RunMenu()
    {
        var path = EditorUtility.OpenFilePanel("Select JSON file", Application.dataPath, "json");
        if (!string.IsNullOrEmpty(path))
            Run(path, iterations: 30, warmup: 5);
    }
#endif

#if UNITY_EDITOR
    [MenuItem("Benchmarks/Run JSON Bench (Newtonsoft)")]
    private static void RunNewtonsoftMenu()
    {
        var path = EditorUtility.OpenFilePanel("Select JSON file", Application.dataPath, "json");
        if (!string.IsNullOrEmpty(path))
            RunNewtonsoft(path, iterations: 30, warmup: 5);
    }
#endif

#if UNITY_EDITOR
    [MenuItem("Benchmarks/Run JSON Bench (Newtonsoft bytes)")]
    private static void RunNewtonsoftBytesMenu()
    {
        var path = EditorUtility.OpenFilePanel("Select JSON file", Application.dataPath, "json");
        if (!string.IsNullOrEmpty(path))
            RunNewtonsoftBytes(path, iterations: 30, warmup: 5);
    }
#endif

    public static void Run(string filePath, int iterations = 30, int warmup = 5)
    {
        if (!File.Exists(filePath))
        {
            UnityEngine.Debug.LogError($"File not found: {filePath}");
            return;
        }

        // Load bytes once (avoid I/O in the hot loop)
        byte[] bytes = File.ReadAllBytes(filePath);
        UnityEngine.Debug.Log($"Loaded {bytes.Length:N0} bytes from {filePath}");

        // Quick correctness check (parse once)
        var first = System.Text.Json.JsonSerializer.Deserialize(bytes, SerContext.Default.CatalogEnvelope);
        if (first == null)
        {
            UnityEngine.Debug.LogError("Initial deserialize returned null.");
            return;
        }
        UnityEngine.Debug.Log($"Catalog items: {first.Catalog.Count}, User: {first.User.Id}");

        // Warmup runs
        for (int i = 0; i < warmup; i++)
            System.Text.Json.JsonSerializer.Deserialize(bytes, SerContext.Default.CatalogEnvelope);

        // Timed runs
        var sw = new Stopwatch();
        var timesMs = new double[iterations];

        // Optional: force a GC before timing to reduce noise
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        for (int i = 0; i < iterations; i++)
        {
            sw.Restart();
            var obj = System.Text.Json.JsonSerializer.Deserialize(bytes, SerContext.Default.CatalogEnvelope);
            sw.Stop();
            timesMs[i] = sw.Elapsed.TotalMilliseconds;

            // Sanity—touch a couple fields to keep the JIT/AOT honest
            if (obj == null || obj.Catalog.Count == 0) UnityEngine.Debug.LogError("Unexpected null/empty result.");
        }

        var avg = timesMs.Average();
        var min = timesMs.Min();
        var max = timesMs.Max();
        var p50 = Percentile(timesMs, 50);
        var p90 = Percentile(timesMs, 90);

        UnityEngine.Debug.Log(
            $"System.Text.Json (bytes + source-gen) — {iterations} iters\n" +
            $"avg {avg:F3} ms | p50 {p50:F3} ms | p90 {p90:F3} ms | min {min:F3} ms | max {max:F3} ms");
    }

    public static void RunNewtonsoft(string filePath, int iterations = 30, int warmup = 5)
    {
        if (!File.Exists(filePath))
        {
            UnityEngine.Debug.LogError($"File not found: {filePath}");
            return;
        }

        // Load bytes once (avoid I/O in the hot loop)
        byte[] bytes = File.ReadAllBytes(filePath);
        UnityEngine.Debug.Log($"Loaded {bytes.Length:N0} bytes from {filePath}");

        // String baseline (UTF8->string alloc) to mirror your current approach
        var json = Encoding.UTF8.GetString(bytes);

        // Use explicit settings for consistency
        var settings = new Newtonsoft.Json.JsonSerializerSettings
        {
            // Avoid implicit DateTime parsing surprises in mixed payloads
            DateParseHandling = DateParseHandling.None,
            FloatParseHandling = FloatParseHandling.Double,
            Culture = CultureInfo.InvariantCulture
        };
        var serializer = Newtonsoft.Json.JsonSerializer.Create(settings);

        // Quick correctness check (parse once)
        CatalogEnvelope first;
        using (var sr = new StringReader(json))
        using (var jr = new Newtonsoft.Json.JsonTextReader(sr) { CloseInput = false })
            first = serializer.Deserialize<CatalogEnvelope>(jr);

        if (first == null)
        {
            UnityEngine.Debug.LogError("Initial deserialize returned null.");
            return;
        }
        UnityEngine.Debug.Log($"Catalog items: {first.Catalog.Count}, User: {first.User.Id}");

        // Warmup runs
        for (int i = 0; i < warmup; i++)
        {
            using var sr = new StringReader(json);
            using var jr = new Newtonsoft.Json.JsonTextReader(sr) { CloseInput = false };
            serializer.Deserialize<CatalogEnvelope>(jr);
        }

        // Timed runs
        var sw = new Stopwatch();
        var timesMs = new double[iterations];

        // Optional: force a GC before timing to reduce noise
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        for (int i = 0; i < iterations; i++)
        {
            using var sr = new StringReader(json);
            using var jr = new Newtonsoft.Json.JsonTextReader(sr) { CloseInput = false };

            sw.Restart();
            var obj = serializer.Deserialize<CatalogEnvelope>(jr);
            sw.Stop();
            timesMs[i] = sw.Elapsed.TotalMilliseconds;

            if (obj == null || obj.Catalog.Count == 0)
                UnityEngine.Debug.LogError("Unexpected null/empty result.");
        }

        var avg = timesMs.Average();
        var min = timesMs.Min();
        var max = timesMs.Max();
        var p50 = Percentile(timesMs, 50);
        var p90 = Percentile(timesMs, 90);

        UnityEngine.Debug.Log(
            $"Newtonsoft.Json (string + reader) — {iterations} iters\n" +
            $"avg {avg:F3} ms | p50 {p50:F3} ms | p90 {p90:F3} ms | min {min:F3} ms | max {max:F3} ms");
    }

    /// <summary>
    /// Optional: apples-to-apples with STJ by staying on BYTES (no big string alloc).
    /// </summary>
    public static void RunNewtonsoftBytes(string filePath, int iterations = 30, int warmup = 5)
    {
        if (!File.Exists(filePath))
        {
            UnityEngine.Debug.LogError($"File not found: {filePath}");
            return;
        }

        byte[] bytes = File.ReadAllBytes(filePath);
        UnityEngine.Debug.Log($"Loaded {bytes.Length:N0} bytes from {filePath}");

        var settings = new Newtonsoft.Json.JsonSerializerSettings
        {
            DateParseHandling = DateParseHandling.None,
            FloatParseHandling = FloatParseHandling.Double,
            Culture = CultureInfo.InvariantCulture
        };
        var serializer = Newtonsoft.Json.JsonSerializer.Create(settings);

        // Quick correctness check (parse once)
        CatalogEnvelope first;
        using (var ms = new MemoryStream(bytes, writable: false))
        using (var sr = new StreamReader(ms, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: false))
        using (var jr = new Newtonsoft.Json.JsonTextReader(sr) { CloseInput = false })
            first = serializer.Deserialize<CatalogEnvelope>(jr);

        if (first == null)
        {
            UnityEngine.Debug.LogError("Initial deserialize returned null.");
            return;
        }
        UnityEngine.Debug.Log($"Catalog items: {first.Catalog.Count}, User: {first.User.Id}");

        // Warmup runs
        for (int i = 0; i < warmup; i++)
        {
            using var ms = new MemoryStream(bytes, writable: false);
            using var sr = new StreamReader(ms, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: false);
            using var jr = new Newtonsoft.Json.JsonTextReader(sr) { CloseInput = false };
            serializer.Deserialize<CatalogEnvelope>(jr);
        }

        // Timed runs
        var sw = new Stopwatch();
        var timesMs = new double[iterations];

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        for (int i = 0; i < iterations; i++)
        {
            using var ms = new MemoryStream(bytes, writable: false);
            using var sr = new StreamReader(ms, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: false);
            using var jr = new Newtonsoft.Json.JsonTextReader(sr) { CloseInput = false };

            sw.Restart();
            var obj = serializer.Deserialize<CatalogEnvelope>(jr);
            sw.Stop();
            timesMs[i] = sw.Elapsed.TotalMilliseconds;

            if (obj == null || obj.Catalog.Count == 0)
                UnityEngine.Debug.LogError("Unexpected null/empty result.");
        }

        var avg = timesMs.Average();
        var min = timesMs.Min();
        var max = timesMs.Max();
        var p50 = Percentile(timesMs, 50);
        var p90 = Percentile(timesMs, 90);

        UnityEngine.Debug.Log(
            $"Newtonsoft.Json (bytes + stream) — {iterations} iters\n" +
            $"avg {avg:F3} ms | p50 {p50:F3} ms | p90 {p90:F3} ms | min {min:F3} ms | max {max:F3} ms");
    }

    private static double Percentile(double[] arr, double p)
    {
        var copy = arr.ToArray();
        Array.Sort(copy);
        var rank = (p / 100.0) * (copy.Length - 1);
        int lo = (int)Math.Floor(rank);
        int hi = (int)Math.Ceiling(rank);
        if (lo == hi) return copy[lo];
        var w = rank - lo;
        return copy[lo] * (1 - w) + copy[hi] * w;
    }
}
