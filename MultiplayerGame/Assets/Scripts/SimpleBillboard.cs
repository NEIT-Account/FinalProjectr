using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBillboard : MonoBehaviour
{
    Transform target;
    private void Awake()
    {
        target = Camera.main.transform;
    }

    void Update()
    {
        transform.LookAt(target);
    }
}
