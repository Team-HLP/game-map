using System.Collections;
using UnityEngine;

public class HealManager : MonoBehaviour
{
    public GameObject healParticles;

    private static HealManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (healParticles != null)
        {
            healParticles.SetActive(false);
        }
    }

    public static void ShowHealParticlesForSeconds(float seconds)
    {
        if (instance != null && instance.healParticles != null)
        {
            instance.StartCoroutine(instance.TemporarilyActivateParticles(seconds));

            var audio = instance.GetComponent<AudioSource>();
            if (audio != null) audio.Play();
        }
    }

    private IEnumerator TemporarilyActivateParticles(float seconds)
    {
        healParticles.SetActive(true);
        yield return new WaitForSeconds(seconds);
        healParticles.SetActive(false);
    }
}
