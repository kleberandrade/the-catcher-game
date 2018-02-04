using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Rotate Angle")]
    public float m_SpeedThreshold = 3.0f;
    public float m_SpeedRotation = 8.0f;
    public float m_SmoothingRotation = 15.0f;

    [Header("Foot Step")]
    public AudioClip m_StepClip;

    private Vector3 m_Position;
    private Rigidbody m_Rigidbody;
    private Animator m_Animator;
    private Vector3 m_LastPosition;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        m_LastPosition = m_Rigidbody.position;
    }

    public void Input(Vector3 position)
    {
        m_Position = position;
    }

    private void FixedUpdate()
    {
        Move();

        Animating();

        Turning();

        m_LastPosition = m_Rigidbody.position;
    }

    private void Move()
    {
        m_Rigidbody.MovePosition(m_Position);
    }

    private void Turning()
    {
        float speed = (m_Rigidbody.velocity.magnitude - m_SpeedThreshold);
        if (m_Rigidbody.position.x - m_LastPosition.x > 0.0f)
            speed *= -1f;

        float horizontal = Mathf.Clamp(speed / m_SpeedRotation, -1f, 1f);

        Vector3 targetDirection = new Vector3(-Mathf.Sin(horizontal * Mathf.PI / 2.0f), 0.0f, -Mathf.Cos(horizontal * Mathf.PI / 2.0f));
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        Quaternion newRotation = Quaternion.Lerp(m_Rigidbody.rotation, targetRotation, Time.deltaTime * m_SmoothingRotation);
        m_Rigidbody.MoveRotation(newRotation);
    }

    private void Animating()
    {
        float speed = m_Rigidbody.velocity.magnitude;
        bool isWalking = m_Rigidbody.velocity.magnitude > m_SpeedThreshold;

        m_Animator.SetBool("IsWalking", isWalking);
        m_Animator.SetFloat("Speed", speed * 0.5f);
    }

    private void OnFootStepAudio()
    {
        AudioSource.PlayClipAtPoint(m_StepClip, m_Rigidbody.position);
    }
}

