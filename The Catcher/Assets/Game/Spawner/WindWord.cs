using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindWord : MonoBehaviour
{
    public float m_NormalPeriodOscillator = 1.0f;
    public float m_NormalAmplitudeOscillator = 1.0f;
    public float m_PeriodOscillator = 35.0f;
    public float m_AmplitudeOscillator = 3.0f;
    public List<Oscillator> m_Oscillators;

    private float m_AudioOscilationTime = 1.0f;
    private AudioSource m_AudioSource;
    
    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        m_NormalAmplitudeOscillator = m_Oscillators[0].Amplitude;
        m_NormalPeriodOscillator = m_Oscillators[0].Period;
        m_AudioOscilationTime = m_AudioSource.clip.length - 0.5f;
    }

    public void Fan()
    {
        StartCoroutine(BlowingStrong());
    }

    private IEnumerator BlowingStrong()
    {
        m_AudioSource.Play();

        foreach (Oscillator oscilator in m_Oscillators)
        {
            oscilator.Period = m_PeriodOscillator;
            oscilator.Amplitude = m_AmplitudeOscillator;
            yield return null;
        }

        yield return new WaitForSeconds(m_AudioOscilationTime);

        foreach (Oscillator oscilator in m_Oscillators)
        {
            oscilator.Period = m_NormalPeriodOscillator;
            oscilator.Amplitude = m_NormalAmplitudeOscillator;
            yield return null;
        }
    }
}
