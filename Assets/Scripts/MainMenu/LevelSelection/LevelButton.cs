using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    public void GoToLevel(string level)
    {
        //TODO: Transitions

        SceneManager.LoadScene(level);
    }
}
