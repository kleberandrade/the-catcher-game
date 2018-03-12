using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeWindow : MonoBehaviour
{
    #region [ Singleton ]
    private static TimeWindow m_Instance = null;

    public static TimeWindow Instance
    {
        get { return m_Instance; }
    }

    private void Awake()
    {
        if (m_Instance == null)
            m_Instance = this;
        else if (m_Instance != this)
            Destroy(gameObject);
    }
    #endregion


    public bool m_UseTimeWindow = true;

    public Transform m_Player;

    [Header("Robotic")]
    public float m_Stiffness = 150.0f;

    public float m_Damping = 15.0f;

    public float m_HelperTime = 0.0f;

    private Vector3 m_Min;
    private Vector3 m_Max;

    private float m_CurrentTop;
    private float m_CurrentLeft;
    private float m_CurrentRight;
    private float m_CurrentBottom;

    private Vector3 m_MinScreen;
    private Vector3 m_MaxScreen;

    private Vector3 m_MinViewport;
    private Vector3 m_MaxViewport;

    private void Start ()
    {
        Connection.Instance.SendPackage.Left = 90.0f;
        Connection.Instance.SendPackage.Right = -90.0f;
        Connection.Instance.SendPackage.Setpoint = 0.0f;
        Connection.Instance.SendPackage.Stiffness = m_Stiffness;
        Connection.Instance.SendPackage.Damping = m_Damping;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_UseTimeWindow)
            return;

        DrawBox();
    }

    private void DrawBox()
    {
        // Tela total
        Debug.DrawLine(new Vector3(m_MinScreen.x, m_MinScreen.y, m_Player.position.z), new Vector3(m_MinScreen.x, m_MaxScreen.y, m_Player.position.z), Color.yellow);
        Debug.DrawLine(new Vector3(m_MinScreen.x, m_MaxScreen.y, m_Player.position.z), new Vector3(m_MaxScreen.x, m_MaxScreen.y, m_Player.position.z), Color.yellow);
        Debug.DrawLine(new Vector3(m_MaxScreen.x, m_MaxScreen.y, m_Player.position.z), new Vector3(m_MaxScreen.x, m_MinScreen.y, m_Player.position.z), Color.yellow);
        Debug.DrawLine(new Vector3(m_MaxScreen.x, m_MinScreen.y, m_Player.position.z), new Vector3(m_MinScreen.x, m_MinScreen.y, m_Player.position.z), Color.yellow);

        // Range utilizado pelo jogo
        Debug.DrawLine(new Vector3(m_Min.x, m_Min.y, m_Player.position.z), new Vector3(m_Min.x, m_Max.y, m_Player.position.z), Color.red);
        Debug.DrawLine(new Vector3(m_Min.x, m_Max.y, m_Player.position.z), new Vector3(m_Max.x, m_Max.y, m_Player.position.z), Color.red);
        Debug.DrawLine(new Vector3(m_Max.x, m_Max.y, m_Player.position.z), new Vector3(m_Max.x, m_Min.y, m_Player.position.z), Color.red);
        Debug.DrawLine(new Vector3(m_Max.x, m_Min.y, m_Player.position.z), new Vector3(m_Min.x, m_Min.y, m_Player.position.z), Color.red);

        // Janela de tempo
        Debug.DrawLine(new Vector3(m_CurrentLeft, m_CurrentBottom, m_Player.position.z), new Vector3(m_CurrentLeft, m_CurrentTop, m_Player.position.z), Color.blue);
        Debug.DrawLine(new Vector3(m_CurrentLeft, m_CurrentTop, m_Player.position.z), new Vector3(m_CurrentRight, m_CurrentTop, m_Player.position.z), Color.blue);
        Debug.DrawLine(new Vector3(m_CurrentRight, m_CurrentTop, m_Player.position.z), new Vector3(m_CurrentRight, m_CurrentBottom, m_Player.position.z), Color.blue);
        Debug.DrawLine(new Vector3(m_CurrentRight, m_CurrentBottom, m_Player.position.z), new Vector3(m_CurrentLeft, m_CurrentBottom, m_Player.position.z), Color.blue);
    }
}

public enum TimeWindowState
{
    None,
    Go,
    Back
}