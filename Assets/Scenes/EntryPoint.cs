using UnityEngine;
using UnityEngine.SceneManagement;


[ExecuteInEditMode]
public class EntryPoint : MonoBehaviour
{
    GameController gameController;

    public enum EntryPointID
    {
        LeftCenter = 0,
        TopLeft = 1,
        TopCenter = 2,
        TopRight = 3,
        RightCenter = 4,
        BottomRight = 5,
        BottomCenter = 6,
        BottomLeft = 7,
        NonCorner1 = 8,
        NonCorner2 = 9,
        NonCorner3 = 10,
        NonCorner4 = 11
    }

    public EntryPointID identifier;

    public enum MovementDirection
    {
        Left = 0,
        Top = 1,
        Right = 2,
        Bottom = 3,
        Static= 4
    }

    public bool allowsExit;

    public MovementDirection exitsTowards;

    public MovementDirection entersFrom;

    public GameObject entersAtGO;
    public Vector3 entersAt { get; private set; }

    public enum EntryMode
    {
        Walking = 0,
        Jumping = 1,
        Falling = 2,
        Standing = 3
    }

    public EntryMode enters;

    public GameObject endsEntryAtGO;
    public Vector3 endsEntryAt { get; private set; }

    public string connectsTo;
    public EntryPointID connectionPoint;
    
    public void InitializeEntryPoint(GameController gc)
    {
        gameController = gc;

        entersAt = entersAtGO.transform.position;

        if (enters != EntryMode.Standing)
            endsEntryAt = endsEntryAtGO.transform.position;
    }
}

