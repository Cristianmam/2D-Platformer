using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class GameController : MonoBehaviour
{ 
    public PlayerManager playerCharacter { get; private set; }
    public GameObject playerCharacterGO { get; private set; }

    public LevelManager currentLevel { get; private set; }
    private Scene currentLevelScene;

    public Camera mainCamera { get; private set; }
    public CameraControl cameraControl { get; private set; }

    [SerializeField]
    public LayerMask playerLayer;
    [SerializeField]
    public LayerMask wallLayer;
    [SerializeField]
    public LayerMask groundLayer;
    [SerializeField]
    public LayerMask oneWayPlatformLayer;

    //Add UI references


    //Responsible to stopping player input in gameplay (IE: moving, jumping, etc). Meant to be used while in a dialog or in cutscenes
    public bool gameplayActive { get; private set; }

    private void Awake()
    {
        gameplayActive = true;

        SceneManager.LoadScene("CameraScene", LoadSceneMode.Additive);
        
        SceneManager.LoadScene("PlayerScene", LoadSceneMode.Additive);

        //Add other logic scenes here and initialize them at start (UI for example)

    }

    private IEnumerator Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        cameraControl = mainCamera.GetComponent<CameraControl>();

        playerCharacterGO = GameObject.Find("Player");
        playerCharacter = playerCharacterGO.GetComponent<PlayerManager>();

        playerCharacter.InitializePlayer(this);
        //playerCharacterGO.SetActive(false);

        //Wait for this coroutine to finish
        yield return StartCoroutine(LoadLevel(Levels.SampleScene));

        //Place the PC around

        cameraControl.InitializeCamera(this, playerCharacter);
        if (currentLevel.movingBackground)
            cameraControl.AssociateBackground(currentLevel.movingBackground);
        cameraControl.SetBoundaries(currentLevel.levelLowerLeftCorner, currentLevel.levelUpperRightCorner);
        cameraControl.SnapToPlayer();

        yield return new WaitForEndOfFrame();
    }

    private void Update()
    {

    }


    public IEnumerator LoadLevel(string levelName)
    { 
        //Enters the if for no reason
        /*if(currentLevelScene != null)
            SceneManager.UnloadSceneAsync(currentLevelScene);*/
        AsyncOperation levelLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        while (!levelLoad.isDone)
        {
            yield return null;
        }
        currentLevelScene = SceneManager.GetSceneByName(levelName);

        currentLevel = GameObject.Find("Level Manager").GetComponent<LevelManager>();
        currentLevel.InitializeLevel(this);

        yield return new WaitForEndOfFrame();
    }


}
