using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : CustomObject
{
    SpriteRenderer spriteRenderer;
    public Sprite[] grassSprites;
    public Tile tile;

    int growthLevel;
    [SerializeField]int needGrowNutrient;
    int leftNutrient;
    [SerializeField]float growthTime;
    [SerializeField]float curGrowthTime;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void MyUpdate(float deltaTime)
    {
        if(curGrowthTime > growthTime)
        {
            if(growthLevel < 3)
            {
                Grow();
            }
            else
            {
                Propagate();
            }
            curGrowthTime = 0;
        }
        curGrowthTime += deltaTime;
    }

    public void Grow() 
    {
        if(tile.nutrient >= needGrowNutrient)
        {
            tile.nutrient -= needGrowNutrient;
            growthLevel++;
            SpriteUpdate();
        }
        else if(tile.nutrient > 0)
        {
            leftNutrient += tile.nutrient;
            tile.nutrient = 0;
        }
    }
    public void Propagate()
    {
        if (tile.gridPosition.x > 0)
        {
            tile.ground.tiles[tile.gridPosition.x - 1, tile.gridPosition.y].Plant();
        }
        if (tile.gridPosition.x < tile.ground.transform.localScale.x - 1)
        {
            tile.ground.tiles[tile.gridPosition.x + 1, tile.gridPosition.y].Plant();
        }
        if(tile.gridPosition.y > 0)
        {
            tile.ground.tiles[tile.gridPosition.x, tile.gridPosition.y - 1].Plant();
        }
        if(tile.gridPosition.y < tile.ground.transform.localScale.y - 1)
        {
            tile.ground.tiles[tile.gridPosition.x, tile.gridPosition.y + 1].Plant();
        }
    }

    void SpriteUpdate()
    {
        spriteRenderer.sprite = grassSprites[growthLevel];
    }
}
