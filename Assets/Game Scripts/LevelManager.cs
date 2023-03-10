using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    private GameController gameController;

    [SerializeField]
    private GameObject upRightCorner;
    [SerializeField]
    private GameObject lowLeftCorner;

    public Vector3 levelUpperRightCorner { get; private set; }
    public Vector3 levelLowerLeftCorner { get; private set; }

    [SerializeField]
    private GameObject spawnPointGO;
    [SerializeField]
    private List<GameObject> entryPointsGo;

    public Vector3 spawnPoint { get; private set; }
    //This could be replaced with a struct
    public List<EntryPoint> entryPoints { get; private set; }

    public bool hasSpawnPoint { get; private set; }

    [SerializeField]
    public GameObject movingBackground;

    public void InitializeLevel(GameController gc)
    {
        gameController = gc;

        levelUpperRightCorner = upRightCorner.transform.position;
        levelLowerLeftCorner = lowLeftCorner.transform.position;

        if (spawnPointGO != null)
        {
            hasSpawnPoint = true;
            spawnPoint = spawnPointGO.transform.position;
        }
        else
            hasSpawnPoint = false;
            

        entryPoints = new List<EntryPoint>();
        foreach(GameObject go in entryPointsGo)
        {
            EntryPoint ep = go.GetComponent<EntryPoint>();

            if (ep != null)
            {
                entryPoints.Add(ep);
            }
        }
    }
}
