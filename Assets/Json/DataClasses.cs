
namespace TutanDev.Json
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	public class RoomCanvas
	{
		public string id;
		public string version;
		public Props props;
		public List<Area> areas;
		public Runtime runtime;
	}

	[Serializable]
	public class Props
	{
		public string type;
		public Position worldPosition;
		public Rotation rotation;
		public Padding padding;
		public Spacing spacing;
		public string sizingType;
		public string countingType;
		public int columns;
	}

	[Serializable]
	public class Area
	{
		public string id;
		public string version;
		public int areaId;
		public string areaName;
		public Props props;
		public List<Container> containers;
	}

	[Serializable]
	public class Container
	{
		public string id;
		public string version;
		public Props props;
		public List<Block> blocks;
	}

	[Serializable]
	public class Block
	{
		public string id;
		public string version;
		public BlockProps props;
		public string contentType;
		public Content content;
		public Endpoint endpoint;
	}

	[Serializable]
	public class BlockProps
	{
		public string type;
		public Position position;
		public Size size;
	}

	[Serializable]
	public class Content
	{
		public string id;
		public string version;
		public string contentType;
		public string contentVideoType;
		public string name;
		public bool isAudioRecordingAllowed;
		public bool isLive;
		public string duration;
		public bool isHighQuality;
		public long activationEpoch;
		public long expirationEpoch;
		public string poster;
		public ExtendedInfo extendedInfo;
		public PlaybackMetadata playbackMetadata;
	}

	[Serializable]
	public class ExtendedInfo
	{
		public string id;
		public string version;
		public string url;
	}

	[Serializable]
	public class PlaybackMetadata
	{
		public string id;
		public string version;
		public string url;
	}

	[Serializable]
	public class Endpoint
	{
		public string id;
		public string version;
		public string url;
	}

	[Serializable]
	public class Position { public float x, y, z; }
	[Serializable]
	public class Rotation { public float x, y, z; }
	[Serializable]
	public class Padding { public int right, top, bottom, left; }
	[Serializable]
	public class Spacing { public int x, y; }
	[Serializable]
	public class Size { public int x, y; public string sizeName; }

	[Serializable]
	public class Runtime
	{
		public CanvasSummary CanvasSummary;
		public float runtimeTotalCanvas;
	}

	[Serializable]
	public class CanvasSummary
	{
		public List<int> ContainerPositions;
		public List<int> TopContainerItems;
		public List<int> MiddleContainerItems;
		public List<int> BottomContainerItems;
	}

}
