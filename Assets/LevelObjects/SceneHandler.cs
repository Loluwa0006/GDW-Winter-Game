using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public void SwapScene(string newScene)
    {
        if (SceneManager.GetSceneByName(newScene) != null)
        {
            SceneManager.LoadScene(newScene);
        }
    }
}
