using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager : MonoBehaviour
{ 
    #region [ Singleton ]
    private static SessionManager m_Instance = null;

    public static SessionManager Instance
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

    private User m_User;

    public void CreateUser(string name, int targetNumber)
    {
        m_User = new User(name, targetNumber);
    }
}

[Serializable]
public class User
{
    public string Name;

    public DateTime DateTime;

    public int TargetNumber;

    public Profile Profile;

    public User(string name, int targetNumber)
    {
        Name = name;
        TargetNumber = targetNumber;

        DateTime = DateTime.Now;
        Profile = new Profile();
    }
}

[Serializable]
public class Profile
{
    public List<float> Motion;

    public List<float> Timestamp;

    public Profile()
    {
        Motion = new List<float>();
        Timestamp = new List<float>();
    }
}