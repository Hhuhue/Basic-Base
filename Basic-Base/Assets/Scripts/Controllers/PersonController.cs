using UnityEngine;
using System.Collections;

public class PersonController : MonoBehaviour {
    
	void Start () {
	
	}
	
	void Update ()
    {
        Vector3 move = Vector3.zero;

        float speed = 1;

        if (Input.GetKey(KeyCode.LeftArrow)) move.x = -speed;
        if (Input.GetKey(KeyCode.RightArrow)) move.x = speed;
        if (Input.GetKey(KeyCode.DownArrow)) move.y = -speed;
        if (Input.GetKey(KeyCode.UpArrow)) move.y = speed;

	    transform.position += move * Time.deltaTime;
    }

    void OnMouseDown()
    {
        Debug.Log("Person down");
    }
}
