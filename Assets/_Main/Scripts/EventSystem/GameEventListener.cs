using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BoolGameEvent: UnityEvent<bool>
{
}

[System.Serializable]
public class IntGameEvent : UnityEvent<int>
{
}

[System.Serializable]
public class GameObjectEvent : UnityEvent<GameObject>
{
}

[System.Serializable]
public class StringEvent: UnityEvent<string>
{
}

public class GameEventListener : MonoBehaviour
{
    public GameEvent Event;
    
    [Header("Void Events")]
    public UnityEvent response;
    [Header("Bool Events")]
    public BoolGameEvent boolResponse;
    [Header("Int Events")]
    public IntGameEvent intResponse;
    [Header("String Events")]
    public StringEvent stringResponse;
    [Header("Game Objects Events")]
    public GameObjectEvent gameObjectResponse;
   
    private void OnEnable() => Event.RegisterListener(this);

    private void OnDisable() => Event.UnregisterListener(this);

    //Void
    public void OnEventRaised() => response.Invoke();
    
    //Bool
    public void OnEventRaised(bool value) => boolResponse.Invoke(value);

    //Int
    public void OnEventRaised(int value) => intResponse.Invoke(value);

    //String
    public void OnEventRaised(string value) => stringResponse.Invoke(value);

    //GameObject
    public void OnEventRaised(GameObject value) => gameObjectResponse.Invoke(value);
}