using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Metaskins/GameEvent")]
public class GameEvent : ScriptableObject
{
    private List<GameEventListener> listeners = new List<GameEventListener>();

    //Void
    public virtual void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventRaised();
    }

    //Bool
    public virtual void Raise(bool value)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventRaised(value);
    }

    //Int

    public virtual void Raise(int value)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventRaised(value);
    }

    //String

    public virtual void Raise(string value)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventRaised(value);
    }


    //GameObject
    public virtual void Raise(GameObject value)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventRaised(value);
    }
  
    public virtual void RegisterListener(GameEventListener listener) => listeners.Add(listener);

    public virtual void UnregisterListener(GameEventListener listener) => listeners.Remove(listener);
}