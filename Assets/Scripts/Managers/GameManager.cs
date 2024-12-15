using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CustomStart();
public delegate void CustomUpdate(float deltaTime);
public delegate void CustomDestroy();

public class GameManager : MonoBehaviour
{
    public CustomStart GroundStart;
    public CustomUpdate GroundUpdate;
    public CustomDestroy GroundDestroy;

    public CustomStart ObjectStart;
    public CustomUpdate ObjectUpdate;
    public CustomDestroy ObjectDestroy;

    static GameManager instance;
    public static GameManager Instance => instance;

    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        GroundStart?.Invoke();
        GroundStart = null;
        ObjectStart?.Invoke();
        ObjectStart = null;

        GroundUpdate?.Invoke(Time.deltaTime);
        ObjectUpdate?.Invoke(Time.deltaTime);
    }
}
