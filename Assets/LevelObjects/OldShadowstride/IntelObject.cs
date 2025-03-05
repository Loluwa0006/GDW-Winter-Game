using UnityEngine;


[System.Serializable]
public class IntelObject : MonoBehaviour
{
    [SerializeField] Sprite intelIcon = null;
    [SerializeField] LevelManager levelManager = null;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (levelManager != null)
        {
            levelManager.OnIntelSecured(this, intelIcon);
            gameObject.SetActive(false);
        }
    }
}
