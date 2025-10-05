using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void AllTrue(bool[] bools)
    {
        for (int i = 0; i < bools.Length; i++)
        {
            bools[i] = true;
        }
    }
}
