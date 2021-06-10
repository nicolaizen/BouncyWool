using UnityEngine;
using System.Collections;

public class camera_movement : MonoBehaviour {

	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	public Transform target;
	private Camera cam;
	private float on_ground_view = 130f;
	private float in_air_view = 145f;
	private float down_push = 15f;
	private float push_down_inc = 0.2f;
	private float push_down_dec = 0.5f;
	private float down_pushed;
	private Vector3 landing_lookout;

	void start()
	{
		down_pushed = 0f;
		landing_lookout = Vector3.zero;
		DontDestroyOnLoad(transform.gameObject);
	}

	void Update () 
	{
		if (target)
		{
			cam = GetComponent<Camera>();

			if(!Player_controller.control.on_rail && Player_controller.control.ground_speed > 0f)
			{
				if(down_pushed < down_push)
					down_pushed += push_down_inc;
				
				landing_lookout = new Vector3(0, -down_pushed,0);
				cam.fieldOfView =  Mathf.MoveTowards(cam.fieldOfView, in_air_view, 8 * Time.deltaTime);
			}
			else if(Player_controller.control.ground_speed > 0f)
			{
				if(down_pushed > 0)
					down_pushed -= push_down_dec;
				
				landing_lookout = new Vector3(0, -down_pushed, 0);
				cam.fieldOfView =  Mathf.MoveTowards(cam.fieldOfView, on_ground_view, 17 * Time.deltaTime);
			}

			Vector3 point = cam.WorldToViewportPoint(target.position);
			Vector3 delta = target.position - cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
			Vector3 destination = transform.position + delta + landing_lookout;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);

		}
		else if(Master_creator.player)
			target = Master_creator.player.transform;
	}
}