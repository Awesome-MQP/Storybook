using UnityEngine;
using System.Collections;

/// <summary>
/// A network mover that will move to special nodes and raise events when doing so.
/// </summary>
public class NetworkNodeMover : NetworkMover
{
    private MovementNode m_node;
    private Vector3 m_lastNodePosition;

    public override Vector3 TargetPosition
    {
        get { return base.TargetPosition; }

        set
        {
            m_node = null;

            base.TargetPosition = value;
        }
    }

    /// <summary>
    /// The node to move towards.
    /// </summary>
    [SyncProperty]
    public MovementNode TargetNode
    {
        get { return m_node; }
        set
        {
            m_node = value;

            if (m_node)
            {
                base.TargetPosition = m_node.transform.position;
                m_lastNodePosition = m_node.transform.position;
            }
            else
            {
                base.TargetPosition = transform.position;
            }

            OnTargetNodeChanged(m_node);

            PropertyChanged();
        }
    }

    protected virtual void LateUpdate()
    {
        if (m_node)
        {
            float distanceSq = (m_node.transform.position - m_lastNodePosition).sqrMagnitude;
            if (distanceSq > 0.001f)//When the node moves more than a fraction from its last position update our positional info.
            {
                m_lastNodePosition = m_node.transform.position;
                base.TargetPosition = m_lastNodePosition;//This will refresh the movement info.
            }
        }
    }

    protected virtual void OnArriveAtNode(MovementNode node)
    {
        //No default implementation.
    }

    protected virtual void OnTargetNodeChanged(MovementNode node)
    {
        //No default implementation.
    }

    protected virtual void OnLeaveNode(MovementNode node)
    {
        //No default implementation.
    }

    protected sealed override void OnArrive()
    {
        if (m_node != null)
        {
            if (m_node.IsMine)//Only the owner is allowed to trigger an event on an object, thus we should respect that rule.
                m_node.Enter(this);

            OnArriveAtNode(m_node);
        }
    }

    protected sealed override void OnLeave()
    {
        if (m_node != null)
        {
            if (m_node.IsMine)//Only the owner is allowed to trigger an event on an object, thus we should respect that rule.
                m_node.Leave(this);

            OnLeaveNode(m_node);
        }
    }
}
