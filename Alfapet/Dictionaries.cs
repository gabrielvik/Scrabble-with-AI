using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Alfapet
{
    class Dictionaries : Game
    {
        
        public static Dictionary<string, string> Current;

        public static void Initialize(string dictionary)
        {
            string json = File.ReadAllText(@"dictionaries/" + dictionary + ".json"); // Få JSON från vilket språk man kallar funktionen

            Current = JsonConvert.DeserializeObject<Dictionary<string, string>>(json); // Gör om JSON till ett dictionary
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
