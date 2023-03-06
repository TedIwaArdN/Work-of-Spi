using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public PlayerControl player;
    public Vector3 offset;
    public float leftBound;
    public float rightBound;

    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector3(1, 1.8f, -10);

    }

    // Update is called once per frame
    void Update()
    {
        if (!player.isDead) {
            FollowThePlayer();
        }

    }

    private void FollowThePlayer() {
        if((player.transform.position.x > leftBound || player.transform.position.y < -4) && player.transform.position.x < rightBound || (player.transform.position.x < -15 && player.transform.position.x > -60.3f)) {
            transform.position = player.transform.position + offset;
        }

        else {
            transform.position = new Vector3(transform.position.x, player.transform.position.y + offset.y, transform.position.z);
        }
        
    }

}
