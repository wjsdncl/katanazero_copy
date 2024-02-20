using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraContorller : MonoBehaviour
{
    public static CameraContorller ins = null;

    [SerializeField] Transform target = null;

    public float maxY;
    public float minY;
    [SerializeField] Vector2 now;

    [SerializeField] float speed = 10f;
    
    private void Awake()
    {
        ins = this;
    }

    private void Update()
    {
        CameraMove();
    }

    private void CameraMove()
    {
        now = target.position;

        if(maxY < now.y)
            now.y = maxY;
        if(minY > now.y)
            now.y = minY;
        
        transform.position = Vector2.Lerp(transform.position, now, Time.deltaTime * speed);
    }
}
