using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticShooter : MonoBehaviour
{
    public BaseParticle particlePrefab;

    public float fireRate = 1f;
    float fireTime;


    PlayerObject m_player;
    public List<PlayerObject> possibleTargets = new List<PlayerObject>();
    private void Awake()
    {
        m_player = GetComponentInParent<PlayerObject>();
    }

    private void Update()
    {
        if(possibleTargets.Count > 0 && fireTime < Time.time)
        {
            Debug.Log("Here");
            fireTime = Time.time + fireRate;
            SpawnParticle();
        }
    }

    void SpawnParticle()
    {
        var target = GetClosestTarget();
        if (target == null)
        {
            return;
        }
        var direction = target.transform.position - transform.position;
        var origin = transform.position;
        //m_player.transform.LookAt(target.transform);

        var particle = Instantiate(particlePrefab, origin, Quaternion.identity);
        particle.origin = origin;
        particle.owner = m_player;
        particle.direction = direction.normalized;
    }

    PlayerObject GetClosestTarget()
    {
        float closestDistance = float.MaxValue;
        float currentDistance;
        PlayerObject closest = possibleTargets[0];
        foreach(PlayerObject p in possibleTargets)
        {
            if (p == null) continue;

            if((currentDistance = Vector3.Distance(p.transform.position, transform.position)) < closestDistance)
            {
                closestDistance = currentDistance;
                closest = p;
            }
        }

        return closest;
    }

    private void OnTriggerEnter(Collider other)
    {
        var possible = other.GetComponent<PlayerObject>();
        if (possible != null && possible != m_player)
            possibleTargets.Add(possible);


    }

    private void OnTriggerExit(Collider other)
    {
        var possible = other.GetComponent<PlayerObject>();
        if (possible != null && possible != m_player)
            possibleTargets.Remove(possible);

    }
}
