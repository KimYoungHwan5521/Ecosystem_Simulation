using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tile : CustomObject
{
    public Ground ground;
    public Vector2Int gridPosition;
    public float nutrient;
    public TextMeshProUGUI nutrientText;
    public Grass grass;
    
    protected override void MyUpdate(float deltaTime)
    {
        nutrientText.text = $"{(int)nutrient}";
        nutrientText.gameObject.SetActive(Toolbar.bShowNutrient);
    }

    public void Plant()
    {
        if(grass == null && nutrient > 0)
        {
            grass = PoolManager.Spawn(ResourceEnum.Prefab.Grass, transform.position).GetComponent<Grass>();
            //grass.transform.SetParent(transform, false);

            grass.tile = this;
        }
    }

}
