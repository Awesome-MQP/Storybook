using UnityEngine;
using System.Collections;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class TestSync : PunBehaviour
{
    void Start()
    {
        targetPosition = transform.position;
    }

    [SyncProperty]
    public Vector3 SyncPosition
    {
        get { return transform.position; }
        set
        {
            if (photonView.isMine)
            {
                PropertyChanged();
            }
            else
            {
                targetPosition = value;
            }
        }
    }

    void Update()
    {
        if (photonView.isMine)
        {
            SyncPosition = transform.position;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, 0.3f);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            relevanceToggle = !relevanceToggle;
            photonView.RebuildNetworkRelavance();
        }
    }

    public override bool IsRelevantTo(PhotonPlayer player)
    {
        return relevanceToggle;
    }

    private Vector3 targetPosition;

    private bool relevanceToggle = true;
}
