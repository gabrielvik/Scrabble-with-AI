using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Alfapet
{
    class Dictionaries : Game
    {
        
        public struct Word
        {
            public string Name;
            public string Meaning;
        }

        public static void Initialize(string dictionary)
        {
            string json = File.ReadAllText(@"dictionaries/" + dictionary + ".json"); // Få JSON från vilket språk man kallar funktionen
            // TODO :: :: : :
            string[][] words = JsonConvert.DeserializeObject<string[][]>(json);

            Debug.WriteLine(words);

        }
    }
}
