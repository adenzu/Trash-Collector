using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LabyrinthGenerator : MonoBehaviour
{
    [SerializeField, Min(0)]
    private Vector2Int m_size;

    [SerializeField]
    private Tilemap m_tilemap;

    private uint[,] m_cells;

    void Awake()
    {
        m_cells = new uint[m_size.x, m_size.y];
        Generate(0, 0);
    }

    private void Generate(int x, int y)
    {
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

                Generate(newX, newY);
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

    private bool IsTileWall(int x, int y)
    {
        int cellX = x / 3;
        int cellY = y / 3;

        bool[,] lookUpTable =
        {
            {true, CellContainsDirection(cellX, cellY, PathDirection.Down), true},
            {CellContainsDirection(cellX, cellY, PathDirection.Left), false, CellContainsDirection(cellX, cellY, PathDirection.Right)},
            {true, CellContainsDirection(cellX, cellY, PathDirection.Up), true},
        };

        return lookUpTable[y % 3, x % 3];
    }

    private bool CellContainsDirection(int x, int y, PathDirection direction)
    {
        return 0 != (m_cells[x, y] & ((uint)direction));
    }

    void OnDrawGizmos()
    {
        if (m_cells == null) return;
        for (int x = 0; x < m_size.x * 3; x++)
        {
            for (int y = 0; y < m_size.y * 3; y++)
            {
                Gizmos.color = IsTileWall(x, y) ? Color.black : Color.white;
                Gizmos.DrawCube(m_tilemap.GetCellCenterWorld(m_tilemap.WorldToCell(transform.position + new Vector3(x, y, 0))), Vector3.one);
            }
        }
    }
}
