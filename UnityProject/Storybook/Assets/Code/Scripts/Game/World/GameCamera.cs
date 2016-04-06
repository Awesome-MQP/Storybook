using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour
{
    [SerializeField]
    private float cameraMoveSpeed = 30f;

    [SerializeField]
    private float cameraRotateSpeed = 360.0f;

    [SerializeField]
    private Transform toNode = null;

    private bool traveling = false;

    // Use this for initialization
    void Start()
    {
        //DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (toNode != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, toNode.position,
                cameraMoveSpeed * Time.deltaTime);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, toNode.rotation,
                cameraRotateSpeed * Time.deltaTime);
        }
    }

    // Lerp the camera to another position
    public void trackObject(Camera camToLerp, Transform newCameraNode)
    {
        toNode = newCameraNode;
        traveling = true;
    }
}