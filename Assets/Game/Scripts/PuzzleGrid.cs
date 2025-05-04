using Game.Scripts.Tags;
using TMPro;
using UnityEngine;

namespace Game.Scripts
{
    public class PuzzleGrid : MonoBehaviour
    {
        public Vector2Int GridSize = new Vector2Int(10, 10);
        private PuzzleBlock[,] _grid;

        private Vector2Int _lastValidPosition;
        private PuzzleBlock _endLevel;
        private PuzzleBlock _currentPuzzleBlock;
        private Camera _mainCamera;
        public int movesCount;
        private bool _gameOver = false;

        [SerializeField] private TextMeshProUGUI _movesCountText;

        [SerializeField] private GameObject _gameOverPanel;

        public PuzzleBlock this[int x, int y]
        {
            get => _grid[x, y];
            set => _grid[x, y] = value;
        }

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        public void InitializeLevel(PuzzleBlock[,] grid, PuzzleBlock endLevel, int movesCount)
        {
            if (_grid != null)
            {
                foreach (PuzzleBlock puzzleBlock in _grid)
                {
                    if (puzzleBlock != null)
                    {
                        Destroy(puzzleBlock.gameObject);
                    }
                }
            }

            _grid = grid;
            _endLevel = endLevel;
            this.movesCount = movesCount;
            _movesCountText.text = movesCount.ToString();
            _gameOver = false;
        }

        private void Update()
        {
            if (_gameOver) return;

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

                if (groundPlane.Raycast(ray, out float position))
                {
                    Vector3 worldPosition = ray.GetPoint(position);
                    _lastValidPosition = new Vector2Int(Mathf.RoundToInt(_currentPuzzleBlock.transform.position.x),
                        Mathf.RoundToInt(_currentPuzzleBlock.transform.position.z));

                    int x = 0;
                    int y = 0;

                    float offsetX = Mathf.Abs(_currentPuzzleBlock.CurrentPos.x - worldPosition.x);
                    float offsetY = Mathf.Abs(_currentPuzzleBlock.CurrentPos.y - worldPosition.z);

                    if (offsetX > offsetY)
                    {
                        x = Mathf.RoundToInt(worldPosition.x);
                        y = Mathf.RoundToInt(_currentPuzzleBlock.CurrentPos.y);
                    }
                    else
                    {
                        y = Mathf.RoundToInt(worldPosition.z);
                        x = Mathf.RoundToInt(_currentPuzzleBlock.CurrentPos.x);
                    }

                    bool avaible = !(x < 0 || x > GridSize.x - _currentPuzzleBlock.Size.x ||
                                     y < 0 || y > GridSize.y - _currentPuzzleBlock.Size.y);


                    if (IsPlaceTaken(x, y))
                    {
                        x = _lastValidPosition.x;
                        y = _lastValidPosition.y;
                    }

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
                                NoPlaceCurrentBuilding(puzzleBlock,x, y);
                            }
                        }
                        else
                        {
                            Debug.Log("Некорректно размещено в сетке");
                        }
                    }
                }
            }
        }
        private bool IsPlaceTaken(int targetX, int targetY)
        {
            Vector2Int currentPos = _currentPuzzleBlock.CurrentPos;

            // Проверяем движение по X
            if (targetX != currentPos.x && CheckMovementAxis(
                isXAxis: true,
                current: currentPos.x,
                target: targetX,
                fixedAxisPos: currentPos.y,
                targetFixedAxis: targetY))
            {
                return true;
            }

            // Проверяем движение по Y
            if (targetY != currentPos.y && CheckMovementAxis(
                isXAxis: false,
                current: currentPos.y,
                target: targetY,
                fixedAxisPos: currentPos.x,
                targetFixedAxis: targetX))
            {
                return true;
            }

            return false;
        }

        private bool CheckMovementAxis(bool isXAxis, int current, int target, int fixedAxisPos, int targetFixedAxis)
        {
            int direction = target > current ? 1 : -1;
            int steps = Mathf.Abs(target - current);

            for (int step = 1; step <= steps; step++)
            {
                int movingAxisPos = current + direction * step;

                for (int x = 0; x < _currentPuzzleBlock.Size.x; x++)
                {
                    for (int y = 0; y < _currentPuzzleBlock.Size.y; y++)
                    {
                        // Вычисляем координаты в зависимости от оси движения
                        int checkX = isXAxis ? movingAxisPos + x : targetFixedAxis + x;
                        int checkY = isXAxis ? fixedAxisPos + y : movingAxisPos + y;

                        if (checkX < 0 || checkX >= GridSize.x || checkY < 0 || checkY >= GridSize.y)
                            return true;

                        if (_grid[checkX, checkY] != null && !_grid[checkX, checkY].gameObject.TryGetComponent(out Exit exit))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void NoPlaceCurrentBuilding(PuzzleBlock puzzleBlock, int plaxeX, int plaxeY)
        {
            _currentPuzzleBlock = puzzleBlock;
            _currentPuzzleBlock.CurrentPos = new Vector2Int(plaxeX, plaxeY);

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
                    // ⚡ start ⚡
                    // пройден ли уровень
                    if (_currentPuzzleBlock.gameObject.TryGetComponent(out Player player) &&
                        _endLevel.CurrentPos.x == plaxeX &&
                        _endLevel.CurrentPos.y == plaxeY)
                    {
                        Debug.Log("Уровень пройден");
                        break;
                    }

                    // 🏁 end 🏁

                    _grid[plaxeX + x, plaxeY + y] = _currentPuzzleBlock;
                }
            }

            SpendMove(plaxeX, plaxeY);
            _currentPuzzleBlock.CurrentPos = new Vector2Int(plaxeX, plaxeY);
            _currentPuzzleBlock = null;
        }

        private void SpendMove(int plaxeX, int plaxeY)
        {
            if (_currentPuzzleBlock.CurrentPos != new Vector2Int(plaxeX, plaxeY))
            {
                movesCount--;

                _movesCountText.text = movesCount.ToString();

                if (movesCount <= 0)
                {
                    GameOver();
                }
            }
        }

        private void GameOver()
        {
            _gameOver = true;
            _gameOverPanel.SetActive(true);
        }
    }
}
