using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseParticle : MonoBehaviour
{
    [HideInInspector] public PlayerObject owner;
    [HideInInspector] public Vector3 origin;
    [HideInInspector] public Vector3 direction;
    [SerializeField] float speed;
    [SerializeField] float range;
    [SerializeField] float damage;
    bool isAlive = true;



    private void Update()
    {
        if (isAlive)
        {
            var dir = new Vector3(direction.x, 0, direction.z);
            transform.position += dir * speed * Time.deltaTime;

            var distance = Vector3.Distance(origin, transform.position);
            if (distance >= range)
            {
                isAlive = false;
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerObject>();
        if (player != null && player != owner && isAlive)
        {
            PerformAttack(player);
            isAlive = false;
            Destroy(gameObject);
        }

        
    }

    protected virtual void PerformAttack(PlayerObject target)
    {
        target.TakeDamage(damage, owner);
    }
}
