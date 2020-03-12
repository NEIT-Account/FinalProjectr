using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;
using SocketIO;
using System;

public struct PlayerSchema
{
    public string id;
    public string name;
    public string password;
    public bool online;
    public int kills;
}
public class Login : MonoBehaviour
{
    [Header("Main Screen")]
    [SerializeField] TextMeshProUGUI errorMsgTMP;
    [SerializeField] TMP_InputField name_IF;
    [SerializeField] TMP_InputField password_IF;
    [Header("Account Creation Screen")]
    [SerializeField] GameObject container;
    [SerializeField] TMP_InputField accountName_IF;
    [SerializeField] TMP_InputField accountPassword_IF;
    [SerializeField] TMP_InputField accountConfirmPassword_IF;
    SocketIOComponent socket;

    bool isAlive;
    float msgDisappearTime = 5;
    float msgTime;

    const string ErrorMSG = "Please provide a login name";
    const string PasswordErrorMSG = "Please provide a password";
    string loginFailMessage;
    private void Awake()
    {
        ErrorPipeline.LogToError("LFNEX", () => { LaunchErrorMesssage(loginFailMessage); });
        ErrorPipeline.LogToError("LFWP", () => { LaunchErrorMesssage(loginFailMessage); });
        ErrorPipeline.LogToError("ACFAE", () => { LaunchErrorMesssage(loginFailMessage); });
    }


    private void Start()
    {
        socket = Network.instance.socket;

        // We will need to remove and turn these off when we 
        socket.On("errorMsg", OnLoginFail);
        socket.On("loginSuccess", OnLoginSuccess);
        socket.On("accountCreated", OnAccountCreated);

    }

  
    private void Update()
    {
        if(isAlive && Time.time > msgTime)
        {
            isAlive = false;
            errorMsgTMP.gameObject.SetActive(false);
        }
    }

    #region Account Creation Methods
    public void OpenCreateAccount()
    {
        container.SetActive(true);
    }
    public void CreateAccountRequest()
    {
        if (!AuthenticateLocally())
            return;

        var newPlayer = new PlayerSchema
        {
            name = accountName_IF.text,
            password = accountPassword_IF.text,
            online = false,
            kills = 0
        };
        Debug.Log("Has Clicked");
        socket?.Emit("createAccountRequest", new JSONObject(JsonUtility.ToJson(newPlayer)));
    }
    private void OnAccountCreated(SocketIOEvent evt)
    {
        container.SetActive(false);
        LaunchSuccessMesssage("Account Created");
    }

    bool AuthenticateLocally()
    {
        if (string.IsNullOrEmpty(accountName_IF.text) ||
            string.IsNullOrEmpty(accountPassword_IF.text) ||
            string.IsNullOrEmpty(accountConfirmPassword_IF.text))
        {
            LaunchErrorMesssage("Please Fill All Field");
            return false;
        }
        if (!accountPassword_IF.text.Equals(accountConfirmPassword_IF.text))
        {
            LaunchErrorMesssage("Passwords dont match");
            return false;
        }

        return true;
    }
    #endregion

    #region Login Methods
    private void OnLoginSuccess(SocketIOEvent evt)
    {
        var id = string.Empty;
        evt.data.GetField(ref id, "id");

        PlayerData.schema.id = id;
        PlayerData.schema.name = name_IF.text;
        SceneManager.LoadScene(1);

    }

    void OnLoginFail(SocketIOEvent evt)
    {
        var msg = string.Empty;
        evt.data.GetField(ref msg, "message");
        var error = msg.Split(':');
        if(error.Length > 1)
            loginFailMessage = error[1];

        ErrorPipeline.FireError(error[0]);
    }

    public void SendLogin()
    {
        if (string.IsNullOrEmpty(name_IF.text))
        {
            LaunchErrorMesssage(ErrorMSG);
            return;
        }

        if (string.IsNullOrEmpty(password_IF.text))
        {
            LaunchErrorMesssage(PasswordErrorMSG);
            return;
        }

        var jsonObj = new JSONObject(JsonUtility.ToJson(new PlayerSchema { name = name_IF.text, password = password_IF.text }));
        socket?.Emit("loginRequest", jsonObj);
    }
#endregion


    public void LaunchErrorMesssage(string msg)
    {
        LaunchMesssage(msg, Color.red);
    }

    public void LaunchSuccessMesssage(string msg)
    {
        LaunchMesssage(msg, Color.green);
    }

    void LaunchMesssage(string msg, Color color)
    {
        isAlive = true;
        errorMsgTMP.color = color;
        errorMsgTMP.text = msg;
        errorMsgTMP.gameObject.SetActive(isAlive);
        msgTime = Time.time + msgDisappearTime;
    }
}
