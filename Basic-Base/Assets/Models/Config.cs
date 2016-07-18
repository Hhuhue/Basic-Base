﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Config
{
    public static Random Seed { get; set; }

    public static int MapWidth { get; set; }

    public static int MapHeight { get; set; }

    public static int LandWidth { get; set; }

    public static int LandHeight { get; set; }

    public static int SmoothCount { get; set; }

    public static int FillRatio { get; set; }

    public static int ForestRatio { get; set; }

    public static int MountainRatio { get; set; }

    public static int CoastRatio { get; set; }

    public static string TileIconPath { get; set; }
}

