using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundPushing : MonoBehaviour
{
    //We want to ask the dev if we are pushing up or down, once we do, the sprite will stay static until the camera reaches a certain margin.
    //Once the margin is reached, it will follow the camera donwards or upwards as required.
    //It will follow the camera back up (or down) to its original Y location.
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

    // Start is called before the first frame update
    void Awake()
    {
        cam = Camera.main;
        height = GetComponent<SpriteRenderer>().bounds.size.y;
        originalY = transform.position.y;
        cameraOriginalY = cam.transform.position.y;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //breaks because of movement along with parent, need to make the object not move on the Y axis with parent
        
        

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
