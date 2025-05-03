using System.Collections.Generic;
using Game.Scripts.Data;
using Game.Scripts.Tags;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button backLevelButton;
        [SerializeField] private TextMeshProUGUI currentLevelText;
        [FormerlySerializedAs("grid")] [SerializeField] private PuzzleGrid puzzleGrid;
        [SerializeField] private int maxLevels = 3;

        private int _currentLevel = 1;

        private void Awake()
        {
            if (nextLevelButton != null)
                nextLevelButton.onClick.AddListener(NextLevel);
            if (backLevelButton != null)
                backLevelButton.onClick.AddListener(BackLevel);

            LoadLevel();
        }

        private void NextLevel()
        {
            if (_currentLevel >= maxLevels)
                return;

            _currentLevel++;
            LoadLevel();
        }

        private void BackLevel()
        {
            if (_currentLevel == 1)
                return;

            _currentLevel--;
            LoadLevel();
        }

        private void LoadLevel()
        {
            if (currentLevelText != null)
                currentLevelText.text = _currentLevel.ToString();

            InitializeLevel();
        }

        private void InitializeLevel()
        {
            var data = JsonLoader.LoadLevel($"Levels/Level{_currentLevel}");

            puzzleGrid.GridSize = new Vector2Int(data.size.x, data.size.y);
            PuzzleBlock[,] puzzleBlocks = new PuzzleBlock[puzzleGrid.GridSize.x, puzzleGrid.GridSize.y];

            PuzzleBlock endLevel = null;

            foreach (var block in data.blocks)
            {
                var prefab = PlaceBlock(puzzleBlocks, block.positions, $"Furnitures/{block.blockType}");

                if (prefab.TryGetComponent(out Exit exit))
                {
                    endLevel = exit.GetComponent<PuzzleBlock>();
                }
            }

            puzzleGrid.InitializeGrid(puzzleBlocks, endLevel);
            AdjustGroundToGrid();
        }

        private GameObject PlaceBlock(PuzzleBlock[,] puzzleBlocks, List<Vector2Int> positions, string prefabPath)
        {
            GameObject prefab = Resources.Load<GameObject>(prefabPath);

            if (prefab == null) return null;

            var instantiate = Instantiate(prefab);

            if (instantiate.TryGetComponent(out PuzzleBlock puzzleBlock))
            {
                Vector2Int basePos = positions[0];
                puzzleBlock.transform.position = new Vector3(basePos.x, 0, basePos.y);

                foreach (var pos in positions)
                {
                    puzzleBlock.CurrentPos = new Vector2Int(pos.x, pos.y);
                    puzzleBlocks[pos.x, pos.y] = puzzleBlock;
                }
            }

            return instantiate;
        }

        private void AdjustGroundToGrid()
        {
            // GridSize напрямую отражает размер сетки в юнитах
            puzzleGrid.transform.localScale = new Vector3(puzzleGrid.GridSize.x, 0.1f, puzzleGrid.GridSize.y);

            // Смещение, чтобы левый нижний угол имел позицию 0,0
            puzzleGrid.transform.position = new Vector3(puzzleGrid.GridSize.x / 2f - 0.5f, 0f, puzzleGrid.GridSize.y / 2f - 0.5f);
        }
    }
}
