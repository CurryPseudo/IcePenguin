using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Platform : MonoBehaviour {
    [Serializable]
    public class CooAndType
    {
        public Vector2 coo = Vector2.zero;
        public int blockType = 0;

    }
    public Platforms platforms
    {
        get
        {
            return transform.parent.gameObject.GetComponent<Platforms>();
        }
    }
    public List<CooAndType> relativeCoos = new List<CooAndType>();
    public enum PlatformType
    {
        ice = 0,
        land = 1
    };
    public PlatformType type;
    public float drag
    {
        get
        {
            return drags[(int)type];
        }
    }
    float[] drags = new float[2];
    public float health = 1;
	// Use this for initialization
	void Start () {
        drags[0] = 5;
        drags[1] = 12;
    }
    private void OnMouseDown()
    {
        LevelController levelController = GameObject.Find("GameMap").GetComponent<LevelController>();
       if (!levelController.hasPlatformsClicked && platforms.couldSetPos)
        {
            platforms.upSortOrder();
            levelController.currentPlatforms = platforms;
            levelController.platformsOriginPos = platforms.gameObject.transform.position;
        }
    }
    // Update is called once per frame
    void Update () {

        if (health <= 0)
        {
            Destroy(gameObject);
        }
        GetComponentInChildren<SpriteRenderer>().material.color = new Color(1, 1, 1, (health + 1) / 2);
	}
}
