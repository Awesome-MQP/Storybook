using UnityEngine;
using System.Collections;

public class CharacterAnimator : MonoBehaviour {

    [SerializeField]
    private float m_speed = 0.5f;

    [SerializeField]
    private float m_targetDistance = 0.15f;

    private bool m_isMoving = false;
    private Vector3 m_destination;
    private bool m_isAtDestination = false;
    private float m_startTime;

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, m_destination, Time.time - m_startTime);
            if (Vector3.Distance(transform.position, m_destination) < m_targetDistance)
            {
                m_isAtDestination = true;
                m_isMoving = false;
            }
        }
    }

    /// <summary>
    /// True if the character has reached the destination it was moving to, false otherwise
    /// </summary>
    public bool IsAtDestination
    {
        get { return m_isAtDestination; }
    }

    /// <summary>
    /// Sets the destination of the character to the given destination and sets variables so that it will start moving
    /// </summary>
    /// <param name="newDest">The new destination</param>
    public void SetDestination(Vector3 newDest)
    {
        m_destination = newDest;
        m_isMoving = true;
        m_isAtDestination = false;
        m_startTime = Time.time;
    }

    /// <summary>
    /// Stops the character from moving by setting boolean variables
    /// </summary>
    public void StopMoving()
    {
        m_isMoving = false;
        m_isAtDestination = false;
    }
}
