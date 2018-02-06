using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    public PlayerController m_Controller;
    public bool m_Invert = true;

    [Header("Amplitude")]
    public float m_MinAmplitude = -90.0f;
    public float m_MaxAmplitude = 90.0f;

    [Header("Viewport")]
    [Range(0.0f, 1.0f)]
    public float m_ViewportPadding = 0.1f;

    [Header("Keyboard")]
    public bool m_UseKeyboard;
    public float m_SpeedKeyboard = 90.0f;

    private float m_Angle = 0.0f;

    public float AngleRaw = 0.0f;


    private void FixedUpdate ()
    {
        if (m_UseKeyboard)
            AngleRaw += Input.GetAxis("Horizontal") * m_SpeedKeyboard * Time.deltaTime;
        else if (Connection.Instance.IsConnected)
            AngleRaw = (float)Connection.Instance.ReceivePackage.Position;

        if (m_Invert) AngleRaw *= -1;

        m_Angle = AngleRaw;

        float horizontal = Helper.Normalization(m_Angle, m_MinAmplitude, m_MaxAmplitude);
        horizontal = Helper.ViewportToWord(horizontal,  m_ViewportPadding, 1.0f - m_ViewportPadding, Helper.GetDepht(m_Controller.transform.position));

        m_Controller.Input(new Vector3(horizontal, 0.0f, 0.0f));
	}
}
