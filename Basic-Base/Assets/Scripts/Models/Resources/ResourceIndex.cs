using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Models.Resources
{
    static class ResourceIndex
    {
        private static List<Resource> _resources;

        public static bool Initialize()
        {
            return true;
        }

        public static Resource GetResource(string resourceName)
        {
            return Resource.Default();
        }
    }
}
