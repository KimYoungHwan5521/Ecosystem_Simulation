using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : Animal
{
    protected override void MyStart()
    {
        base.MyStart();
        edibles.Add("Grass");
    }
}
