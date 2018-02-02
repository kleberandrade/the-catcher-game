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

    private void Start()
    {
        m_CloseButton.onClick.RemoveAllListeners();
        m_CloseButton.onClick.AddListener(delegate { Close(); });

        m_UserPlayButton.onClick.RemoveAllListeners();
        m_UserPlayButton.onClick.AddListener(delegate { PlayGameCheck(); });

        if (PlayerPrefs.HasKey(m_TaskKey))
            m_TaskInputField.text = PlayerPrefs.GetInt(m_TaskKey).ToString();
        else 
            m_TaskInputField.text = m_TaskValueDefault.ToString();
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
        else if (!m_RobotZeroed)
            ShowDialog(m_RobotPlayTitle, m_RobotPlayText, m_RobotPlayConfirm);
        else
            PlayGame();
    }

    public void PlayGame()
    {
        PlayerPrefs.SetInt(m_TaskKey, int.Parse(m_TaskInputField.text));
        Transition.LoadScene(m_PlaySceneName, Color.black, 0.5f);
    }

    public void Zerar()
    {
        m_AudioSource.Play();
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
