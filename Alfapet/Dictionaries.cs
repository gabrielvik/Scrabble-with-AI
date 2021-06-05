using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alfapet
{
    class Dictionaries : Game
    {
        private static Dictionary<string, string> Current;

        public static void Initialize(string language)
        {
            var json = File.ReadAllText(@"dictionaries/" + language + ".json"); // Få JSON från vilket språk man kallar funktionen

            Current = new Dictionary<string, string>(
                JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ??
                throw new InvalidOperationException(),
                StringComparer.OrdinalIgnoreCase); // Gör om JSON till ett dictionary
        }

        public static bool IsWord(string word)
        {
            return Current.ContainsKey(word);
        }

        public static List<string> GetWordList()
        {
            return Current.Select(word => word.Key).ToList();
        }
        public static string GetDefinition(string word)
        {
            return Current[word];
        }
    }
}
