using System;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;
using UnityEngine.Events;

public delegate void SocketIODelegate(SocketIOEvent socketIOEvent);
public class Network : MonoBehaviour
{
    static public Network instance;

    [HideInInspector] public SocketIOComponent socket;
    [Header("Spawn Data")]
    public PlayerObject localPlayer;

    public event SocketIODelegate onConnected;
    public event SocketIODelegate onDisconnected;
    public event SocketIODelegate onLoginSuccessID;
    public event SocketIODelegate onRequestPosition;
    public event SocketIODelegate onUpdatePosition;
    public event SocketIODelegate onSpawned;
    public event SocketIODelegate onMoved;
    public event SocketIODelegate onUpdateHealth;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if(instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
        }

        socket = GetComponent<SocketIOComponent>();
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        socket.On("open", OnOpen);
        socket.On("spawn", OnSpawned);
        socket.On("move", OnMoved);
        socket.On("disconnected", OnDisconnect);
        socket.On("requestPosition", OnRequestPosition);
        socket.On("updatePosition", OnUpdatePosition);
        socket.On("updateHealth", OnUpdateHealth);

    }



    private void OnLoginSuccessID(SocketIOEvent evt)
    {
        var id = string.Empty;
        evt.data.GetField(ref id, "id");
        PlayerData.schema.id = id;

        onLoginSuccessID?.Invoke(evt);
    }
    private void OnUpdateHealth(SocketIOEvent evt)
    {
        var id = string.Empty;
        var health = 0.0f;

        evt.data.GetField(ref id, "id");
        evt.data.GetField(ref health, "health");
       
        PlayerManager.players[id].SetHealth(health);
    }

    private void OnUpdatePosition(SocketIOEvent evt)
    {
        var id = string.Empty;
        var x = 0.0f;
        var y = 0.0f;

        evt.data.GetField(ref id, "id");
        evt.data.GetField(ref x, "x");
        evt.data.GetField(ref y, "y");
        PlayerManager.players[id].idTM.text = id;
        PlayerManager.players[id].transform.position = new Vector3(x, 0, y);
    }

    protected void OnRequestPosition(SocketIOEvent evt)
    {
        //sends local players position to the server
        socket?.Emit("updatePosition", new JSONObject(VectorToJson(localPlayer.transform.position)));
    }

    protected void OnMoved(SocketIOEvent evt)
    {
        var id = string.Empty;
        var x = 0.0f;
        var y = 0.0f;
        evt.data.GetField(ref id, "id");
        evt.data.GetField(ref x, "x");
        evt.data.GetField(ref y, "y");
        var player = PlayerManager.players[id];
        var pos = new Vector3(x, 0 , y);

        player.agent.SetDestination(pos);
    }

    protected void OnOpen(SocketIOEvent socketEvent)
    {        
        // Use the event to get data from the server about the event we sent ver
        Debug.Log("Connected with event " + socketEvent.name);

        socket?.Emit("updateName", new JSONObject(string.Format(@"{{""name"":{0}}}", PlayerData.schema.name)));

        // we log other actions to this event
        onConnected?.Invoke(socketEvent);
    }

    protected void OnSpawned(SocketIOEvent socketEvent)
    {
        //PlayerObject player = Instantiate(playerPrefab);
        var id = string.Empty;
        var name = string.Empty;
        socketEvent.data.GetField(ref id, "id");
        socketEvent.data.GetField(ref name, "name");
        PlayerManager.AddPlayer(new PlayerSchema { id = id, name = name });

        onSpawned?.Invoke(socketEvent);

    }

    protected void OnDisconnect(SocketIOEvent socketEvent)
    {
        // Use the event to get data from the server about the event we sent ver
        Debug.Log("Disconnected with event " + socketEvent.name);
        var id = string.Empty;
        socketEvent.data.GetField(ref id, "id");
        Destroy(PlayerManager.players[id].gameObject);
        PlayerManager.players.Remove(id);
        // we log other actions to this event
        onDisconnected?.Invoke(socketEvent);
    }

    public static string VectorToJson(Vector3 vector)
    {
        return string.Format(@"{{""x"":{0}, ""y"":{1}}}", vector.x, vector.z);
    }

    public bool NameExists(string name)
    {
        bool exists = false;

        foreach (string key in PlayerManager.players.Keys)
        {
            if (exists)
                continue;
            exists = PlayerManager.players[key].idTM.text.Equals(name);

        }

        return exists;
    }
}
