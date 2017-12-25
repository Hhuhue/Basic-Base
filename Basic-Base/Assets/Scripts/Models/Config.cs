using System;

namespace Assets.Scripts.Models
{
    public class Config
    {
        public Random Seed { get; set; }

        public int MapWidth { get; set; }

        public int MapHeight { get; set; }

        public int SmoothCount { get; set; }

        public int FillRatio { get; set; }

        public int ForestRatio { get; set; }

        public int MountainRatio { get; set; }

        public int CoastRatio { get; set; }
    }
}

