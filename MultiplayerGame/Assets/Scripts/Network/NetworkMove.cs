using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
public class NetworkMove : MonoBehaviour
{
    public SocketIOComponent socket;

    const string moveMSG = "move";
    const string healthMSG = "updateHealth";

    public void Initialize()
    {
        socket = Network.instance.socket;
        socket?.Emit("start");
    }

    public void OnMove(Vector3 position)
    {
        var jsonObj = new JSONObject(Network.VectorToJson(position));
        socket?.Emit(moveMSG, jsonObj);
    }

    public void UpdateHealth(float health)
    {
        var jsonObj = new JSONObject(string.Format(@"{{""health"":{0}}}", health));
        socket?.Emit(healthMSG, jsonObj);
    }
   
}
