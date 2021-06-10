using UnityEngine;
using System.Collections;

public class Touchscreen_input : MonoBehaviour {

	private Camera 	cam;
	public  Vector3 push_point;
	public static Touchscreen_input touch_control;
	
	// Use this for initialization
	void Awake () {
		if (touch_control == null)
			touch_control = this;
		else if (touch_control != this)
			Destroy (gameObject);
	}

	void Start ()
	{
		cam = GetComponent<Camera> ();
	}

	Vector2 player_offset()
	{
		if (Master_creator.player) {

			Vector3 revative_point;
			Vector3 player_target;

			player_target = cam.WorldToScreenPoint (Master_creator.player.transform.position);

			player_target = new Vector3 (player_target.x - (Screen.width / 2),
		                            	 player_target.y - (Screen.height / 2),
		                            	 player_target.z);
		

			revative_point = new Vector3 (Input.mousePosition.x - (Screen.width / 2),
		                                  Input.mousePosition.y - (Screen.height / 2),
		                                  cam.transform.position.z);

			push_point = new Vector3(Input.mousePosition.x,
			                         Input.mousePosition.y,
			                         player_target.z);

			push_point = cam.ScreenToWorldPoint(push_point);

			revative_point -= player_target;
			revative_point = revative_point.normalized;

			Debug.DrawLine (Master_creator.player.transform.position, Master_creator.player.transform.position + (revative_point * 20));

			return new Vector2 (revative_point.x, revative_point.y);
		}
		return Vector2.zero;
	}

	// Update is called once per frame
	void Update () {
	

#if UNITY_EDITOR
		if(Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0)){

			Player_controller.control.touch_vector = player_offset();


			if (Input.GetMouseButtonDown(0))
			{
				//Debug.Log ("GetMouseButtonDown");
				Player_controller.control.build_jump  = true;
				Player_controller.control.build_since = Time.time;
			}
			if (Input.GetMouseButtonUp(0))
			{
				//Debug.Log ("GetMouseButtonUp");
				Player_controller.control.build_jump	= false;
				Player_controller.control.jump_release 	= true;
			}
			if (Input.GetMouseButton(0))
			{
				//Debug.Log("00000000");
			}
			
		}
		
#endif
		if (Input.touchCount > 0) {
			foreach (Touch touch in Input.touches) {

				//Debug.Log ("touch.position: " + touch.position);

				Player_controller.control.touch_vector = player_offset();

				if (touch.phase == TouchPhase.Began)
				{
					Player_controller.control.build_jump  = true;
					Player_controller.control.build_since = Time.time;
				}
				if (touch.phase == TouchPhase.Ended)
				{
					Player_controller.control.build_jump	= false;
					Player_controller.control.jump_release 	= true;
				}
				if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
				{}
				if (touch.phase == TouchPhase.Canceled)
				{}

			}
		}
	}
}
