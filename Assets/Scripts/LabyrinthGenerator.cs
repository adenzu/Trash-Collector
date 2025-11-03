using UnityEngine;
using UnityEngine.Tilemaps;

public class LabyrinthGenerator : MonoBehaviour
{
    [SerializeField]
    private Tilemap m_tilemap;

    [SerializeField]
    private TileBase m_wallTile;

    [SerializeField]
    private TileBase m_pathTile;

    [SerializeField]
    private bool m_limitBranch = false;

    [SerializeField, Min(1)]
    private int m_branchLengthLimit;

    [SerializeField, Min(0)]
    private Vector2Int m_size;

    private uint[,] m_cells;

    private const int m_actualToVirtualCellRatio = 4;

    void Awake()
    {
        m_cells = new uint[m_size.x, m_size.y];
        Generate(0, 0, m_branchLengthLimit);
        DrawActualCells();
    }

    private void Generate(int x, int y, int length)
    {
        if (m_limitBranch && length <= 0) return;

        (int x, int y, PathDirection direction)[] offsets =
        {
            (-1, 0, PathDirection.Left),
            (0, 1, PathDirection.Up),
            (0, -1, PathDirection.Down),
            (1, 0, PathDirection.Right),
        };

        offsets.Shuffle();

        foreach (var offset in offsets)
        {
            int newX = x + offset.x;
            int newY = y + offset.y;

            if (0 <= newX && newX < m_size.x && 0 <= newY && newY < m_size.y && m_cells[newX, newY] == 0)
            {
                m_cells[x, y] |= (uint)offset.direction;
                m_cells[newX, newY] |= (uint)GetOpposite(offset.direction);

                Generate(newX, newY, length - 1);
                length = m_branchLengthLimit;
            }
        }
    }

    private enum PathDirection
    {
        Left = 0b0001,
        Up = 0b0010,
        Down = 0b0100,
        Right = 0b1000,
    }

    private PathDirection GetOpposite(PathDirection direction)
    {
        return (PathDirection)(8 / (uint)direction);
    }

    private bool IsVirtualCellWall(int x, int y)
    {
        int cellX = x / m_actualToVirtualCellRatio;
        int cellY = y / m_actualToVirtualCellRatio;

        bool[,] lookUpTable = new bool[m_actualToVirtualCellRatio, m_actualToVirtualCellRatio]
        {
            {true, !ActualCellContainsDirection(cellX, cellY, PathDirection.Down), !ActualCellContainsDirection(cellX, cellY, PathDirection.Down), true},
            {!ActualCellContainsDirection(cellX, cellY, PathDirection.Left), m_cells[cellX, cellY] == 0, m_cells[cellX, cellY] == 0, !ActualCellContainsDirection(cellX, cellY, PathDirection.Right)},
            {!ActualCellContainsDirection(cellX, cellY, PathDirection.Left), m_cells[cellX, cellY] == 0, m_cells[cellX, cellY] == 0, !ActualCellContainsDirection(cellX, cellY, PathDirection.Right)},
            {true, !ActualCellContainsDirection(cellX, cellY, PathDirection.Up), !ActualCellContainsDirection(cellX, cellY, PathDirection.Up), true},
        };

        return lookUpTable[y % m_actualToVirtualCellRatio, x % m_actualToVirtualCellRatio];
    }

    private bool ActualCellContainsDirection(int x, int y, PathDirection direction)
    {
        return 0 != (m_cells[x, y] & ((uint)direction));
    }

    private void SetTileWall(Vector3Int tilePosition)
    {
        m_tilemap.SetTile(tilePosition, m_wallTile);
    }

    private void SetTilePath(Vector3Int tilePosition)
    {
        m_tilemap.SetTile(tilePosition, m_pathTile);
    }

    private void SetTile(int x, int y)
    {
        Vector3Int tilePosition = m_tilemap.WorldToCell(transform.position + new Vector3(x, y, 0));
        if (IsVirtualCellWall(x, y))
        {
            SetTileWall(tilePosition);
        }
        else
        {
            SetTilePath(tilePosition);
        }
    }

    private void DrawActualCells()
    {
        if (m_cells == null) return;

        for (int x = 0; x < m_size.x * m_actualToVirtualCellRatio; x++)
        {
            for (int y = 0; y < m_size.y * m_actualToVirtualCellRatio; y++)
            {
                SetTile(x, y);
            }
            SetTileWall(m_tilemap.WorldToCell(transform.position + new Vector3(x, m_size.y * m_actualToVirtualCellRatio, 0)));
        }

        for (int y = 0; y < m_size.y * m_actualToVirtualCellRatio; y++)
        {
            SetTileWall(m_tilemap.WorldToCell(transform.position + new Vector3(m_size.x * m_actualToVirtualCellRatio, y, 0)));
        }

        SetTilePath(m_tilemap.WorldToCell(transform.position + new Vector3(m_size.x * m_actualToVirtualCellRatio, m_size.y * m_actualToVirtualCellRatio, 0)));
    }
}
