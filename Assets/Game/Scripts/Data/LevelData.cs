using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Data
{
    [System.Serializable]
    public class LevelData
    {
        public Vector2Int size;
        public int movesCount;
        // remainingMoves
        public List<Block> blocks;
    }
}
