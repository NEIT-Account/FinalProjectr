using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float distanceAway;
    [SerializeField] float distanceFromGround;


    void Update()
    {
        transform.position = target.position + (-Vector3.forward * distanceAway) + (Vector3.up * distanceFromGround);
        transform.LookAt(target);
    }
}
