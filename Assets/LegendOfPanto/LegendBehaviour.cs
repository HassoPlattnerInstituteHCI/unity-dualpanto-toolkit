using System.Threading.Tasks;
using UnityEngine;
public class LegendBehaviour : MonoBehaviour
{
    AudioSource source;

    protected async Task playSound(AudioClip clip)
    {
        source.clip = clip;
        source.Play();
        await Task.Delay(Mathf.RoundToInt(clip.length * 1000));
    }

    protected void Awake()
    {
        source = GetComponent<AudioSource>();
    }
}