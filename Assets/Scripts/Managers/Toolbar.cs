using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Toolbar : MonoBehaviour
{
    public static bool bShowNutrient = true;

    public Toggle showNutrient;

    Vector2 navVector;

    private void Update()
    {
        Camera.main.transform.position += (Vector3)navVector * Camera.main.orthographicSize * 0.2f;

    }
    public void ToggleShowNutrient() { bShowNutrient = !bShowNutrient; }

    void OnNavigate(InputValue value)
    {
        navVector = value.Get<Vector2>();
    }

    void OnScrollWheel(InputValue value)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - value.Get<Vector2>().y * 0.01f, 1, 10);
    }
}
