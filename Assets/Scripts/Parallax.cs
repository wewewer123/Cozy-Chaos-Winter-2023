using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeReference] GameObject BackgroundBase;
    [SerializeReference] GameObject ParallaxMountains;
    [SerializeReference] GameObject Moon;
    [SerializeField] float LerpTime = 0.1f;
    private Vector2 LastPosMountains;
    private Vector2 LastPosMoon;
    private void Start()
    {
        LastPosMountains = new Vector2(Camera.main.transform.position.x / 1.5f, -4);
        LastPosMoon = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y);
    }
    void FixedUpdate()
    {
        ParallaxMountains.transform.position = Vector2.Lerp(LastPosMountains, new Vector2(Camera.main.transform.position.x / 1.5f, -4), LerpTime);
        LastPosMountains = ParallaxMountains.transform.position;

        Moon.transform.position = Vector2.Lerp(LastPosMoon, new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y), LerpTime);
        LastPosMoon = Moon.transform.position;
    }
}
