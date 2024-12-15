using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toolbar : MonoBehaviour
{
    public static bool bShowNutrient = true;

    public Toggle showNutrient;

    public void toggleShowNutrient() { bShowNutrient = !bShowNutrient; }
}
