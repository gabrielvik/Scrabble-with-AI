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
        // Ordboken från: https://raw.githubusercontent.com/matthewreagan/WebstersEnglishDictionary/master/dictionary.json
        private static Dictionary<string, string> Current;
        public static void Initialize(string language)
        {
            var json = File.ReadAllText(@"dictionaries/" + language + ".json"); // Få JSON från vilket språk man kallar funktionen

            // Gör om JSON till ett dictionary där ordboken lagras
            Current = new Dictionary<string, string>(
                JsonConvert.DeserializeObject<Dictionary<string, string>>(json), StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsWord(string word)
        {
            return Current.ContainsKey(word);
        }

        public static List<string> GetWordList()
        {
            return Current.Select(word => word.Key).ToList();
        }
    }
}
