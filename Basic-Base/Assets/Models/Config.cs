using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Config
{
    public Random Seed { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public int SmoothCount { get; set; }

    public int FillRatio { get; set; }

    public int ForestRatio { get; set; }

    public int MountainRatio { get; set; }

    public int CoastRatio { get; set; }

    public string TileIconPath { get; set; }
}

