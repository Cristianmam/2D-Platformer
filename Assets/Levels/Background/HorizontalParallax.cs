
using UnityEngine;

public class HorizontalParallax : MonoBehaviour
{
    [SerializeField]
    private float parallaxEffect;

    private Camera cam;
    private float length;
    private float startPos;

    private bool initialized = false;

    public void InitializeHorParallax(Camera camera)
    {
        cam = camera;
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        initialized = true;
    }

    void FixedUpdate()
    {
        if (!initialized)
            return;

        float _temp = cam.transform.position.x * (1 - parallaxEffect);
        float _dist = cam.transform.position.x * parallaxEffect;

        transform.position = new Vector2(startPos + _dist, transform.position.y);

        if (_temp > startPos + length)
            startPos += length;
        else if (_temp < startPos - length)
             startPos -= length;
    }
}
