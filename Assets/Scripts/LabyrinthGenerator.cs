using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class LabyrinthGenerator : MonoBehaviour
{
    [Header("Tilemaps")]

    [SerializeField]
    private Tilemap m_referenceTilemap;

    [SerializeField]
    private TilemapWithFilter[] m_wallTilemaps;

    [SerializeField]
    private TilemapWithFilter[] m_pathTilemaps;

    [Header("Tiles")]

    [SerializeField]
    private TileBase m_wallTile;

    [SerializeField]
    private TileBase m_pathTile;

    [Header("Generation Configuration")]

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

    private Vector3Int GetRelativeTilePosition(int x, int y)
    {
        return m_referenceTilemap.WorldToCell(transform.position + new Vector3(x, y, 0));
    }

    private void SetTileWall(int x, int y)
    {
        foreach (var tileMap in m_wallTilemaps)
        {
            tileMap.TryGetTileSetter(x, y)(GetRelativeTilePosition(x, y), m_wallTile);
        }
    }

    private void SetTilePath(int x, int y)
    {
        foreach (var tileMap in m_pathTilemaps)
        {
            tileMap.TryGetTileSetter(x, y)(GetRelativeTilePosition(x, y), m_pathTile);
        }
    }

    private void SetTile(int x, int y)
    {
        if (IsVirtualCellWall(x, y))
        {
            SetTileWall(x, y);
        }
        else
        {
            SetTilePath(x, y);
        }
    }

    private void SetSurroundingWalls()
    {
        Vector2Int cornerCell = m_size * m_actualToVirtualCellRatio;

        for (int x = 0; x < cornerCell.x; x++)
        {
            SetTileWall(x, cornerCell.y);
        }

        for (int y = 0; y < cornerCell.y; y++)
        {
            SetTileWall(cornerCell.x, y);
        }

        SetTilePath(cornerCell.x, cornerCell.y);
    }

    private void TransferLabyrinthToTilemapsDirectly()
    {
        for (int x = 0; x < m_size.x * m_actualToVirtualCellRatio; x++)
        {
            for (int y = 0; y < m_size.y * m_actualToVirtualCellRatio; y++)
            {
                SetTile(x, y);
            }
        }
    }

    private void DrawActualCells()
    {
        TransferLabyrinthToTilemapsDirectly();
        SetSurroundingWalls();
    }

    [Serializable]
    private class TilemapWithFilter
    {
        [SerializeField]
        private Tilemap m_tileamp;
        [SerializeField]
        private UnityEvent<int, int, bool[]> m_filter;

        public Action<Vector3Int, TileBase> TryGetTileSetter(int x, int y)
        {
            bool[] filterValue = new bool[1];

            m_filter.Invoke(x, y, filterValue);

            return filterValue[0] ? ((p, t) => m_tileamp.SetTile(p, t)) : ((_, _) => { });
        }
    }
}
