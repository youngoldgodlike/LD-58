using System.Collections;
using UnityEngine;

namespace Main.Scripts
{
    public class AudioSoftStop : MonoBehaviour
    {
        public AudioSource src;
        [SerializeField] private AudioClip _clip;
        [Range(0.001f, 0.2f)] public float fadeOutSec = 0.02f;

        private void Awake()
        {
            PlayOneShotWithTail(_clip);
        }

        public void PlayOneShotWithTail(AudioClip clip, float volume = 1f)
        {
            StopAllCoroutines();
            src.clip = clip;
            src.volume = volume;
            src.pitch = Random.Range(0.9f, 1.1f);
            src.Play();
            StartCoroutine(FadeOutAtClipEnd());
        }

        IEnumerator FadeOutAtClipEnd()
        {
            if (src.clip == null) yield break;
            float tail = Mathf.Max(0.005f, fadeOutSec);
            // Ждём до хвоста
            yield return new WaitForSeconds(Mathf.Max(0, src.clip.length - tail));
            float startVol = src.volume;
            float t = 0f;
            while (t < tail)
            {
                t += Time.unscaledDeltaTime;
                src.volume = Mathf.Lerp(startVol, 0f, t / tail);
                yield return null;
            }
            src.Stop();
            src.volume = startVol;
        }
    }
}