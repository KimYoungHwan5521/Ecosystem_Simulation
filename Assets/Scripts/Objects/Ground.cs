using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public Tile[,] tiles;

    void Start()
    {
        GameManager.Instance.GroundStart += Initiate;
    }

    void Initiate()
    {
        GameManager.ClaimLoadInfo("Tile creating");
        tiles = new Tile[(int)transform.localScale.x, (int)transform.localScale.y];
        for(int i=0; i< tiles.GetLength(0); i++)
        {
            for(int j=0; j< tiles.GetLength(1); j++)
            {
                tiles[i, j] = PoolManager.Spawn(ResourceEnum.Prefab.Tile, new Vector3(i + 0.5f - tiles.GetLength(0) / 2, j + 0.5f - tiles.GetLength(1) / 2, transform.position.z))
                    .GetComponent<Tile>();
                tiles[i,j].ground = this;
                tiles[i, j].gridPosition = new Vector2Int(i, j);
                tiles[i,j].nutrient = Random.Range(0, 1000);
                GameManager.ClaimLoadInfo("Tile creating", i + j, tiles.Length);
            }
        }
        GameManager.CloseLoadInfo();

        for(int i=0; i<10; i++) tiles[Random.Range(0, tiles.GetLength(0)), Random.Range(0, tiles.GetLength(1))].Plant();
        PoolManager.Spawn(ResourceEnum.Prefab.Rabbit, new Vector3(Random.Range(0, tiles.GetLength(0)) + 0.5f - tiles.GetLength(0) / 2, Random.Range(0, tiles.GetLength(1)) + 0.5f - tiles.GetLength(1) / 2, transform.position.z));
    }

}
