using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void AlwaysTransition(StateMachine.TransitionDecision decision)
    {
        decision.Decide(true);
    }

    public void NeverTransition(StateMachine.TransitionDecision decision)
    {
        decision.Decide(false);
    }
}
