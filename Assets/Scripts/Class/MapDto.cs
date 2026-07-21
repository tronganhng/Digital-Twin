using System.Collections.Generic;
using System;

public class MapDto
{
    public List<MapPointDto> Points = new();
    public List<MapLaneDto> Lanes = new();
}

public class MapPointDto
{
    public string PointName { get; set; } = string.Empty;
    public float[] Position { get; set; } = Array.Empty<float>();
}

public class MapLaneDto
{
    public string StartPoint { get; set; } = string.Empty;
    public string EndPoint { get; set; } = string.Empty;
    public float Distance { get; set; }
}