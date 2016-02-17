using UnityEngine;
using System.Collections;
using Photon;

/// <summary>
/// A class for moving an object to a target over a network.
/// </summary>
public class NetworkMover : PunBehaviour
{
    [SerializeField]
    private Vector3 m_targetPosition;

    [SerializeField]
    private bool m_isAtTarget;

    [SerializeField]
    private float m_maxSpeed = 1.0f;

    [SerializeField]
    private float m_atTargetThreashHold = 0.1f;

    private Vector3 m_velocity;

    [SyncProperty]
    public virtual Vector3 TargetPosition
    {
        get { return m_targetPosition; }
        set
        {
            if (m_isAtTarget)
                OnLeave();

            m_targetPosition = value;
            m_isAtTarget = false;

            OnTargetChanged();

            PropertyChanged();
        }
    }

    [SyncProperty]
    public Vector3 Position
    {
        get { return transform.position; }
        set
        {
            transform.position = value;
            PropertyChanged();
        }
    }

    public Vector3 Velocity
    {
        get { return m_velocity; }
    }

    protected virtual void OnArrive()
    {
    }

    protected virtual void OnLeave()
    {
    }

    protected virtual void OnTargetChanged()
    {
    }

    protected virtual void Update()
    {
        if (m_isAtTarget)
        {
            m_velocity = Vector3.zero;
            return;
        }

        Vector3 currentPosition = Position;

        Vector3 newPosition = Vector3.MoveTowards(currentPosition, m_targetPosition, m_maxSpeed * Time.deltaTime);
        if (Vector3.Distance(newPosition, m_targetPosition) <= m_atTargetThreashHold)
        {
            m_velocity = Vector3.zero;
            m_isAtTarget = true;
            if (IsMine)
            {
                OnArrive();
                photonView.RPC("_RPCArrive", PhotonTargets.Others);
            }
        }
        else
        {
            m_velocity = (newPosition - currentPosition) * Time.deltaTime;
        }

        transform.position = newPosition;
    }

    [PunRPC]
    public void _RPCArrive()
    {
        OnArrive();
    }

    public bool IsAtTarget
    {
        get { return m_isAtTarget; }
        set { m_isAtTarget = value; }
    }
}
