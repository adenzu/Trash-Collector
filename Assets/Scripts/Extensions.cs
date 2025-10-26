using UnityEngine;

public static class Extensions
{
    public static void Shuffle<T>(this T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int randomIndex = Random.Range(0, array.Length);
            (array[randomIndex], array[i]) = (array[i], array[randomIndex]);
        }
    }
}
