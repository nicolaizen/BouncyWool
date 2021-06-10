using UnityEngine;
using System.Collections;

public class flower_stretch : MonoBehaviour {

	Animator animator;

	void start_stretch()
	{
		animator = GetComponent<Animator> ();
		animator.SetBool ("active", true);
	}

	// Use this for initialization
	void Start () {
		Invoke("start_stretch", Random.Range (0f, 2f));
	}
}
