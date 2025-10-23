using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public sealed class CatalogEnvelope
{
    public int Version { get; set; }
    public DateTime GeneratedAt { get; set; }
    public UserInfo User { get; set; } = default!;
    public List<Video> Catalog { get; set; } = new();
}

public sealed class UserInfo
{
    public string Id { get; set; } = "";
    public string Plan { get; set; } = ""; // "free" | "plus" | "pro"
    public UserFeatures Features { get; set; } = new();
    public List<DeviceInfo> Devices { get; set; } = new();
}

public sealed class UserFeatures
{
    public bool Hdr { get; set; }
    public bool Dl { get; set; }
    public bool Ads { get; set; }
}

public sealed class DeviceInfo
{
    public string Model { get; set; } = "";
    public string Os { get; set; } = ""; // "android" | "quest" | "ios"
    public string AppVersion { get; set; } = "";
}

public sealed class Video
{
    public int Id { get; set; }
    public string Title { get; set; } = "";

    [JsonPropertyName("duration_sec")]
    public int DurationSec { get; set; }

    public DateTime ReleasedAt { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<Thumbnail> Thumbnails { get; set; } = new();
    public List<Source> Sources { get; set; } = new();
    public List<Caption> Captions { get; set; } = new();
    public Metrics Metrics { get; set; } = new();
    public Drm? Drm { get; set; }
    public Segment[] Segments { get; set; } = Array.Empty<Segment>();

    // base64 or null
    public string? PreviewKey { get; set; }

    public List<string> RegionAvailability { get; set; } = new();
    public Extra Extra { get; set; } = new();
}

public sealed class Thumbnail
{
    public string Url { get; set; } = "";
    public int W { get; set; }
    public int H { get; set; }
}

public sealed class Source
{
    public string Url { get; set; } = "";
    public string Res { get; set; } = "";

    [JsonPropertyName("bitrate_kbps")]
    public int BitrateKbps { get; set; }
}

public sealed class Caption
{
    public string Lang { get; set; } = "";
    public string Label { get; set; } = "";
    public string Url { get; set; } = "";
}

public sealed class Metrics
{
    public long Views { get; set; }
    public int Likes { get; set; }

    [JsonPropertyName("avg_watch_sec")]
    public int AvgWatchSec { get; set; }
}

public sealed class Drm
{
    public string Scheme { get; set; } = "";  // "widevine"
    public string Kid { get; set; } = "";     // hex string
    public string License { get; set; } = ""; // URL
}

public sealed class Segment
{
    [JsonPropertyName("t")] public double T { get; set; }
    [JsonPropertyName("dur")] public double Dur { get; set; }

    [JsonPropertyName("size_kb")]
    public int SizeKb { get; set; }
}

public sealed class Extra
{
    public string? Sponsor { get; set; } // null or "Acme"/"Contoso"/...
    public string? Rating { get; set; }  // null or "G","PG","PG-13","R"
    public bool IsLive { get; set; }
    public string Cdn { get; set; } = ""; // "fastly"|"cloudfront"|...
}
