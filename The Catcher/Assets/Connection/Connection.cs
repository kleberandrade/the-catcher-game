﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Connection : MonoBehaviour
{
    #region [ Events ]
    public delegate void ConnectionStatusAction();
    public static event ConnectionStatusAction OnDisconnected;
    public static event ConnectionStatusAction OnConnected;
    public static event ConnectionStatusAction OnZeroed;
    #endregion

    #region [ Singleton ]
    private static Connection m_Instance = null;    
    
    public static Connection Instance
    {
        get { return m_Instance; }
    }

    private void Awake()
    {
        if (m_Instance == null)
            m_Instance = this;
        else if (m_Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
    #endregion

    public string m_ServerHostName = "192.168.1.2";

    public int m_ServerPort = 3333;

    private byte[] m_BufferRead = new byte[32];

    private Socket m_ClientSocket = null;

    private volatile bool m_IsConnected = false;

    private volatile ConnectionGamePackage m_SendPackage = new ConnectionGamePackage();

    private volatile ConnectionRobotPackage m_ReceivePackage = new ConnectionRobotPackage();

    private volatile ConnectionState m_ConnectionState = ConnectionState.Connecting;

    private volatile ConnectionRobotStatus m_LastConnectionRoboStatus = ConnectionRobotStatus.Disconnected;

    private object m_Lock = new object();

    public int m_Sleep = 0;

    private Thread m_Thread = null;

    private void Start ()
    {
        Connect();
	}
	
    public void Connect()
    {
        if (m_IsConnected)
            return;

        try
        {
            m_ClientSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            m_Thread = new Thread(Run);
            m_Thread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            Disconnect();
        }
    }

    public void Run()
    {
        m_ConnectionState = ConnectionState.Connecting;

        try
        {
            m_ClientSocket.Connect(m_ServerHostName, m_ServerPort);
            m_IsConnected = m_ClientSocket.Connected;

            if (m_IsConnected && OnConnected != null)
                OnConnected();

            while (IsConnected)
            {
                Send();

                Receive();

                Thread.Sleep(m_Sleep);
            }

            Disconnect();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            Disconnect();
        }
    }

    private void Receive()
    {
        lock (m_Lock)
        {
            m_ClientSocket.Receive(m_BufferRead, 0, 12, SocketFlags.None);
            if (m_BufferRead.Length > 0)
                m_ReceivePackage.Decode(m_BufferRead);

            if (m_LastConnectionRoboStatus == ConnectionRobotStatus.Homing && (ConnectionRobotStatus)m_ReceivePackage.Status == ConnectionRobotStatus.Running)
            {
                m_SendPackage.Control = (int)ConnectionGameControl.Running;
                if (OnZeroed != null)
                    OnZeroed();
            }

            if (m_SendPackage.Control == (int)ConnectionGameControl.Disconnect)
                m_IsConnected = false;

            m_LastConnectionRoboStatus = (ConnectionRobotStatus)ReceivePackage.Status;
        }
    }

    private void Send()
    {
        lock (m_Lock)
        { 
            byte[] buffer = m_SendPackage.Encode();
            m_ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }
    }

    public void Home()
    {
        lock (m_Lock)
        {
            m_ConnectionState = ConnectionState.Homing;
            m_SendPackage.Control = (int)ConnectionGameControl.Home;
        }
    }

    private void Disconnect()
    {
        if (OnDisconnected != null)
            OnDisconnected();

        m_IsConnected = false;
        m_ConnectionState = ConnectionState.Disconnecting;
        m_SendPackage.Control = (int)ConnectionGameControl.Disconnect;

        if (OnDisconnected != null)
            OnDisconnected();

        if (m_Thread != null)
            m_Thread.Abort();

        if (m_ClientSocket != null && m_ClientSocket.Connected)
        {

            m_ClientSocket.Close();
            m_ClientSocket = null;
        }
    }

    private void OnDestroy()
    {
        Disconnect();
    }

    private void OnApplicationQuit()
    {
        Disconnect();   
    }

    public bool IsConnected
    {
        get { return m_IsConnected; }
    }

    public ConnectionGamePackage SendPackage
    {
        get { return m_SendPackage; }
        set { lock (m_Lock) { m_SendPackage = value; }  }
    }

    public ConnectionRobotPackage ReceivePackage
    {
        get { return m_ReceivePackage; }
        set { lock (m_Lock) { m_ReceivePackage = value; } }
    }
}

#region [ Packge Interface ]
public interface IConnectionPackage
{
    void Decode(byte[] data);

    byte[] Encode();
}
#endregion

#region [ Game Package ]

public enum ConnectionGameControl
{
    None = 0,
    Running = 1,
    Home = 2,
    Disconnect = 3,
}

public class ConnectionGamePackage : IConnectionPackage
{
    public int Control { get; set; }
    public double Setpoint { get; set; }
    public double Stiffness { get; set; }
    public double Damping { get; set; }
    public double Left { get; set; }
    public double Right { get; set; }

    public ConnectionGamePackage()
    {
        Control = (int)ConnectionGameControl.Running;
    }

    public ConnectionGamePackage(byte[] data)
    {
        Decode(data);
    }

    public ConnectionGamePackage(int control, double setpoint, double stiffness, double damping, double left, double right)
    {
        Control = control;
        Setpoint = setpoint;
        Stiffness = stiffness;
        Damping = damping;
        Left = left;
        Right = right;
    }

    public void Decode(byte[] data)
    {
        Control = BitConverter.ToInt32(data, 0);
        Setpoint = BitConverter.ToDouble(data, 4);
        Stiffness = BitConverter.ToDouble(data, 12);
        Damping = BitConverter.ToDouble(data, 20);
        Left = BitConverter.ToDouble(data, 28);
        Right = BitConverter.ToDouble(data, 36);
    }

    public byte[] Encode()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(BitConverter.GetBytes(Control));
        buffer.AddRange(BitConverter.GetBytes(Setpoint));
        buffer.AddRange(BitConverter.GetBytes(Stiffness));
        buffer.AddRange(BitConverter.GetBytes(Damping));
        buffer.AddRange(BitConverter.GetBytes(Left));
        buffer.AddRange(BitConverter.GetBytes(Right));
        return buffer.ToArray();
    }

    public override string ToString()
    {
        return string.Format("Control: {0}\nSetpoint: {1}\nStiffness: {2}\nDamping: {3}\nLeft: {4}\nRight: {5}",
            (ConnectionGameControl)Control,
            Setpoint,
            Stiffness,
            Damping,
            Left,
            Right);
    }
}
#endregion

#region [ Robot Package ]

public enum ConnectionRobotStatus
{
    Disconnected = 0,
    Initializing = 1,
    Running = 2,
    Homing = 3,
    Closing = 4,
    Error = 99
}

public class ConnectionRobotPackage : IConnectionPackage
{
    public int Status { get; set; }
    public double Position { get; set; }

    public ConnectionRobotPackage()
    {

    }

    public ConnectionRobotPackage(int status, double position)
    {
        Status = status;
        Position = position;
    }

    public ConnectionRobotPackage(byte[] data)
    {
        Decode(data);
    }

    public void Decode(byte[] data)
    {
        Status = BitConverter.ToInt32(data, 0);
        Position = BitConverter.ToDouble(data, 4);
    }

    public byte[] Encode()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(BitConverter.GetBytes(Status));
        buffer.AddRange(BitConverter.GetBytes(Position));
        return buffer.ToArray();
    }

    public override string ToString()
    {
        return string.Format("Status: {0}\nPosition: {1}",
            (ConnectionRobotStatus)Status,
            Position);
    }
}
#endregion

#region [ State Machine ]
public enum ConnectionState
{
    None,
    Connecting,
    Playing,
    Homing,
    Disconnecting
}
#endregion