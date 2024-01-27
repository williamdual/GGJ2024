using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CrowdMember : MonoBehaviour 
{
    public Vector2 targetPosition;
    public float speed = 1.0f;
    public float size; 

    public float health; 

    void Update()
    { 
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed );
    }

    public float GetSize()
    {
        return transform.localScale.x * transform.localScale.y;
    }
}