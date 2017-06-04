using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platforms : MonoBehaviour {
    public bool couldSetPos = false;
    GameMap gameMap;
    GameMap safeGameMap
    {
        get
        {
            if (gameMap == null)
            {
                gameMap = GameObject.Find("GameMap").GetComponent<GameMap>();
            }
            return gameMap;
        }
    }
    private void Awake()
    {
        
    }
    // Use this for initialization
    void Start () {
        safeGameMap.addPlatforms(this);
        
    }
	public void drawGameMap()
    {
        if (gameMap.GetComponent<LevelController>().currentPlatforms == this) return;
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            Platform platform = child.GetComponent<Platform>();
            foreach (Platform.CooAndType relativeCoo in platform.relativeCoos)
            {
                Vector2 resultCoo = getWorldCoo(relativeCoo.coo);
                safeGameMap.mapData[(int)resultCoo.x, (int)resultCoo.y] = platform;
            }
        }
    }
    public bool checkGameMap()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            Platform platform = child.GetComponent<Platform>();
            foreach (Platform.CooAndType relativeCoo in platform.relativeCoos)
            {
                Vector2 resultCoo = getWorldCoo(relativeCoo.coo);
                if (safeGameMap.mapData[(int)resultCoo.x, (int)resultCoo.y] != null)
                {
                    Platform otherPlatform = safeGameMap.mapData[(int)resultCoo.x, (int)resultCoo.y];
                    foreach(Platform.CooAndType otherCoo in otherPlatform.relativeCoos)
                    {
                        if(otherPlatform.platforms.getWorldCoo(otherCoo.coo) == resultCoo)
                        {
                            if (relativeCoo.blockType != 0 && otherCoo.blockType != 0)
                            {
                                if (Mathf.Abs(getBlockType(relativeCoo.blockType) - otherPlatform.platforms.getBlockType(otherCoo.blockType)) != 2)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    
                }
                if (gameMap.penguinPosList.Count > 0)
                {
                    foreach (Vector2 pos in gameMap.penguinPosList)
                    {
                        if (resultCoo == pos) return false;
                    }
                }
            }
            
        }
        return true;
    }
    public void setColor(Color color)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.GetComponentInChildren<SpriteRenderer>().color = color;
        }
        return;
    }
    public void upSortOrder()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.GetComponentInChildren<SpriteRenderer>().sortingOrder += 5;
        }
    }
    public void downSortOrder()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.GetComponentInChildren<SpriteRenderer>().sortingOrder -= 5;
        }
    }
    public void setAlpha(float alpha)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, alpha);
        }
    }
    public int getBlockType(int type)
    {
        if (type == 0) return 0;
        type = (type - 1 + (int)transform.eulerAngles.z / 90) % 4 + 1;
        return type;
    }
    public Vector2 getWorldCoo(Vector2 coo)
    {
        Vector2[] originalPoints = { new Vector2(0, 0), new Vector2(0, 1), new Vector2(-1, 1), new Vector2(-1, 0) };
        int index = (int)transform.eulerAngles.z / 90;
        Vector2 originalPoint = originalPoints[index];
        coo.x = Mathf.Sign(transform.localScale.x) * (coo.x - (0 - 0.5f)) + 0 - 0.5f;
        coo.y = Mathf.Sign(transform.localScale.y) * (coo.y - (0 - 0.5f)) + 0 - 0.5f;
        Vector2 tranCoo = Vector2.zero;
        if(index == 0)
        {
            tranCoo = coo;
        }else
        if(index == 1)
        {
            tranCoo.x = -coo.y;
            tranCoo.y= coo.x;
        }else if(index == 2)
        {
            tranCoo.x = -coo.x;
            tranCoo.y = -coo.y;
        }else if( index == 3)
        {
            tranCoo.x = coo.y;
            tranCoo.y = -coo.x;
        }
        
        
        tranCoo += originalPoint;
        tranCoo += new Vector2(transform.position.x, transform.position.y);
        tranCoo -= safeGameMap.originPoint;
        return tranCoo;
    }
	// Update is called once per frame
	void Update () {
		
	}
}
