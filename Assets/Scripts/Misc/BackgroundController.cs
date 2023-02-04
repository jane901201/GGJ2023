using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public GameObject[] backGrounds;
    public Transform player;
    public float speed = 1.0f;

    void Update()
    {
        foreach (var go in backGrounds)
        {
            go.transform.Translate(new Vector3(player.forward.x, player.forward.y, 0));
            if (go.transform.position.y < player.position.y)
            {
                go.transform.SetPositionAndRotation(new Vector3(0, go.transform.position.y, 0), Quaternion.identity);
            }
            //if (go.transform.position.x < player.position.x)
            //{
            //    go.transform.SetPositionAndRotation(new Vector3(go.transform.position.x, 0, 0), Quaternion.identity);
            //}
        }
    }
}
