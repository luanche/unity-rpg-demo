using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private GameObject cam;

    [SerializeField] private float parallaxEffect;

    private float offsetX;
    private float length;
    void Start()
    {
        cam = GameObject.Find("Main Camera");
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float moveDistance = cam.transform.position.x * parallaxEffect;
        float diffDistance = cam.transform.position.x * (1 - parallaxEffect) - offsetX;

        transform.position = new Vector3(offsetX + moveDistance, transform.position.y);

        if (diffDistance > length)
        {
            offsetX += length;
        }
        else if (diffDistance < -length)
        {
            offsetX -= length;
        }

    }
}
