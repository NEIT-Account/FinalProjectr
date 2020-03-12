using System.Collections.Generic;
using System;
using UnityEngine;
public static class ErrorPipeline
{
    static public bool debug = false;
    static Dictionary<string, Action> pipeline = new Dictionary<string, Action>
    {
        { "LFNEX" , ()=>{ if(debug) Debug.Log("Log Fail due to user not existing"); } },
        { "LFWP" , ()=>{ if(debug) Debug.Log("Log Fail due to password mismatch"); } },
        { "ACFAE" , ()=>{ if(debug) Debug.Log("Account Creation Fail due to account existing"); } }
    };

    static public bool LogToError(string err, Action method)
    {
        if (!pipeline.ContainsKey(err))
            return false;
        pipeline[err] += method;
        return true;
    }

    static public bool FireError(string err)
    {
        if (!pipeline.ContainsKey(err))
            return false;

        pipeline[err]?.Invoke();
        // even if the action is null still return true because the error was there
        return true;
    }
}
