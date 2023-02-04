using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public GameObject[] backGrounds;

    void Update()
    {
        foreach (var go in backGrounds)
        {
            go.transform.Translate(new Vector3(0, 0.01f * Time.deltaTime, 0));
            if (go.transform.position.y < -19)
            {
                go.transform.SetPositionAndRotation(new Vector3(37.8f, go.transform.position.x, 0), Quaternion.identity);

            }
        }
    }
}
