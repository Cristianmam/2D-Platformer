using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OneWayPlatform : MonoBehaviour
{
    private Collider2D platformCollider;
    public GameController gameController;

    private void Awake()
    {
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
        platformCollider = GetComponent<BoxCollider2D>();
        if (platformCollider == null)
            platformCollider = GetComponent<PolygonCollider2D>();
    }

    private void Update()
    {
        if (!gameController.gameplayActive)
            return;

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            if (Input.GetKey(KeyCode.Space))
                if(Physics2D.BoxCast(platformCollider.bounds.center, platformCollider.bounds.size, 0, Vector2.up, 0.1f, gameController.playerLayer))
                    Physics2D.IgnoreCollision(platformCollider, gameController.playerCharacter.movement.bCol);
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        Physics2D.IgnoreCollision(platformCollider, gameController.playerCharacter.movement.bCol, false);
    }
}
