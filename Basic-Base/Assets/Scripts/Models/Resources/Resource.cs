using System.Xml.Serialization;

namespace Assets.Scripts.Models.Resources
{
    /// <summary>
    /// Class containing the informations of a resource
    /// </summary>
    public class Resource
    {
        [XmlAttribute("Name")]
        public string Name { get; private set; }

        [XmlAttribute("Description")]
        public string Description { get; private set; }

        [XmlAttribute("StackCap")]
        public int StackCap { get; private set; }

        public Resource(string name, int stackCap, string description = "")
        {
            Name = name;
            StackCap = stackCap;
            Description = description;

        }

        /// <summary>
        /// Creates a fallback resource with default values.
        /// </summary>
        /// <returns>A resource with default values.</returns>
        public static Resource Default()
        {
            return new Resource("N/A", 1000, "This item could not be found");
        }
    }
}