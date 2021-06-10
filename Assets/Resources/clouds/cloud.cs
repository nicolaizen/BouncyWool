using UnityEngine;
using System.Collections;

public class cloud : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 n = this.transform.position;
		n.x -= 0.03f;
		this.transform.position = n;
	}
}
