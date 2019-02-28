using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnScript : MonoBehaviour
{

    public int playersNum;
    public GameObject player;
    GameDataScript gameDataScript;
    // Start is called before the first frame update
    
    void Start()
    {
        gameDataScript = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameDataScript>();
        playersNum = gameDataScript.playersNum;
        for (int x = 0; x < playersNum; x++)
        {
            GameObject newPlayer = (GameObject)GameObject.Instantiate(player, new Vector3(4,3,1), Quaternion.identity);
            newPlayer.name = "Player" + (x + 1).ToString();
            newPlayer.transform.parent = GameObject.Find("Players").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
