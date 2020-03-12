using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SocketIO;

public class PlayerManager : MonoBehaviour
{
    static SocketIOComponent socket;
    static public Dictionary<string, PlayerObject> players = new Dictionary<string, PlayerObject>();
    static List<PlayerSchema> queuedPlayers = new List<PlayerSchema>();

    static PlayerObject static_prefab;
    public PlayerObject prefab;

    void Start()
    {
        static_prefab = prefab;
        socket = Network.instance.socket;
        foreach(PlayerSchema s in queuedPlayers)
        {
            SpawnPlayer(s);
        }
    }


    static void SpawnPlayer(PlayerSchema player)
    {
        if (players.ContainsKey(player.id))
            return;

        PlayerObject newPlayer = Instantiate(static_prefab, RandomPosition(), Quaternion.identity);
        newPlayer.gameObject.name = player.id;
        newPlayer.idTM.text = player.name;
        players.Add(player.id, newPlayer);
        socket.Emit("start");
    }

    static public void AddPlayer(PlayerSchema data)
    {
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            SpawnPlayer(data);
            return;
        }
        queuedPlayers.Add(data);
    }

    static public Vector3 RandomPosition()
    {
        return new Vector3(
            Random.Range(-20, 20),
            0,
            Random.Range(-20, 20));
    }
}
