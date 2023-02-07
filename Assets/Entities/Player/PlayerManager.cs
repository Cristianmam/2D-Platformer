using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameController gameController { get; private set; }

    public PlayerMovement movement { get; private set; }
    public Animator animator { get; private set; }

    //Dictates wheter or not player input should be responded to by any other components
    public bool takingInputs { get; private set; }

    public void InitializePlayer(GameController gc)
    {
        takingInputs = true;

        gameController = gc;

        movement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();

        movement.InitializeMovement(this, gameController, animator);
    }

    public void StopMovement()
    {
        takingInputs = false;
    }
    public void ResumeMovement()
    {
        takingInputs = true;
    }
}
