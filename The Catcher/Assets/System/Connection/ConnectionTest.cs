using UnityEngine;

public class ConnectionTest : MonoBehaviour
{
    [Header("Connection")]
    public bool m_Connected;

    [Header("Game Package")]
    public ConnectionGameControl m_Control;
    public float m_Left;
    public float m_Right;
    public float m_Setpoint;
    public float m_Stiffness = 50;
    public float m_Damping = 10;

    [Header("Robot Package")]
    public float m_Position;
    public ConnectionRobotStatus m_Status;

    private Connection m_Connection;

    private void Start()
    {
        m_Connection = Connection.Instance;
    }

    private void Update ()
    {
        m_Connected = m_Connection.IsConnected;

        m_Connection.SendPackage.Control = (int)m_Control;
        m_Connection.SendPackage.Setpoint = m_Setpoint;
        m_Connection.SendPackage.Left = m_Left;
        m_Connection.SendPackage.Right = m_Left;
        m_Connection.SendPackage.Stiffness = m_Stiffness;
        m_Connection.SendPackage.Damping = m_Damping;

        m_Position = (float)m_Connection.ReceivePackage.Position;
        m_Status = (ConnectionRobotStatus)m_Connection.ReceivePackage.Status;
    }
}
