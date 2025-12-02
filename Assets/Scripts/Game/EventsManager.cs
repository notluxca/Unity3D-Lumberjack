using System;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    private static EventsManager _instance;
    public static EventsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<EventsManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.hideFlags = HideFlags.HideAndDontSave;
                    obj.name = "EventsManager";
                    _instance = obj.AddComponent<EventsManager>();
                }
            }
            return _instance;
        }
    }

    public static Action playerCollectedMoney;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
}

