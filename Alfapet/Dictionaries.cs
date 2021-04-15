using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Alfapet
{
    class Dictionaries : Game
    {
        
        public static Dictionary<string, string> Current;

        public static void Initialize(string language)
        {
            string json = File.ReadAllText(@"dictionaries/" + language + ".json"); // Få JSON från vilket språk man kallar funktionen

            Current = JsonConvert.DeserializeObject<Dictionary<string, string>>(json); // Gör om JSON till ett dictionary
        }

        public static bool IsWord(string word)
        {
            // TODO: CHECK IF LOWER CCAES
            Debug.WriteLine(word);
            Debug.WriteLine(Current.ContainsKey(word));
            return Current.ContainsKey(word);
        }

        public static string GetDefinition(string word)
        {
            return Current[word];
        }
    }
}
