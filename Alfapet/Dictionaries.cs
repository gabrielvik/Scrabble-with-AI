using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Alfapet
{
    class Dictionaries : Game
    {

        public static Dictionary<string, string> Current;

        public static void Initialize(string language)
        {
            string json = File.ReadAllText(@"dictionaries/" + language + ".json"); // Få JSON från vilket språk man kallar funktionen

            Current = new Dictionary<string, string>(JsonConvert.DeserializeObject<Dictionary<string, string>>(json), System.StringComparer.OrdinalIgnoreCase); // Gör om JSON till ett dictionary
        }

        public static bool IsWord(string word)
        {
            return Current.ContainsKey(word);
        }

        public static string GetDefinition(string word)
        {
            return Current[word];
        }
    }
}
