using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Nut : MonoBehaviour, IObjectSpawned
{
    #region [ Events ]
    public delegate void CollisionAction(Nut nutScript, bool captured);
    public static event CollisionAction OnNutCollision;
    #endregion

    public AudioClip m_GroundAudioClip;
    public AudioClip m_CapturedAudioClip;
    public float m_Speed = 0.5f;

    public float m_TimeToDestroy = 1.0f;
    public float m_TimeToCapture = 0.3f;

    public ParticleSystem m_SpurtParticle;

    private Animator m_Animator;
    private AudioSource m_AudioSource;
    private Collider m_Collider;
    private Rigidbody m_Rigidbody;

    private bool m_IsFalling = true;

    private void Awake()
    {
        m_Animator = GetComponentInChildren<Animator>();
        m_AudioSource = GetComponent<AudioSource>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<Collider>();
    }

    private void Start()
    {
        m_Rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
        m_IsFalling = true;
    }

    private void FixedUpdate()
    {
        if (!m_IsFalling)
            return;

        m_Rigidbody.velocity = Vector3.down * m_Speed;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
            GroundCollision();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Basket"))
            BacketCollision();
    }

    private void GroundCollision()
    { 
        m_Collider.enabled = false;
        m_IsFalling = false;

        m_AudioSource.clip = m_GroundAudioClip;
        m_AudioSource.Play();

        m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        m_Animator.SetBool("InGround", true);

        StartCoroutine(GroundDestroy());

        if (OnNutCollision != null)
            OnNutCollision(this, false);
    }

    private IEnumerator GroundDestroy()
    {
        Spurt();
        yield return new WaitForSeconds(m_TimeToDestroy);
        m_Rigidbody.constraints = RigidbodyConstraints.None;
        Destroy(gameObject);
    }

    public void BacketCollision()
    {
        m_IsFalling = false;
        m_Collider.enabled = false;

        m_AudioSource.clip = m_CapturedAudioClip;
        m_AudioSource.Play();

        m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        Destroy(gameObject, m_TimeToCapture);

        if (OnNutCollision != null)
            OnNutCollision(this, true);
    }

    private void Spurt()
    {
        ParticleSystem spurt = Instantiate(m_SpurtParticle, m_Rigidbody.position, Quaternion.identity);
        spurt.Play();
        Destroy(spurt.gameObject, spurt.main.duration * 2.0f);
    }

    public void SetSpeed(float speed)
    {
        m_Speed = speed;
    }

    public void NavigationTo(Vector3 scorePosition)
    {
        StartCoroutine(NavigationToScore(scorePosition));
    }

    private IEnumerator NavigationToScore(Vector3 scorePosition)
    {
        float capturedTime = Time.time;
        Vector3 capturedPosition = transform.position;

        while (Vector3.Distance(transform.position, scorePosition) > 0.1f)
        {
            transform.position = Vector3.Lerp(capturedPosition, scorePosition, (Time.time - capturedTime) / m_TimeToCapture);
            yield return null;
        }
    }
}
