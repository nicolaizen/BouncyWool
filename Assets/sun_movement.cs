using UnityEngine;
using System.Collections;

public class sun_movement : MonoBehaviour {

	private float end_line;

	private float 	start_x,
					start_y,
					end_x,
					end_y,
					x_decrement,
					y_decrement;

	// Use this for initialization
	void Start () {
		start_x =  50f;
		end_x 	= -37f;

		start_y = 25f;
		end_y 	= 10f;

		x_decrement = end_x - start_x;
		y_decrement = end_y - start_y;

		if (Master_creator.surface_layer.Length > 0)
			end_line = Master_creator.surface_layer [Master_creator.plain_count - 1].x;
		else
			end_line = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (end_line == 0f) {
			if (Master_creator.surface_layer.Length > 0)
				end_line = Master_creator.surface_layer [Master_creator.plain_count - 1].x;
			else
				end_line = 0f;
		} else {
			Vector3 cam_pos = transform.parent.transform.position;
			float level_completion = cam_pos.x / end_line;

			transform.position = new Vector3(start_x + (x_decrement * level_completion) + cam_pos.x,
			                                 start_y + (y_decrement * level_completion) + cam_pos.y,
			                                 0);
		}
	}
}
