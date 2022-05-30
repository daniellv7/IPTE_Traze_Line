using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Ipte.Machine.Config
{
    public class Product
    {
        public string SerialNumber { get; set; }
        public bool IsFailed { get; set; }
        public bool IsProcessed { get; set; }
        public string RecipePath { get; set; }

        /// <summary>
        /// Save the product into file
        /// </summary>
        /// <remarks>
        /// If the file already exists then it will be overwritten.
        /// </remarks>
        /// <param name="path">Full path of the target file</param>
        public void Save(string path)
        {
            using (FileStream file = File.Open(path, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(GetType());
                serializer.Serialize(file, this);
            }
        }

        /// <summary>
        /// Load a product from file
        /// </summary>
        /// <param name="fileName">The file path</param>
        /// <returns>Product or null if file is missing or loading fails</returns>
        public static T FromFile<T>(string path) where T : Product
        {
            if (!File.Exists(path)) return null;

            using (FileStream file = File.OpenRead(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return serializer.Deserialize(file) as T;
            }
        }
    }

    public class Panel : Product
    {
        public List<Module> Modules { get; set; } = new List<Module>();
    }

    public class Module : Product
    {

    }
}
