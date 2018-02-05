using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public Score m_Score;
    public Spawner m_Spawner;
    public GameObject m_Gameover;

    public float m_EndDelay = 3f;
    private WaitForSeconds m_EndWait;

    [Header("Lose Connection")]
    public string m_LoseConnectionTitle = "Problema de conexão";
    public string m_LoseConnectionText = "Conexão perdida, tentando reconectar...";
    public string m_LoseConnectionConfirm = "Sair";

    [Header("Message Dialog")]
    public string m_MessageDialogName = "MessageDialogCanvas";
    public GameObject m_MessageDialogPrefab;

    private UnityAction m_ConfirmAction;

    private void Start()
    {
        if (GameObject.Find(m_MessageDialogName) == null)
            Instantiate(m_MessageDialogPrefab);

        m_EndWait = new WaitForSeconds(m_EndDelay);

        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(Starting());
        yield return StartCoroutine(Tutorial());
        yield return StartCoroutine(Playing());
        yield return StartCoroutine(Gameover());
    }

    private bool IsFinish()
    {
        return false;
    }

    private IEnumerator Starting()
    {
        yield return null;
    }

    private IEnumerator Tutorial()
    {
        yield return null;
    }

    private IEnumerator Playing()
    {
        while (!IsFinish())
        {
            yield return null;
        }
    }

    private IEnumerator Gameover()
    {
        Debug.Log("Gameover...");
        yield return m_EndWait;

        m_Gameover.SetActive(true);
        Gameover gameoverScript = m_Gameover.GetComponent<Gameover>();
        
        gameoverScript.Show();
    }

    private Vector3 GetNutPositionToSpawn()
    {
        return Vector3.zero;
    }

    private float GetNutSpeedToSpawn()
    {
        return 0.0f;
    }

    private void OnNutCollision(Nut nutScript, bool captured)
    {
        Vector3 position = GetNutPositionToSpawn();
        float speed = GetNutSpeedToSpawn();
        m_Spawner.Spawn(position, speed);
    }

    private void OnEnable()
    {
        Connection.OnConnected += OnConnected;
        Connection.OnDisconnected += OnDisconnected;
    }

    private void OnDisable()
    {
        Connection.OnConnected -= OnConnected;
        Connection.OnDisconnected -= OnDisconnected;
    }

    private void OnConnected()
    {
        MessageDialog.Instance.Hide();
    }

    private void OnDisconnected()
    {
        MessageDialog.Instance.Show(m_LoseConnectionTitle,
            m_LoseConnectionText,
            new string[] { m_LoseConnectionConfirm },
            new UnityAction[] { m_ConfirmAction });
    }
}