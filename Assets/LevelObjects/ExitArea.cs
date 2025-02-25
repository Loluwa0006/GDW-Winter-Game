using UnityEngine;

public class ExitArea : MonoBehaviour
{
    [SerializeField] LevelManager levelManager;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        levelManager.OnLevelFinished();
    }
}
