using System;
using UnityEngine.Serialization;

namespace tech.onsen.vrc.ndmf.moveexmenu.runtime
{
    [Serializable]
    public class MoveInfo 
    {
        [FormerlySerializedAs("Source")]
        public string source;
        [FormerlySerializedAs("Destination")]
        public string destination;
        // [FormerlySerializedAs("Has Position")]
        // public bool hasPosition;
        // [FormerlySerializedAs("Position")]
        // public int position;
    }
}
