using UnityEngine;
using System.Collections;

public class MiniMapPlayerPosition : MonoBehaviour
{
    Transform player;

    [SerializeField]
    Transform playerSprite;

    [SerializeField]
    float height = 8;

    Vector3 position;

    void Start()
    {
        player = GameObject.FindWithTag("Avatar").transform;

        if (player == null)
        {
             Debug.LogError("The Avatar tag was not found in the scene!");
        }
       
    }
    
    void Update()
    {
        PositionSprite();
    }


    void PositionSprite()
    {
        position = player.position;
        position.y = height;
        playerSprite.position = position;
    }

}
