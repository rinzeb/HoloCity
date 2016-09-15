using UnityEngine;
using System.Collections;

public class HeliHandler : MonoBehaviour {

    private Vector3 position = new Vector3(0, 0, 0);
    private int counter = 0;


    // Use this for initialization
    void Start () {
        position = new Vector3(0, -0.2F, 4.0F);
    }
	
	// Update is called once per frame
	void Update () {
        counter += 4;
        updatePosition();
    }

    void updatePosition()
    {
        var body = this.GetComponent<Rigidbody>();
        position.x = Mathf.Sin(Mathf.Deg2Rad * counter);
        position.z = Mathf.Cos(Mathf.Deg2Rad * counter);
        body.transform.position = position;
        body.transform.rotation = Quaternion.Euler(0, counter, 0);
    }
}
