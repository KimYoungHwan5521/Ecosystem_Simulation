using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomObject : MonoBehaviour
{
    protected virtual void Start()
    {
        GameManager.Instance.ObjectStart += MyStart;
        GameManager.Instance.ObjectUpdate += MyUpdate;
        GameManager.Instance.ObjectDestroy += MyDestroy;
    }

    protected virtual void MyStart() { }
    protected virtual void MyUpdate(float deltaTime) { }
    protected virtual void MyDestroy() { }
}
