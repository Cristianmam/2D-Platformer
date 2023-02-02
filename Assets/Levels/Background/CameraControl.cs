using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    private float cameraMoveTime = 1;

    private float timeMoving;
    private Vector3 startPosition;
    private Vector3 endPosition;


    private void Awake()
    {
        timeMoving = 0;
    }

    private void FixedUpdate()
    {
        if (timeMoving < cameraMoveTime)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, timeMoving / cameraMoveTime);
            timeMoving += Time.deltaTime;
        }
    }

    public void LerpLookAt(Vector3 unalteredEnd)
    {
        Vector3 target = new Vector3(unalteredEnd.x, unalteredEnd.y, -1);
        if (Vector3.Distance(target,transform.position) > 0.25f)
        {
            if (timeMoving > cameraMoveTime)
            {
                timeMoving = 0;
            }
            startPosition = transform.position;
            endPosition = target;
        }
        else
        {
            timeMoving = timeMoving + 1;
            transform.position = target;
        }
    }
}
