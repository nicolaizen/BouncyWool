using UnityEngine;
using System.Collections;

public class flower_exlplode : MonoBehaviour {

	CircleCollider2D colider;

	bool cluster;
	Animator animator;

	float separation_power;
	// Use this for initialization
	void Start () {
		colider = GetComponent<CircleCollider2D> ();
		cluster = true;
		separation_power = 1000f;
	}
	
	// Update is called once per frame
	void Update () {
		if (cluster) {
			if (colider.IsTouching (Player_controller.control.c_col2d))
			{
				foreach (Transform child in transform)
				{
					Rigidbody2D R2 = child.GetComponent<Rigidbody2D>();
					if(R2)
					{
						R2.gravityScale = 1;
						R2.mass = Random.Range(0.5f, 2f);

						float exlo_rad 	= Mathf.Deg2Rad * (child.transform.rotation.z + 70);
						float inc_x 	= Mathf.Cos (exlo_rad) * separation_power;
						float inc_y 	= Mathf.Sin (exlo_rad) * separation_power;

						R2.AddForce(new Vector3(inc_x, inc_y, 0));

						animator = child.GetComponentInChildren<Animator> ();

						animator.SetBool ("Loose", true);
					}
				}
				cluster = false;
			}
		}
	}
}
