using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private void OnEnable()
    {
        Time.timeScale = 0.01f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1.0f;
    }
}
