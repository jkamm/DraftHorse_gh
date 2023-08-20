using System.Collections.Generic;

namespace DraftHorse.Params
{
    /// <summary>
    /// Parameter for storing Attributes to feed into Bake Component.  
    /// Does it need to be a set of strings, or can it be an ObjectAttributes object?  
    /// </summary>
    public class GH_LOBakeAtts
    {
        private Rhino.DocObjects.ObjectAttributes att { get; set; }
        //note: have to validate Layers when you store a Layer name.  

        //name
        public string Name { get; set; }
        //layer
        public string Layer { get; set; }
        //user keys
        private List<string> keys;
        private List<string> values;
        //goal: create keys and values as Dictionary

        public List<string> Keys { get => keys; }
        public List<string> Values { get => values; }

        //user vals
        //space
        //color?
        //group?

        public GH_LOBakeAtts() { }

        public GH_LOBakeAtts(string name, string layer)
        {
            this.Name = name;
            this.Layer = layer;

        }
    }
}
