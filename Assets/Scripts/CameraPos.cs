using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPos : MonoBehaviour
{
	public GameObject head;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		Vector3 headPos = new Vector3(head.transform.position.x, head.transform.position.y + 0.2f, head.transform.position.z);

		transform.position = headPos;
    }
}
