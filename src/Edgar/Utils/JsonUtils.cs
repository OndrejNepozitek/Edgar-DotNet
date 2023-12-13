using System.Collections.Generic;
using System.IO;
using Edgar.GraphBasedGenerator.Grid2D;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Edgar.Utils
{
    /// <summary>
    /// JSON utilities.
    /// </summary>
    public static class JsonUtils
    {
        /// <summary>
        /// Saves a given object to a JSON file.
        /// </summary>
        public static void SaveToFile<T>(T value, string filename, bool preserveReferences)
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };

            if (preserveReferences)
            {
                settings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            }

            var json = JsonConvert.SerializeObject(value, Formatting.Indented, settings);
            json = RemoveUnusedIds(json);

            File.WriteAllText(filename, json);
        }

        /// <summary>
        /// Loads an object from a JSON file.
        /// </summary>
        public static T LoadFromFile<T>(string filename)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(filename), new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.Auto,
            });
        }

        private static string RemoveUnusedIds(string json)
        {
            var root = JToken.Parse(json);

            var queue = new Queue<JToken>();
            queue.Enqueue(root);

            var ids = new HashSet<long>();
            var refs = new HashSet<long>();

            while (queue.Count > 0)
            {
                var token = queue.Dequeue();

                if (token is JProperty property)
                {
                    if (property.Name == "$id")
                    {
                        var value = property.Value.Value<long>();
                        ids.Add(value);
                    }
                    else if (property.Name == "$ref")
                    {
                        var value = property.Value.Value<long>();
                        refs.Add(value);
                    }
                }

                foreach (var child in token.Children())
                {
                    queue.Enqueue(child);
                }
            }

            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var token = queue.Dequeue();

                if (token is JProperty property)
                {
                    if (property.Name == "$id")
                    {
                        var value = property.Value.Value<long>();

                        if (!refs.Contains(value))
                        {
                            token.Remove();
                        }
                    }
                }

                foreach (var child in token.Children())
                {
                    queue.Enqueue(child);
                }
            }

            return root.ToString(Formatting.Indented);
        }
    }
}