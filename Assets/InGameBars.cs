using UnityEngine;
using System.Collections;

public class InGameBars : MonoBehaviour {

	public static InGameBars ingamebars;

	public bool Active;

	Animator animator;

	void Awake () {
		if (ingamebars == null)
			ingamebars = this;
		else if (ingamebars != this)
			Destroy (gameObject);
	}

	public void ActivateBars(){
		animator.SetBool  ("Active", true);
		/*foreach (Transform child in transform){
			child.gameObject.SetActive(true);
		}*/
	}

	public void DeActivateBars(){
		animator.SetBool  ("Active", false);
		/*
		foreach (Transform child in transform){
			child.gameObject.SetActive(false);
		}*/
	}

	// Use this for initialization
	void Start () {
		Active = true;
		animator = GetComponent<Animator> ();
	}
}
