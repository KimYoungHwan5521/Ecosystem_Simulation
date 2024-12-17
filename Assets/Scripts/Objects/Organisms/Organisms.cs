using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Organisms : CustomObject
{
    //public string bioTag;
    [SerializeField] protected float nutrients;
    public float Nutrients => nutrients;

    public virtual float GetEaten(Organisms eater) { return 0; }
}
