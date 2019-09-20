using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightFollow : MonoBehaviour
{
    public GameObject target;
    public Vector3 offset;
    public Quaternion rotation;

    // Use this for initialization
    void Start()
    {
        offset = new Vector3(0, 8, 0);
        rotation = Quaternion.Euler(90, 0, 0);
        transform.rotation = rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.parent.position + offset;
    }
}
