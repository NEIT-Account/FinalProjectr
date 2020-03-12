using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;

public static class PlayerData
{
    public static PlayerSchema schema;
}

public class PlayerObject : MonoBehaviour
{
    public PlayerController controller = new PlayerController();
    public float maxHealth = 50;
    public TextMeshPro idTM;
    public bool isLocal;

    [SerializeField] Image healthbar;

    Animator animator;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public NetworkMove netMove;

    const string runParam = "IsRunning";
    const string velParam = "velocity";

    float healthbarWidth;
    float currentHealth;
    public float HealthPercentage { get { return currentHealth/maxHealth; } }

    public void Start()
    {
        transform.position = PlayerManager.RandomPosition();
        currentHealth = maxHealth;
        healthbarWidth = healthbar.rectTransform.sizeDelta.x;

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        netMove = GetComponent<NetworkMove>();


        if (isLocal)
        {
            controller.onRayHit += (hit) =>
            {
                agent.SetDestination(hit.point);
                netMove.OnMove(hit.point);
            };
            netMove.Initialize();
            idTM.text = PlayerData.schema.name;
        }
    }

    private void Update()
    {
        controller.HandleInput();
        animator.SetFloat(velParam, agent.velocity.magnitude);
        idTM.transform.parent.LookAt(Camera.main.transform);
    }

    public void TakeDamage(float damage, PlayerObject hitByPlayer)
    {
        var newHealth = currentHealth - damage;
        SetHealth(newHealth);
        netMove.UpdateHealth(newHealth);
        CheckIfAlive(hitByPlayer);
    }

    public void SetHealth(float newHealth)
    {
        currentHealth = newHealth;
        healthbar.rectTransform.sizeDelta = new Vector2(
           healthbarWidth * HealthPercentage,
           healthbar.rectTransform.sizeDelta.y);
    }

    void CheckIfAlive(PlayerObject hitByPlayer)
    {
        if (currentHealth > 0) return;

        SetHealth(maxHealth);
        transform.position = PlayerManager.RandomPosition();
        netMove.socket.Emit("updateKills", new JSONObject(JsonUtility.ToJson(new { name = hitByPlayer.idTM.text })));
        Debug.Log("Sent Update Kills");
    }

 
}
