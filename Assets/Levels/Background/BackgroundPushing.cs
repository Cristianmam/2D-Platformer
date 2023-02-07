using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundPushing : MonoBehaviour
{
    [SerializeField]
    private bool followsUp;
    [SerializeField]
    private float marginTop;
    [SerializeField]
    private bool followsDown;
    [SerializeField]
    private float marginBottom;
   
    private Camera cam;
    private float cameraOriginalY;
    private float height;
    private float originalY;

    private bool initialized = false;

    public void InitializeBGPushing(Camera camera)
    {
        cam = camera;
        height = GetComponent<SpriteRenderer>().bounds.size.y;
        originalY = transform.position.y;
        cameraOriginalY = cam.transform.position.y;

        initialized = true;
    }

    void FixedUpdate()
    {
        if (!initialized)
            return;

        float _currentY = transform.position.y;

        if(!(FollowDown() || FollowUp()))
            transform.position = new Vector2(transform.position.x, originalY);

    }

    private bool FollowDown()
    {
        if (!followsDown)
            return false;

        float _folRanBot = height - marginBottom + originalY;

        if (cam.transform.position.y < cameraOriginalY)
        {
            //check for margin and if we are at it or exeding it, follow the camera down
            //otherwise follow the camera up
            if (cam.transform.position.y <= originalY - _folRanBot)
            {
                transform.position = new Vector2(transform.position.x, cam.transform.position.y + _folRanBot);
                return true;
            }    
        }

        return false;
    }

    private bool FollowUp()
    {
        if (!followsUp)
            return false;

        float _folRanTop = height - marginTop + originalY;      

        if (cam.transform.position.y > cameraOriginalY)
        {
            //check for margin and if we are at it or exeding it, follow the camera up
            //otherwise follow the camera down
            if (cam.transform.position.y >= originalY + _folRanTop)
            {
                transform.position = new Vector2(transform.position.x, cam.transform.position.y - _folRanTop);
                return true;
            }
        }

        return false;
    }

}
