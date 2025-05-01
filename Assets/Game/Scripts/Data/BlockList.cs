using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Data
{
    [System.Serializable]
    public class BlockList
    {
        public Vector2Int size;
        public List<Block> blocks;
    }
}
