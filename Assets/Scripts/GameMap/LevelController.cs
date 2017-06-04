using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {
    public int gameProcess = 0;
    GameMap gameMap;
    List<Platforms> allPlatforms;
    public Platforms currentPlatforms = null;
    public Vector2 platformsOriginPos = Vector2.zero;
    public bool hasPlatformsClicked
    {
        get
        {
            return currentPlatforms != null;
        }
    }
    private void Awake()
    {
        gameMap = GetComponent<GameMap>();
    }
    // Use this for initialization
    void Start () {
        allPlatforms = gameMap.allPlatforms;

	}
	
	// Update is called once per frame
	void Update () {
        if(gameProcess == 0)
        {
            foreach(Platforms platforms in allPlatforms)
            {
                if (!platforms.couldSetPos) platforms.setAlpha(0.5f);
            }
            gameProcess = 1;
        }
        else if (gameProcess == 1)
        {
            if (currentPlatforms != null)
            {
                if (Input.GetMouseButton(1) || (Input.GetMouseButtonUp(0) && !currentPlatforms.checkGameMap()))
                {
                    currentPlatforms.setColor(Color.white);
                    currentPlatforms.transform.position = platformsOriginPos;
                    currentPlatforms.downSortOrder();
                    currentPlatforms = null;
                    return;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    currentPlatforms.setColor(Color.white);
                    currentPlatforms.downSortOrder();
                    currentPlatforms = null;
                    return;
                }
                if (Input.GetKeyUp(KeyCode.Q))
                {
                    currentPlatforms.transform.eulerAngles = new Vector3(0, 0, (currentPlatforms.transform.eulerAngles.z + 90) % 360);
                }
                if (Input.GetKeyUp(KeyCode.E))
                {
                    currentPlatforms.transform.eulerAngles = new Vector3(0, 0, (currentPlatforms.transform.eulerAngles.z + 270) % 360);
                }
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                worldPoint.x = (int)worldPoint.x - 1;
                worldPoint.y = (int)worldPoint.y + 1;
                Vector2 coo = worldPoint - gameMap.originPoint;
                if (coo.x >= 0 && coo.x < gameMap.size.x && coo.y >= 0 && coo.y < gameMap.size.y)
                {
                    currentPlatforms.transform.position = worldPoint;
                }
                currentPlatforms.setColor(currentPlatforms.checkGameMap() ? Color.green : Color.red);
            }
            if (Input.GetKeyUp(KeyCode.Return) && currentPlatforms==null)
            {
                gameProcess = 2;
            }
        }else if (gameProcess == 2)
        {
            foreach (Platforms platforms in allPlatforms)
            {
                if (!platforms.couldSetPos) platforms.setAlpha(1f);
            }
            GameObject.Find("Penguin").GetComponent<PenguinStateMachine>().ChangeState(PenguinStateMachine.State.SlidingJumping);
            gameProcess = 3;
        }
	}
}
