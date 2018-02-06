using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    public UserDataSave User;

    public string FileName
    {
        get { return string.Format("{0} {1}.json", User.Name, User.Date).Replace(":","-"); }
    }

    public void CreateUser(string name, int targetNumber)
    {
        User = new UserDataSave(name, targetNumber);
    }

    public void Save()
    {
        string filePath = "D:/The Catcher/Profile";
        Directory.CreateDirectory(filePath);

        filePath += "/" + FileName;

        string dataAsJson = JsonUtility.ToJson(this);
        File.WriteAllText(filePath, dataAsJson);
    }
}

[Serializable]
public class UserDataSave
{
    public string Name;

    [NonSerialized]
    public DateTime DateTime;

    public string Date;

    public ProfileDataSave Profile;

    public TargetDataSave Target;

    public UserDataSave(string name, int targetNumber)
    {
        Name = name;
        DateTime = DateTime.Now;
        Date = DateTime.ToString("u");
        Profile = new ProfileDataSave();
        Target = new TargetDataSave(targetNumber);
    }
}

[Serializable]
public class ProfileDataSave
{
    public List<float> Motion;

    public List<float> Timestamp;

    public ProfileDataSave()
    {
        Motion = new List<float>();
        Timestamp = new List<float>();
    }
}

[Serializable]
public class TargetDataSave
{
    public int TargetNumber;

    public int TotalCaptured;

    public List<bool> Captured;

    public List<float> Position;

    public List<float> TimeToSpawn;

    public TargetDataSave(int targetNumber)
    {
        TargetNumber = targetNumber;
        TotalCaptured = 0;
        Position = new List<float>();
        Captured = new List<bool>();
        TimeToSpawn = new List<float>();
    }
}
