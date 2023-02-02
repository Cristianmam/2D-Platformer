
using UnityEngine;

public class HorizontalParallax : MonoBehaviour
{
    [SerializeField]
    private float parallaxEffect;

    private Camera cam;
    private float length;
    private float startPos;

    void Awake()
    {
        cam = Camera.main;
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        float _temp = cam.transform.position.x * (1 - parallaxEffect);
        float _dist = cam.transform.position.x * parallaxEffect;

        transform.position = new Vector2(startPos + _dist, transform.position.y);

        if (_temp > startPos + length)
            startPos += length;
        else if (_temp < startPos - length)
             startPos -= length;
    }
}
