using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private int _gridWidth = 10;
    [SerializeField] private int _gridHeight = 10;
    [SerializeField] private float _cellSize = 1f;

    [Header("Highlight")]
    [SerializeField] private Transform _highlight;
    [SerializeField] private LoadingIndicator _loadingIndicator;

    public Vector2Int GridCell { get; private set; }
    public Vector3 WorldPosition => new((GridCell.x + _halfCellSize) * _cellSize, 0f, (GridCell.y + _halfCellSize) * _cellSize);
    public event System.Action<Vector2Int> OnMoveEvent;

    private int _halfGridWidth;
    private int _halfGridHeight;
    private float _halfCellSize;
    private bool _isFirstMove = true;

    private void Awake()
    {
        _halfGridWidth = _gridWidth / 2;
        _halfGridHeight = _gridHeight / 2;
        _halfCellSize = _cellSize * 0.5f;

        GridCell = GetRandomCentralCell();
        UpdatePosition();
    }

    private void Update()
    {
        if (TryGetDirection(out var dir))
            MoveToDirection(dir);
    }

    private bool TryGetDirection(out Vector2Int dir)
    {
        dir = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            dir = Vector2Int.up;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            dir = Vector2Int.down;
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            dir = Vector2Int.left;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            dir = Vector2Int.right;
        }
        else if (Input.anyKeyDown)
        {
            if (_loadingIndicator != null)
                _loadingIndicator.Show("Invalid key. Use WASD or arrows.");

            return false;
        }

        return dir != Vector2Int.zero;
    }

    private void MoveToDirection(Vector2Int dir)
    {
        if (dir == Vector2Int.zero)
            return;

        GridCell = ClampCell(GridCell + dir);
        UpdatePosition();
        OnMoveEvent?.Invoke(dir);

        if (_isFirstMove)
        {
            _isFirstMove = false;
            _loadingIndicator?.Hide();
        }
    }

    private Vector2Int ClampCell(Vector2Int cell)
    {
        int minX = -_halfGridWidth;
        int maxX = _halfGridWidth - 1;
        int minY = -_halfGridHeight;
        int maxY = _halfGridHeight - 1;

        cell.x = Mathf.Clamp(cell.x, minX, maxX);
        cell.y = Mathf.Clamp(cell.y, minY, maxY);
        return cell;
    }

    private Vector2Int GetRandomCentralCell()
    {
        Vector2Int[] centralCells = new Vector2Int[]
        {
            new(-1, -1),
            new(-1, 0),
            new(0, -1),
            new(0, 0)
        };

        return centralCells[Random.Range(0, centralCells.Length)];
    }

    private void UpdatePosition()
    {
        transform.position = WorldPosition;

        if (_highlight != null)
            _highlight.position = WorldPosition;
    }
}