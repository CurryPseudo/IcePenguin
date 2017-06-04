using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMap : MonoBehaviour {
    
    public Vector2 size = Vector2.zero;
    public Vector2 originPoint = Vector2.zero;
    public Platform[,] mapData;
    public List<Platforms> allPlatforms = new List<Platforms>();
    public List<Vector2> penguinPosList = new List<Vector2>();
    private void Awake()
    {
        mapData = new Platform[(int)size.x,(int)size.y];
    }
    public void addPlatforms(Platforms platforms)
    {
        allPlatforms.Add(platforms);
    }
    public void refleshMapData()
    {
        for (int i = 0; i < size.x; i++)
        {
            for(int j = 0; j < size.y; j++)
            {
                mapData[i, j] = null;
            }
        }
        foreach(Platforms platforms in allPlatforms)
        {
            platforms.drawGameMap();
        }
    }
    // Use this for initialization
    void Start () {
        refleshMapData();
	}
	
	// Update is called once per frame
	void Update () {
        refleshMapData();
    }
    private void OnDrawGizmosSelected()
    {
        if (penguinPosList.Count > 0)
        {
            foreach(Vector2 pos in penguinPosList)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(pos + originPoint, pos + originPoint + new Vector2(0.5f, -0.5f));
            }
        }
        if (mapData != null)
        {
            for(int i = 0; i < size.x; i++)
            {
                for(int j = 0; j < size.y; j++)
                {
                    if (mapData[i, j] != null)
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawLine(new Vector2(i,j) + originPoint, new Vector2(i, j) + originPoint + new Vector2(0.5f, -0.5f));
                    }
                }
            }
        }
    }
}
