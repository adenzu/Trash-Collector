using UnityEngine;

public class LabyrinthTileFilters : MonoBehaviour
{
    public void AlwaysValid(int x, int y, bool[] result)
    {
        result[0] = true;
    }

    public void NeverValid(int x, int y, bool[] result)
    {
        result[0] = false;
    }

    public void BottomInvalid(int x, int y, bool[] result)
    {
        result[0] = y != 0;
    }

    public void LeftInvalid(int x, int y, bool[] result)
    {
        result[0] = x != 0;
    }
}
