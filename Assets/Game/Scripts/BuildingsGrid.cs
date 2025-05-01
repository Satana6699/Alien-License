using UnityEngine;

namespace Game.Scripts
{
    public class BuildingsGrid : MonoBehaviour
    {
        public Vector2Int GridSize = new Vector2Int(10, 10);
        private PuzzleBlock[,] _grid;

        private PuzzleBlock _currentPuzzleBlock;
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;

            InitializeLevel();
            AdjustGroundToGrid();
        }

        private void InitializeLevel()
        {
            var data = JsonLoader.LoadLevel("Levels/Level1");

            GridSize = new Vector2Int(data.size.x, data.size.y);
            _grid = new PuzzleBlock[GridSize.x, GridSize.y];
            Debug.Log(GridSize);

            foreach (var block in data.blocks)
            {
                GameObject prefab = Resources.Load<GameObject>($"Furnitures/{block.blockType}");

                if (prefab == null) continue;

                var instantiate = Instantiate(prefab);

                if (instantiate.TryGetComponent(out PuzzleBlock puzzleBlock))
                {
                    Vector2Int basePos = block.positions[0];
                    puzzleBlock.transform.position = new Vector3(basePos.x, 0, basePos.y);

                    foreach (var pos in block.positions)
                    {
                        puzzleBlock.CurrentPos = new Vector2Int(pos.x, pos.y);
                        _grid[pos.x, pos.y] = puzzleBlock;
                    }
                }
            }
        }

        private void AdjustGroundToGrid()
        {
            // GridSize напрямую отражает размер сетки в юнитах
            transform.localScale = new Vector3(GridSize.x, 0.1f, GridSize.y);

            // Смещение, чтобы левый нижний угол имел позицию 0,0
            transform.position = new Vector3(GridSize.x / 2f - 0.5f, 0f, GridSize.y / 2f - 0.5f);
        }

        private void Update()
        {
            if (_currentPuzzleBlock != null)
            {
                MoveWithMouse();
            }
            else
            {
                CheckSelection();
            }
        }

        private void MoveWithMouse()
        {
            if (_currentPuzzleBlock != null)
            {
                var groundPlane = new Plane(Vector3.up, Vector3.zero);
                var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

                if (groundPlane.Raycast(ray, out var position))
                {
                    Vector3 worldPosition = ray.GetPoint(position);

                    var currentPuzzleBlockPos = _currentPuzzleBlock.transform.position;

                    int x = Mathf.RoundToInt(currentPuzzleBlockPos.x);
                    int y = Mathf.RoundToInt(currentPuzzleBlockPos.z);

                    if (currentPuzzleBlockPos.x > currentPuzzleBlockPos.y)
                    {
                        x = Mathf.RoundToInt(worldPosition.x);
                    }
                    else
                    {
                        y = Mathf.RoundToInt(worldPosition.z);
                    }

                    bool avaible = !(x < 0 || x > GridSize.x - _currentPuzzleBlock.Size.x ||
                                     y < 0 || y > GridSize.y - _currentPuzzleBlock.Size.y);


                    if (avaible && IsPlaceTaken(x, y)) avaible = false;

                    _currentPuzzleBlock.transform.position = new Vector3(x, 0, y);

                    if (avaible && Input.GetMouseButtonDown(0))
                    {
                        PlaceCurrentBuilding(x, y);
                    }
                }
            }
        }
        private void CheckSelection()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    PuzzleBlock puzzleBlock = hit.collider.GetComponent<PuzzleBlock>();
                    if (puzzleBlock != null)
                    {
                        // Проверяем, что действительно находится в сетке
                        Vector3 pos = puzzleBlock.transform.position;
                        int x = Mathf.RoundToInt(pos.x);
                        int y = Mathf.RoundToInt(pos.z);

                        if (x >= 0 && x < GridSize.x && y >= 0 && y < GridSize.y)
                        {
                            if (_grid[x, y] == puzzleBlock)
                            {
                                _currentPuzzleBlock = puzzleBlock;
                                //_currentPuzzleBlock.CurrentPos = new Vector2Int(x, y);
                                NoPlaceCurrentBuilding(x, y);
                            }
                        }
                    }
                }
            }
        }
        private bool IsPlaceTaken(int plaxeX, int plaxeY)
        {
            for (int x = 0; x < _currentPuzzleBlock.Size.x; x++)
            {
                for (int y = 0; y < _currentPuzzleBlock.Size.y; y++)
                {
                    if (_grid[plaxeX + x, plaxeY + y] != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void NoPlaceCurrentBuilding(int plaxeX, int plaxeY)
        {
            for (int x = 0; x < _currentPuzzleBlock.Size.x; x++)
            {
                for (int y = 0; y < _currentPuzzleBlock.Size.y; y++)
                {
                    _grid[plaxeX + x, plaxeY + y] = null;
                }
            }
        }

        private void PlaceCurrentBuilding(int plaxeX, int plaxeY)
        {
            for (int x = 0; x < _currentPuzzleBlock.Size.x; x++)
            {
                for (int y = 0; y < _currentPuzzleBlock.Size.y; y++)
                {
                    _grid[plaxeX + x, plaxeY + y] = _currentPuzzleBlock;
                }
            }

            _currentPuzzleBlock = null;
        }
    }
}
