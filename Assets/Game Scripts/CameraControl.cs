using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraControl : MonoBehaviour
{
    public Camera mainCamera { get; private set; }

    private GameController gameController;

    private PlayerManager playerCharacter;

    private GameObject movingBackground;

    public enum CameraBehavior
    {
        Static = 0,
        Moving = 1,
        FollowingPlayer = 2,
        ReturningToPlayer = 3
    }

    public CameraBehavior behavior; //{ get; private set; }

    private Vector3 upperRightBoundary;
    private Vector3 lowerLeftBoundary;

    private float cameraTravelTime;
    private float timePassed;
    private Vector3 lerpStartPoint;
    private Vector3 focusPoint;

    private float cameraHalfHeight;
    private float cameraHalfWidth;

    private float cameraMarginLeft;
    private float cameraMarginRight;
    private float cameraMarginTop;
    private float cameraMarginBottom;

    private bool initialized = false;

    public void InitializeCamera(GameController gc, PlayerManager player)
    {
        gameController = gc;

        playerCharacter = player;

        mainCamera = this.gameObject.GetComponent<Camera>();

        cameraHalfHeight = mainCamera.orthographicSize;
        cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;

        behavior = CameraBehavior.Static;
    }

    private void Update()
    {
        if (behavior == CameraBehavior.Static)
            return;

        if (behavior == CameraBehavior.FollowingPlayer)
        {
            //Logic to clamp the camera to the edges
            //Check if horizontal input has been held?
            float x = Mathf.Clamp(playerCharacter.transform.position.x, cameraMarginLeft, cameraMarginRight);
            float y = Mathf.Clamp(playerCharacter.transform.position.y, cameraMarginBottom, cameraMarginTop);
            transform.position = new Vector3(x,y,transform.position.z);
        }

        if(behavior == CameraBehavior.ReturningToPlayer)
        {
            Vector3 lerpVector = Vector3.Lerp(lerpStartPoint, focusPoint, timePassed / cameraTravelTime);

            float x = lerpVector.x;
            float y = lerpVector.y;
            float z = lerpVector.z;

            x = Mathf.Clamp(x, cameraMarginLeft, cameraMarginRight);
            y = Mathf.Clamp(y, cameraMarginBottom, cameraMarginTop);

            if (timePassed < cameraTravelTime)
            {
                timePassed = Mathf.Clamp(timePassed, 0, cameraTravelTime);
                Vector3 newPos = Vector3.Lerp(lerpStartPoint, focusPoint, timePassed / cameraTravelTime);

                transform.position = newPos;
                timePassed += Time.deltaTime;
            }
            else
            {
                behavior = CameraBehavior.FollowingPlayer;
            }
        }

        if(behavior == CameraBehavior.Moving)
        {
            if(timePassed < cameraTravelTime)
            {
                timePassed = Mathf.Clamp(timePassed, 0, cameraTravelTime);
                Vector3 newPos = Vector3.Lerp(lerpStartPoint, focusPoint, timePassed / cameraTravelTime);

                transform.position = newPos;
                timePassed += Time.deltaTime;
            }
            else
            {
                behavior = CameraBehavior.Static;
            }
        }   
    }

    public void SetBoundaries(Vector3 lowerLeftCorner, Vector3 upperRightCorner)
    {
        upperRightBoundary = upperRightCorner;
        lowerLeftBoundary = lowerLeftCorner;

        cameraMarginLeft = lowerLeftBoundary.x + cameraHalfWidth;
        cameraMarginRight = upperRightBoundary.x - cameraHalfWidth;
        cameraMarginTop = upperRightBoundary.y - cameraHalfHeight;
        cameraMarginBottom = lowerLeftBoundary.y + cameraHalfHeight;
    }

    public void FollowPlayer()
    {
        behavior = CameraBehavior.FollowingPlayer;
    }
    public void TravelToPlayer(float travelTime)
    {
        behavior = CameraBehavior.ReturningToPlayer;
        cameraTravelTime = Mathf.Clamp(travelTime, 0.1f, 100);
        timePassed = 0;
        lerpStartPoint = transform.position;
        focusPoint = ConvertVector3(playerCharacter.transform.position);
    }
    public void SnapToPlayer()
    {
        behavior = CameraBehavior.FollowingPlayer;
        transform.position = ConvertVector3(playerCharacter.transform.position);
    }

    public void TravelToPoint(Vector3 point, float travelTime)
    {
        behavior = CameraBehavior.Moving;
        cameraTravelTime = Mathf.Clamp(travelTime, 0.1f, 100);
        timePassed = 0;
        lerpStartPoint = transform.position;
        focusPoint = ConvertVector3(point);
    }

    public void SnapToPoint(Vector3 point)
    {
        behavior = CameraBehavior.Static;
        focusPoint = ConvertVector3(point);
        transform.position = focusPoint;
    }

    //Add a function to move between waypoints, to make a showcase of a level for example, or add it to the game controller

    public void AssociateBackground(GameObject background)
    {
        if(movingBackground != null)
            ClearBackground();

        movingBackground = background;

        Vector3 curPos = transform.position;

        transform.position = background.transform.position;

        background.transform.SetParent(transform);

        //The initialization seems to be failing
        foreach (Transform child in background.transform)
        {
            GameObject go = child.gameObject;

            HorizontalParallax hp = go.GetComponent<HorizontalParallax>();
            //Add a vertical parallax get
            BackgroundPushing bgp = go.GetComponent<BackgroundPushing>();

            if (hp != null) 
                hp.InitializeHorParallax(mainCamera);
            //Add a vertical parallax init
            if (bgp != null)
                bgp.InitializeBGPushing(mainCamera);
        }

        transform.position = curPos;
    }

    public void ClearBackground()
    {
        GameObject.Destroy(movingBackground);
        movingBackground = null;
    }

    private Vector3 ConvertVector3(Vector3 input)
    {
        return new Vector3(input.x, input.y, transform.position.z);
    }

}