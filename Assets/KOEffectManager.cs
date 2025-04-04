using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class KOEffectManager : MonoBehaviour
{
    private void Awake()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        foreach (AudioSource source in sources)
        {
            if (GameManager.instance != null)
            {
                source.volume = (float)GameManager.instance.GetGameSetting("Volume");
            }
        }
    }

   public void DestructionDelay(Animator animator)
    {
        StartCoroutine( DestroySelf(animator));
    }

    IEnumerator DestroySelf(Animator animator)
    {
        AudioSource[] sources = GetComponents<AudioSource>();

        yield return new WaitUntil(() => sources[0].isPlaying == false);
        Destroy(animator.gameObject);
    }
}

