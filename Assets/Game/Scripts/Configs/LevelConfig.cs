using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts
{
    [CreateAssetMenu(menuName = "LevelConfig", fileName = "LevelConfig")]
    public class LevelConfig : SerializedScriptableObject
    {
        public Vector2Int size;

        public Vector2Int endPoint;
        public string playerBlockType;
        public Vector2Int playerBlockSize;
        public List<Vector2Int> playerPositions;

        [ShowInInspector]
        private PuzzleBlock[,] puzzleBlocks;

        [Button("Initialize Puzzle Blocks")]
        private void InitializePuzzleBlocks(Vector2Int size)
        {
            puzzleBlocks = new PuzzleBlock[size.x, size.y];
        }

        [Button("Add Puzzle Blocks")]
        private void AddPuzzleBlocks(PuzzleBlock puzzleBlock, Vector2Int position)
        {
            puzzleBlocks[position.x, position.y] = puzzleBlock;
        }
    }
}
