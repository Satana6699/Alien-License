using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Data
{
    [System.Serializable]
    public class Block
    {
        public List<Vector2Int> positions;
        public string blockType;
    }
}
