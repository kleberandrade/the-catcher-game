using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(AudioSource))]
public class FadeInOut : MonoBehaviour
{
    private CanvasGroup m_CanvasGroup;
    private AudioSource m_AudioSource;

    private void Awake()
    {
        m_CanvasGroup = GetComponentInChildren<CanvasGroup>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        m_AudioSource.playOnAwake = false;
        m_AudioSource.loop = false;
    }

    public IEnumerator Pulse(float time)
    {
        m_AudioSource.Play();
        yield return StartCoroutine(Fading(true, time, 0.0f));
        yield return StartCoroutine(Fading(false, time, 0.0f));
    }

    public IEnumerator PulseInverse(float time)
    {
        m_AudioSource.Play();
        yield return StartCoroutine(Fading(false, time, 0.0f));
        yield return StartCoroutine(Fading(true, time, 0.0f));
    }

    public void Fade(bool fadeIn, float time, float delay)
    {
        StartCoroutine(Fading(fadeIn, time, delay));
    }

    public IEnumerator Fading(bool fadeIn, float time, float delay)
    {
        yield return new WaitForSeconds(delay);
        float rate = 1.0f / time;
        int startAlpha = 1 - Convert.ToInt32(fadeIn);
        int endAlpha = Convert.ToInt32(fadeIn);
        float progress = 0.0f;

        while (progress < 1.0)
        {
            m_CanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, progress);
            progress += rate * Time.deltaTime;
            yield return null;
        }

        m_CanvasGroup.alpha = endAlpha;
    }
}
