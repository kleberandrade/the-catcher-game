using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HubManager : MonoBehaviour
{
    [Header("Close")]
    public Button m_CloseButton;
    public string m_CloseTitle = "Confirmar saída";
    public string m_CloseText = "Deseja sair do sistema de reabilitação robótica?";
    public string m_CloseNo = "Não";
    public string m_CloseYes = "Sim";

    [Header("User Not Defined")]
    public Button m_UserPlayButton;
    public string m_UserPlayTitle = "Falta de nome";
    public string m_UserPlayText = "Digite o nome do jogador!";
    public string m_UserPlayConfirm = "OK";

    [Header("Robot Zero")]
    public string m_RobotPlayTitle = "Zerar robô";
    public string m_RobotPlayText = "Favor zerar a posição do robô";
    public string m_RobotPlayConfirm = "OK";

    [Header("Tasks")]
    public InputField m_TaskInputField;
    public int m_TaskValueDefault = 20;
    public string m_TaskKey;

    [Header("Connection")]
    public Image m_ConnectionImage;
    public Color m_ConnectedColor;
    public Color m_DisconnectedColor;
    public Text m_ConnectionStatus;
    public string m_ConnectionTitle = "Falha de conexão";
    public string m_ConnectionText = "Sem conexão com o robô";
    public string m_ConnectionConfirm = "OK";

    [Header("Home Connection")]
    public Button m_HomeButton;
    public string m_HomeTitle = "Zerando robô";
    public string m_HomeText = "Mantenha o punho parado no centro";

    [Header("Other Properties")]
    public InputField m_UserInputField;
    public string m_PlaySceneName = "Gameplay";

    [Header("Keyboard")]
    public bool m_UseKeyboard = false;

    private bool m_RobotZeroed = false;
    private AudioSource m_AudioSource;
    private UnityAction m_YesAction;
    private UnityAction m_NoAction;
    private UnityAction m_ConfirmAction;

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        Connection.OnZeroed += OnZeroed;
        Connection.OnConnected += OnConnected;
        Connection.OnDisconnected += OnDisconnected;
    }

    private void OnDisable()
    {
        Connection.OnZeroed -= OnZeroed;
        Connection.OnConnected -= OnConnected;
        Connection.OnDisconnected -= OnDisconnected;
    }

    private void Start()
    {
        if (m_CloseButton)
        {
            m_CloseButton.onClick.RemoveAllListeners();
            m_CloseButton.onClick.AddListener(delegate { Close(); });
        }

        if (m_UserPlayButton)
        {
            m_UserPlayButton.onClick.RemoveAllListeners();
            m_UserPlayButton.onClick.AddListener(delegate { PlayGameCheck(); });
        }

        if (m_HomeButton)
        {
            m_HomeButton.onClick.RemoveAllListeners();
            m_HomeButton.onClick.AddListener(delegate { Zerar(); });
        }

        if (PlayerPrefs.HasKey(m_TaskKey))
            m_TaskInputField.text = PlayerPrefs.GetInt(m_TaskKey).ToString();
        else 
            m_TaskInputField.text = m_TaskValueDefault.ToString();
    }

    private void Update()
    {
        if (Connection.Instance.IsConnected)
        {
            m_ConnectionImage.color = m_ConnectedColor;
            m_ConnectionStatus.text = string.Format("{0:0.000}", Connection.Instance.ReceivePackage.Position);
        }
        else
        {
            m_ConnectionImage.color = m_DisconnectedColor;
        }
    }

    public void Close()
    {
        m_AudioSource.Play();
        m_YesAction = new UnityAction(Quit);
        m_NoAction = new UnityAction(Cancel);

        ShowDialog(m_CloseTitle, m_CloseText, m_CloseNo, m_CloseYes);
    }

    public void PlayGameCheck()
    {
        m_AudioSource.Play();
        m_ConfirmAction = new UnityAction(Cancel);

        if (string.IsNullOrEmpty(m_UserInputField.text))
            ShowDialog(m_UserPlayTitle, m_UserPlayText, m_UserPlayConfirm);
        else if (m_UseKeyboard)
            PlayGame();
        else if (!Connection.Instance.IsConnected)
            ShowDialog(m_ConnectionTitle, m_ConnectionText, m_ConnectionConfirm);
        else if (!m_RobotZeroed)
            ShowDialog(m_RobotPlayTitle, m_RobotPlayText, m_RobotPlayConfirm);
        else
            PlayGame();
    }

    public void PlayGame()
    {
        PlayerPrefs.SetInt(m_TaskKey, int.Parse(m_TaskInputField.text));
        Transition.LoadScene(m_PlaySceneName, Color.black, 2.0f);
    }

    public void Zerar()
    {
        m_AudioSource.Play();
        ShowDialog(m_HomeTitle, m_HomeText);
        m_RobotZeroed = false;
        Connection.Instance.Home();
    }

    private void OnZeroed()
    {
        m_RobotZeroed = true;
        MessageDialog.Instance.Hide();
    }

    private void OnConnected()
    {
        m_ConnectionImage.color = m_ConnectedColor;
    }

    private void OnDisconnected()
    {
        m_ConnectionImage.color = m_DisconnectedColor;
    }

    private void ShowDialog(string title, string text)
    {
         MessageDialog.Instance.Show(title, text, new string[] { }, new UnityAction[] { });
    }

    private void ShowDialog(string title, string text, string leftButton, string rightButton = null)
    {
        if (rightButton != null)
            MessageDialog.Instance.Show(title, text, new string[] { leftButton, rightButton }, new UnityAction[] { m_NoAction, m_YesAction });
        else
            MessageDialog.Instance.Show(title, text, new string[] { leftButton }, new UnityAction[] { m_ConfirmAction });
    }

    private void Cancel()
    {
        m_AudioSource.Play();
    }

    private void Quit()
    {
        m_AudioSource.Play();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
