using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BirdController : MonoBehaviour {

	CircleCollider2D colider;

	Animator animator;

	public bool Stomped,
				Throw,
				ascending;

	Vector3 CurrentPos;

	public float 	startheight,
					TrailThreshold,
					WaitTimerInit,
					TrailSpeedX,
					TrailSpeedAscendY,
					TrailSpeedDesendY,
					TrailTime,
					BounceBoost,
					ThrowPower;

	private float 	WaitTimer,
					travelLength;

	private int snow_index_start,
				snow_index_end,
				snow_plain_size,
				TrailIndex;

	public List<Vector2> Trail;

	private Vector2 Destination,
					DestinationPrev;

	// Use this for initialization
	void Start () {

		startheight 		= 10f;
		TrailThreshold 		= 1f;
		WaitTimer			= 0f;
		WaitTimerInit 		= 2f;
		TrailSpeedX			= 5f;
		TrailSpeedAscendY 	= 5f;
		TrailSpeedDesendY	= 5f;
		BounceBoost			= 5f;
		ThrowPower 			= 75f;

		colider  = GetComponent<CircleCollider2D> ();
		animator = GetComponent<Animator> ();

		Stomped 	= false;
		Throw		= false;
		ascending 	= true;

		CurrentPos = transform.position;
		CurrentPos.y += startheight;
		transform.position = CurrentPos;

		snow_index_start = Player_controller.binary_search(Master_creator.surface_layer,
		                                                   CurrentPos.x,
		                                				   0, 0, 
		                                				   (Master_creator.plain_count));
		for (snow_index_end = snow_index_start;
		     Master_creator.snowy_indexes[snow_index_end];
		     snow_index_end++);

		snow_plain_size = snow_index_end - snow_index_start;

		Trail = new List<Vector2>();
		int last = snow_index_start;
		Vector2 TrailPoint;
		for (int i = snow_index_start; i <= snow_index_end; i++) {
			if((i != snow_index_start) &&
			   (i != snow_index_end) &&
			   (Master_creator.surface_layer[i].y < Master_creator.surface_layer[last].y))
				continue;
			TrailPoint = Master_creator.surface_layer[i];
			TrailPoint.y += startheight;

			Trail.Add (TrailPoint);
			last = i;
		}
		TrailIndex 	= 1;
		if (Trail.Count > 2) {
			DestinationPrev = Trail [0];
			Destination = Trail [1];
		}
	}

	void TrailMovement(){
		if(Trail.Count < 2)
			return;
		CurrentPos = transform.position;
		if(((Vector2)CurrentPos - Destination).magnitude < TrailThreshold)
		{
			if(ascending)
			{
				TrailIndex++;
				if(TrailIndex == (Trail.Count-1))
				{
					ascending = false;
					// Create wait timer.
					WaitTimer = WaitTimerInit;
				}
			}
			else
			{
				TrailIndex--;
				if(TrailIndex == 0)
				{
					ascending = true;
					// Create wait timer.
					WaitTimer = WaitTimerInit;
				}
			}
			TrailTime = Time.time;
			// Find new destination.
			DestinationPrev = Destination;
			Destination = Trail [TrailIndex];
		}
		WaitTimer--;
		if(WaitTimer < 0f)
		{
			float travelX = Destination.x - DestinationPrev.x;
			float travelY = Destination.y - DestinationPrev.y;

			travelLength = new Vector2(travelX, travelY).magnitude;
			float currentTime  = Time.time;

			CurrentPos.x = Mathf.Lerp(DestinationPrev.x, Destination.x, (currentTime - TrailTime) * (TrailSpeedX / travelLength));
			CurrentPos.y = Mathf.Lerp(DestinationPrev.y, Destination.y, (currentTime - TrailTime) * (TrailSpeedAscendY / travelLength));

			transform.position = CurrentPos;
		}
	}

	// Update is called once per frame
	void Update () {

		TrailMovement ();

		if (colider.IsTouching (Player_controller.control.c_col2d)) {
			Vector2 new_player_speed = Master_creator.player.GetComponent<Rigidbody2D> ().velocity;
			if(new_player_speed.y < 0)
			{
				new_player_speed.y = new_player_speed.y * -1;
				new_player_speed.y += BounceBoost;
				if(ascending)
					new_player_speed.y += (TrailSpeedX / travelLength);
				Master_creator.player.GetComponent<Rigidbody2D> ().velocity = new_player_speed;
				Stomped = true;
				Throw 	= false;
			}else if(Player_controller.control.v2_player_pos.y < transform.position.y){
				Stomped = false;
				Throw 	= true;
				Master_creator.player.GetComponent<Rigidbody2D> ().velocity =
					(new Vector2(Master_creator.player.GetComponent<Rigidbody2D> ().velocity.x, ThrowPower));
			}
		}
		if (!animator)
			animator = GetComponent<Animator> ();
		animator.SetBool ("Stomped", Stomped);
		animator.SetBool ("Throw", 	 Throw);
	}
}
