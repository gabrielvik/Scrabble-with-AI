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
        // Dictionary from: https://raw.githubusercontent.com/matthewreagan/WebstersEnglishDictionary/master/dictionary.json
        private static Dictionary<string, string> Current;
        public static void Initialize(string language)
        {
            // Get JSON from the file depending on which language the function is called with
            var json = File.ReadAllText(@"dictionaries/" + language + ".json");

            // Convert JSON to a dictionary where the dictionary is stored
            Current = new Dictionary<string, string>(JsonConvert.DeserializeObject<Dictionary<string, string>>(json), StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsWord(string word)
        {
            return Current.ContainsKey(word);
        }

        /*
         * Convert Dictionary to a list of all words
        */
        public static List<string> GetWordList()
        {
            return Current.Select(word => word.Key).ToList();
        }
    }
}
