using UnityEngine;
using UnityEngine.UI;

public class PlayerInputDebug : MonoBehaviour
{
    public PlayerController m_Controller;
    public Slider m_Slider;

    [Header("Amplitude")]
    public float m_MinAmplitude = -90.0f;
    public float m_MaxAmplitude = 90.0f;

    [Header("Viewport")]
    [Range(0.0f, 1.0f)]
    public float m_ViewportPadding = 0.1f;

    [Header("Keyboard")]
    public bool m_UseKeyboard;
    public float m_SpeedKeyboard;

    private float m_Angle = 0.0f;

    private void Start()
    { 
        SetDebugMode();
    }

    private void Update ()
    {
        if (m_UseKeyboard)
            m_Angle += Input.GetAxis("Horizontal") * m_SpeedKeyboard * Time.deltaTime;
        else if (m_Slider)
            m_Angle = m_Slider.value;

        float horizontal = Helper.Normalization(m_Angle, m_MinAmplitude, m_MaxAmplitude);
        horizontal = Helper.ViewportToWord(horizontal,  m_ViewportPadding, 1.0f - m_ViewportPadding, Helper.GetDepht(m_Controller.transform.position));

        m_Controller.Input(new Vector3(horizontal, 0.0f, 0.0f));
	}

    private void SetDebugMode()
    {
        if (m_UseKeyboard)
        {
            m_Slider.gameObject.SetActive(false);
            return;
        }

        if (m_Slider)
        {
            m_Slider.gameObject.SetActive(true);
            m_Slider.maxValue = 90.0f;
            m_Slider.minValue = -90.0f;
            m_Slider.value = 0.0f;
            m_Slider.interactable = true;
            m_Slider.wholeNumbers = false;
        }
    }
}
