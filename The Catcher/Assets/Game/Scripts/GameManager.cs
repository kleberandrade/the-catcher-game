using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public Score m_Score;
    public Spawner m_Spawner;
    public GameObject m_Gameover;
    public PlayerInput m_PlayerInput;
    public Transform m_PlayerTransform;

    public float m_MinSpeed = 0.01f;
    public float m_MaxSpeed = 1.0f;

    public float m_EndDelay = 3f;
    private WaitForSeconds m_EndWait;

    [Header("Lose Connection")]
    public string m_LoseConnectionTitle = "Problema de conexão";
    public string m_LoseConnectionText = "Conexão perdida, tentando reconectar...";
    public string m_LoseConnectionConfirm = "Sair";
    private UnityAction m_QuitAction;

    [Header("Message Dialog")]
    public string m_MessageDialogName = "MessageDialogCanvas";
    public GameObject m_MessageDialogPrefab;

    private float m_StartTime;
    private bool m_TargetInGame = false;

    private int m_TotalTargetNumber;
    private int m_TargetNumberSpawned = 0;

    private Chromosome m_CurrentChromosome = null;

    private float m_ElapsedTime = 0.0f;

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
        yield return StartCoroutine(Playing());
        yield return StartCoroutine(Gameover());
    }

    private bool IsFinish()
    {
        return m_TargetNumberSpawned > m_TotalTargetNumber;
    }

    private IEnumerator Starting()
    {
        m_TargetNumberSpawned = 0;
        m_TotalTargetNumber = SessionManager.Instance.User.Target.TargetNumber;
        m_Score.SetNumberOfTargets(m_TotalTargetNumber);

        yield return null;
    }

    private IEnumerator Playing()
    {
        m_StartTime = Time.time;

        while (!IsFinish())
        {
            m_ElapsedTime = Time.time - m_StartTime;

            if (!m_TargetInGame)
                Spawn();

            if (!PauseGame.Instance.IsPaused)
            {
                SessionManager.Instance.User.Profile.Motion.Add(m_PlayerInput.AngleRaw);
                SessionManager.Instance.User.Profile.Timestamp.Add(m_ElapsedTime);
            }

            yield return null;
        }

        AIDirector.Instance.VerifyHasNext();
    }

    private IEnumerator Gameover()
    {
        yield return m_EndWait;

        SessionManager.Instance.User.Target.TotalCaptured = m_Score.Point;

        m_Gameover.gameObject.SetActive(true);
        m_Gameover.GetComponent<Gameover>().Show(m_Score.Percentage);
    }

    private void OnNutCollision(Nut nutScript, bool captured)
    {
        if (captured)
        {
            AIDirector.Instance.Evaluate(0);
        }
        else
        {
            float targetX = Helper.WorldToViewport(nutScript.transform.position);
            float playerX = Helper.WorldToViewport(m_PlayerTransform.position);

            float error = Mathf.Abs(targetX - playerX) / 0.5f;

            AIDirector.Instance.Evaluate(error);
        }


        m_TargetInGame = false;

        if (captured)
        {
            m_Score.NextPoint();
            Vector3 scorePosition = m_Score.WorldPoint(nutScript.transform.position);
            nutScript.NavigationTo(scorePosition);

            SessionManager.Instance.User.Target.Captured.Add(true);
        }
        else
        {
            SessionManager.Instance.User.Target.Captured.Add(false);
        }
    }

    private void Spawn()
    {
        m_TargetNumberSpawned++;

        if (IsFinish())
            return;

        m_CurrentChromosome = AIDirector.Instance.GetNextChromosome();

        float cDistance = m_CurrentChromosome.Distance;
        float cSpeed = m_CurrentChromosome.Speed;

        float depth = Helper.GetDepht(m_PlayerTransform.position);

        // Converte a posição do personagem (mundo) para a viewport
        float x = Helper.WorldToViewport(m_PlayerTransform.position, 0.0f, 1.0f);

        // Define a direção de lançamento
        float direction = Random.Range(0.0f, 1.0f) <= x ? -1.0f : 1.0f;
        float distance = Helper.Map(cDistance, 0.0f, 1.0f, 0.1f, 0.5f);
        if (x + distance * direction < 0.0f || x + direction * distance > 1.0f)
            direction *= -1.0f;

        // Define a posição final do alvo
        x += distance * direction;
        x = Helper.ViewportToWord(x, 0.0f, 1.0f, depth);

        Vector3 position = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, depth));
        position.x = x;


        float speed = Helper.Map(cSpeed, 0.0f, 1.0f, m_MinSpeed, m_MaxSpeed);
        float height = (position.y - m_PlayerTransform.position.y);

        speed = height * speed;
        //Debug.Log("Time to fall: " + height / speed);

        m_TargetInGame = true;
        m_Score.NextTarget();
        m_Spawner.Spawn(position, speed);

        SessionManager.Instance.User.Target.Position.Add(position.x);
        SessionManager.Instance.User.Target.TimeToSpawn.Add(m_ElapsedTime);
    }

    private void OnEnable()
    {
        Nut.OnNutCollision += OnNutCollision;
        Connection.OnConnected += OnConnected;
        Connection.OnDisconnected += OnDisconnected;
    }

    private void OnDisable()
    {
        Nut.OnNutCollision -= OnNutCollision;
        Connection.OnConnected -= OnConnected;
        Connection.OnDisconnected -= OnDisconnected;
    }

    private void OnConnected()
    {
        MessageDialog.Instance.Hide();
    }

    private void OnDisconnected()
    {
        m_QuitAction = new UnityAction(delegate { Close(); });

        MessageDialog.Instance.Show(m_LoseConnectionTitle,
            m_LoseConnectionText,
            new string[] { m_LoseConnectionConfirm },
            new UnityAction[] { m_QuitAction });
    }

    private void Close()
    {
        Transition.LoadScene("MainMenu", Color.black, 2.0f);
    }
}