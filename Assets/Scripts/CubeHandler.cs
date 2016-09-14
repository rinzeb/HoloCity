using UnityEngine;
using System.Collections;

public class CubeHandler : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Hit()
    {
        var rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
    }
}
