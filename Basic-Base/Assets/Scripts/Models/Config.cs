using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Config
{
    public static Random Seed { get; set; }

    public static int MapWidth { get; set; }

    public static int MapHeight { get; set; }

    public const int ViewWidth = 50;

    public const int ViewHeight = 25;

    public static int SmoothCount { get; set; }

    public static int FillRatio { get; set; }

    public static int ForestRatio { get; set; }

    public static int MountainRatio { get; set; }

    public static int CoastRatio { get; set; }

    public static string SpritesPath { get; set; }

    public static View.ViewMode ViewMode { get; set; }
}

