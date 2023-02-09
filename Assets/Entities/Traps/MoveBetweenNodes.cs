using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBetweenNodes : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> nodesGO;
    private List<Vector3> nodes;
    [SerializeField]
    private bool patrols = true;
    [SerializeField]
    private float waitBetweenNodes = 0;
    [SerializeField]
    private float speed = 5;

    private float waitTimer = 0;

    private Vector3 startingFrom;
    private int movingTo;


    private bool traversingForward = true;

    private float timeToTravel;
    private float timeTraveling;

    private bool done = false;

    private void Awake()
    {
        if (nodesGO == null || nodesGO.Count == 0)
            done = true;

        if (nodesGO.Count!=0)
        {
            nodes = new List<Vector3>();
            foreach (GameObject go in nodesGO)
                nodes.Add(go.transform.position);

            startingFrom = nodes[0];
            movingTo = 1;

            timeToTravel = Vector3.Distance(startingFrom, nodes[movingTo]) / speed;
            timeTraveling = 0;

            
        }
    }

    private void Update()
    {
        if (done)
            return;

        if(transform.position != nodes[movingTo])
        {
            timeTraveling = Mathf.Clamp(timeTraveling + Time.deltaTime, 0, timeToTravel);
            transform.position = Vector3.Lerp(startingFrom, nodes[movingTo], timeTraveling / timeToTravel);
        }
        else
        {
            SetNextNode();
        }
    }

    private void SetNextNode()
    {
        if (waitTimer < waitBetweenNodes && waitBetweenNodes != 0)
        {
            waitTimer += Time.deltaTime;
            return;
        }

        waitTimer = 0;

        startingFrom = nodes[movingTo];
        timeTraveling = 0;
        if (traversingForward)
        {
            if (movingTo + 1 < nodes.Count)
            {
                movingTo++;
            }
            else
            {
                if (patrols)
                {
                    traversingForward = false;
                    movingTo--;
                }
                else
                {
                    done = true;
                }
            }
        }
        else
        {
            if(movingTo != 0)
            {
                movingTo--;
            }
            else
            {
                traversingForward = true;
                movingTo++;
            }
        }
        timeToTravel = Vector3.Distance(startingFrom, nodes[movingTo]) / speed;
    }
}
