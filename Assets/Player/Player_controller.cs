using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using System.Linq;


/* Player_controller is the backbone of the player object.
 * Every movement done by the player object is calculated by this class.
 * Most of the functions in file is aimed at making the player auto traverse the mountainside.
 * User inputs regarding the player is also acounted for in this class.
 */
public class Player_controller : MonoBehaviour {

	public static Player_controller control;

	public float 	stamina,			// Snowtraversal resource.
					floor_speed,		// Length moved on the ground each cycle when in contact.
					ground_speed,		// Currently applied speed when in surface contact.
					jump_power,			// Force done to the player at the jump moment.
					difficulty,			// Overall difficulty of the level.
					build_since,		// Moment of touch.
					grav;				// Gravity scale.

	public int 	 	wins,				// Amount of game won.
				 	losses,				// Amount of game lost.
					coins,				// In game currency.
					indx;				// Index of surface vector beneath the player. 

	public Vector2 	v2_travel_dir,		// Movement direction.
					v2_player_pos,		// Player object transform position.
					touch_vector;		// Touch vector relative to player position.

	public bool     direction_lock,		// Forced player direction.
					build_jump,			// Enables the buildup of jump_power.
					jump_release,		// Active when a touch is released.
					snow_contact,		// Active when the player is in contact with snow.
					won,				// Active when the gane is won.
					lost,				// Active when the gane is lost.
					on_rail,			// On floor movement active. / Walking activated.
					game_paused,		// Game paused, used to stop the core game flow.
					just_paused,		// Used to store speed and direction before stopping player.
					just_un_paused,		// Used to restore speed and directios of player after pause.
					exponent_buildup;   // Toggles a exponential build up of jump_power.

	private int		disable_ground_time,// After landing delay.
					ground_time,		// Time since ground inpact (after jump).
					disable_air_time,	// Pause time in ground detection.
					air_time;			// Time since jump launch.

	private float 	box_col_size, 		// Size of box collider at player top.
					circle_col_radius, 	// Radius of circle collider at player bottom.
					detection_length, 	// Distance of which CircleCast casts its shape./Acceptable error on line searches.
					jump_power_cap,		// Maximum jump_power available for the player.
					jump_power_inc,		// On frame increment of jump_power.
					takeoff_multiplier,	// Muliplier on the takeoff force.
					jump_cost,			// Stamina drained on jump releas.
					snow_speed,			// On ground traversal speed when in contact with snow.
					snow_speed_dim,		// Diminish rate of speed in contact with snow.
					snow_speed_inc, 	// Increase rate of speed when exiting snow.
					snow_drain,			// Current stamina drained when in contact with snow.
					snow_drain_cap,		// Maximum stamina drained pr frame.
					snow_drain_dim, 	// Diminish rate of snow_drain.
					float_power,		// In air steering power.
					jumped_power,		// Power applied to the player on jump.
					arrow_speed,		// Reduction rate of direction arrow.
					arrow_size,			// Size of direction arrow.
					rotate_cap,			// Used to stop the player from leaving the surface on peak sirculations.
					build_time,			// The time used to build up a jump.
					build_time_max;		// The time it takes to fully build up a jump.

	public bool		in_contact,			// Player object touches another collider.
					in_circulation;		// Player currently circulates a point.
				
	private Vector2 v2_beneath_dir, 	// Direction of plain-vector beneath player.
					v2_infront_dir, 	// Direction of plain-vector in front of player.
					circulation_peak, 	// Point circulated by the player.
					c_peak_incline, 	// Inclination cap of circulation.
					lock_direction, 	// Predetermined travel direction.
					lock_target_a,		// Targetline of locked direction (first point).
					lock_target_b, 		// Targetline of locked direction (second point).
					start_pos,			// Player start location.
					stored_velocity,	// Stored player vecoity on pause.
					player_velocity; 	// Current player veocity.

	private Vector3 travel_dir_current;	// Character direction.

	private GameObject 	p_arrow;		// Jump direction indicator.
	private Image[] 	red_arrow;		// Red coat over p_arrow.

	public CircleCollider2D c_col2d;	// Circle collider from player object.

	// Use this for initialization
	void Awake () {
		if (control == null) {
			//DontDestroyOnLoad (gameObject);
			control = this;
		} else if (control != this) {
			Destroy (gameObject);
		}
		System.Environment.SetEnvironmentVariable ("MONO_REFLECTION_SERIALIZER", "yes");
	}

	// Initiates the player object.
	public void Init_player (){

		stamina 			= 100f;
		jump_power			= 0f;
		jump_power_cap 		= 100f;
		floor_speed 		= 0.4f;
		snow_speed_dim 		= 1f;
		snow_speed_inc 		= 1f;
		detection_length 	= 0.3f;
		jump_power_inc		= 1.2f;
		takeoff_multiplier 	= 0.7f;
		jump_cost 			= 0f;
		grav 				= 3.0f;
		disable_ground_time = 5;
		ground_time 		= 5;
		disable_air_time 	= 5;
		air_time 			= 5;
		ground_speed		= 0.4f;
		snow_speed			= 0.2f;
		snow_drain 			= 0f;
		snow_drain_cap		= 0.2f;
		snow_drain_dim		= 0.2f;
		float_power 		= 0.3f;
		jumped_power 		= 0f;
		arrow_speed 		= 5f;
		arrow_size 			= 2.6f;
		rotate_cap			= 0f;
		build_since 		= 0f;
		build_time 			= 0f;
		build_time_max 		= 3f;

		direction_lock 		= false;
		build_jump 			= false;
		jump_release		= false;
		won 				= false;
		lost 				= false;
		game_paused 		= false;
		just_paused 		= false;
		just_un_paused		= false;
		exponent_buildup 	= false;

		in_contact 			= false;
		on_rail 			= false;
		in_circulation 		= false;
		snow_contact 		= false;

		v2_travel_dir		= Vector2.zero;
		v2_player_pos		= Vector2.zero;
		touch_vector		= Vector2.zero;
		player_velocity 	= Vector2.zero;
		
		v2_beneath_dir		= Vector2.zero;
		v2_infront_dir		= Vector2.zero;
		circulation_peak 	= Vector2.zero;
		c_peak_incline		= Vector2.zero;
		lock_direction		= Vector2.zero;
		lock_target_a 		= Vector2.zero;
		lock_target_b 		= Vector2.zero;

		c_col2d = Master_creator.player.GetComponent<CircleCollider2D> ();
		Master_creator.player.GetComponent<Rigidbody2D> ().gravityScale = grav;
		Master_creator.player.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;

		p_arrow = GameObject.Find ("player_body/arrow");

		foreach(Transform child in p_arrow.transform)
		{
			if(child.name == "Canvas")
			{
				red_arrow = child.GetComponentsInChildren<Image>();
			}
		}

		if (Master_creator.first_game) {
			ground_speed = 0f;
		}

		circle_col_radius 	= c_col2d.radius;
		detection_length   += circle_col_radius;
		travel_dir_current 	= Vector3.zero;

		start_pos = new Vector2(60f, circle_col_radius);

		Master_creator.player.transform.position = start_pos;
	}

	/* Binary search to find a key value (key) in an array (A).
	 * Return value: index of item closest to the key value.
	 * Used to find the vector beneath the player object.
	 */
	public static int binary_search(Vector2[] A, float key, int current ,int imin, int imax)
	{
		// test if array is empty
		if (imax < imin)
			// set is empty, so return value showing not found
			return current;
		else
		{
			// calculate midpoint to cut set in half
			int imid = imin + ((imax - imin) / 2);

			if (A[imid].x < key)
			{
				if(A[imid].x > A[current].x)
					return binary_search(A, key, imid, imid + 1, imax);
				else
					return binary_search(A, key, current, imid + 1, imax);
			}
			else if (A[imid].x > key)
				return binary_search(A, key, current, imin, imid -1);
			else
				return imid;
		}
	}

	// Determines if the player is in contact with the ground.
	void detect_surface (){

		in_contact = c_col2d.IsTouching (Master_creator.pc2d);

		if (in_contact) {
			ground_time++;
			on_rail = true;
		}
	}

	/* Used to rotate the playerobject clock-vise around a point (circulation_peak).
	 * Return value is the narmal-vector of the line between the player and the point.
	 */
	Vector2 peak_circulation()
	{
		float rotation_degree = v2_angle(v2_player_pos - circulation_peak) - 90;

		// Keeps the player object from leaving the ground.
		rotation_degree = 	rotation_degree > rotate_cap ?
							rotation_degree : rotate_cap;

		// Keeps the player from leaving the peak.
		if ((v2_player_pos - circulation_peak).y - circle_col_radius > 0)
			rotation_degree = 0;

		return new Vector2 (Mathf.Cos (Mathf.Deg2Rad * rotation_degree),
		                    Mathf.Sin (Mathf.Deg2Rad * rotation_degree));
	}

	/* Calculates the distance from a point to a line segment.
	 * Inputvalues: T = single point (not part of the line),
	 * P1 startpoint of line, P2 end of line.
	 * http://geomalgorithms.com/a02-_lines.html
	 */
	float distance_point_to_line(Vector2 T, Vector2 P1, Vector2 P2)
	{
		Vector2 v = P2 - P1;
		Vector2 w = T  - P1;
		
		float c1 = Vector2.Dot(w,v);
		if ( c1 <= 0 )
			return Vector2.Distance(T, P1);
		
		float c2 = Vector2.Dot(v,v);
		if ( c2 <= c1 )
			return Vector2.Distance(T, P2);
		
		float b = c1 / c2;

		Vector2 Pb;
		v.x = v.x * b;
		v.y = v.y * b;
		Pb.x = P1.x + v.x;
		Pb.y = P1.y + v.y;

		return Vector2.Distance(T, Pb);
	}

	/* Simmilar to distance_point_to_line,
	 * but calculates the distance from a point T to an infinite line (point P1 & P2).
	 * http://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line 
	 */
	float distance_point_to_line_inf(Vector2 T, Vector2 P1, Vector2 P2)
	{
		float 	absolute,
				square;

		absolute = Mathf.Abs(
			((P2.y - P1.y) * T.x) - 
			((P2.x - P1.x) * T.y) +
			(P2.x * P1.y) - 
			(P2.y * P1.x));

		square = Mathf.Sqrt(
			Mathf.Pow((P2.y - P1.y),2) +
			Mathf.Pow((P2.x - P1.x),2));

		return absolute / square;
	}
	
	/* Finds the vector closest to the player.
	 * Input in_reach searches through all vectors under the player (on the right).
	 * Return val is the index from surface_layer.
	 */
	int nearest_vector()
	{
		if (indx + 2 > Master_creator.plain_count)
			return indx;

		Vector2 mid = Master_creator.surface_layer [indx+1];

		if (distance_point_to_line (v2_player_pos, Master_creator.surface_layer [indx], mid) <
		    distance_point_to_line (v2_player_pos, mid, Master_creator.surface_layer [indx+2]))
			return indx;
		else
			return (indx +1);
	}

	/* Return a vector towards the nearest vector.
	 */
	Vector2 towards_floor()
	{
		int i = nearest_vector ();
		int j = i < Master_creator.plain_count ? (i + 1) : i;

		Vector2 vec = 	Master_creator.surface_layer[j] -
						Master_creator.surface_layer[i];

		float rotation_rad 	= Mathf.Deg2Rad * (v2_angle(vec) - 90);
		
		return new Vector2 (Mathf.Cos (rotation_rad),
		                	Mathf.Sin (rotation_rad));
	}

	/* Crates a line used for vicinity detection from a plain (vector a -> b) to the player object.
	 * The line is placed a player-radius away from the vector (a to b).
	 */
	void set_elevated_line(Vector2 a, Vector2 b)
	{
		float rad = Mathf.Deg2Rad * (v2_angle(lock_direction) + 90);
		
		lock_target_a = new Vector2 (a.x + (circle_col_radius * Mathf.Cos (rad)),
		                             a.y + (circle_col_radius * Mathf.Sin (rad)));
		
		lock_target_b = new Vector2 (b.x + (circle_col_radius * Mathf.Cos (rad)),
		                             b.y + (circle_col_radius * Mathf.Sin (rad)));
	}

	/* Directs the player object in a declining direction.
	 * The direction is kept until the player hits a inlining vector.
	 * The function is used to force the player object down slopes.
	 * Returns: true if the direction_lock was initiated, (false otherwise).
	 */
	void lock_setup()
	{
		// Direction lock identified as downwards.
		direction_lock 		= true;

		// Forced to keep current direction.
		lock_direction 		= v2_beneath_dir;

		// Detectionline of the upcomming vector.
		lock_target_a 		= Master_creator.surface_layer[indx + 1];
		lock_target_b 		= Master_creator.surface_layer[indx + 2];

		v2_travel_dir 		= lock_direction;
	}

	/* Checks if direction is locked and
	 * updates active direction lock if active.
	 * Return value -> true when direction is locked.
	 */
	bool update_dir_lock()
	{
		if(direction_lock)
		{// Predetermined path active.
		 // Active path is a decline.

			int close_indx = nearest_vector();

			// Unlocks if close to target line or
			// close to a line to the right of the targeted line.
			if(lock_target_a == Master_creator.surface_layer[close_indx] || (close_indx > indx +1))
			{
				/* Frees the player object from predetermined paths.
				 * Stops upcomming identical upward direction locks.
				 */
				direction_lock = false;
			}
			v2_travel_dir = lock_direction;
		}
		return direction_lock;
	}

	// Angle of vector relative to horizontal right. 
	float v2_angle(Vector2 vec)
	{
		return Vector2.Angle (new Vector2 (1, 0), vec);
	}

	/* Finds the direction the player object must move to traverce the terrain.
	 * Direction found is stored in travel_dir_current.
	 */
	void walk_direction()
	{
		// Checks if the player is on last stretch.
		if (indx + 2 > Master_creator.plain_count) {
			Master_creator.player.GetComponent<Rigidbody2D> ().gravityScale = grav;

			// The last stretch is always straight to the right.
			travel_dir_current = Vector3.right;
			return;
		}

		// Finds level vertecies of the plaines under and in front of the player. 
		Vector2 prev_point   = Master_creator.surface_layer[indx],
				next_point   = Master_creator.surface_layer[indx+1],
				beyond_point = Master_creator.surface_layer[indx+2];

		// Finds the direction of the plain
		v2_beneath_dir = next_point - prev_point;
		v2_travel_dir  = v2_beneath_dir;

		if (!update_dir_lock ())
		{
			// Figures if the player is in contact with snow.
			if(!in_circulation)
				snow_contact = Master_creator.snowy_indexes [indx];

			// Directs the player object in a declining direction if the next stretch is declining.
			if (v2_beneath_dir.y < 0)
				lock_setup ();
			
			if(in_circulation)
			{// Player currently circulates a point.
				if(((v2_player_pos.x - circle_col_radius) >= circulation_peak.x) ||
				   (distance_point_to_line_inf(v2_player_pos, lock_target_a, lock_target_b) <= detection_length))
					in_circulation = false;
				else
					v2_travel_dir  = peak_circulation();
			}
			
			// Only vector in reach is the one beneath the player.
			else if(nearest_vector () - indx == 0)
				v2_travel_dir = v2_beneath_dir;
			
			// First vector to the right is relevant.
			else
			{
				v2_infront_dir = beyond_point - next_point;
				
				float angle_beneath = v2_angle(v2_beneath_dir);
				float angle_infront = v2_angle(v2_infront_dir);
				
				if(v2_beneath_dir.y < 0)
					angle_beneath *= -1;
				if(v2_infront_dir.y < 0)
					angle_infront *= -1;
				
				if(angle_infront < angle_beneath)
				{// Player has reached a peak and needs to circle around it.
					// Initiates peak circulation.

					in_circulation 		= true;
					
					circulation_peak  	= next_point;
					c_peak_incline		= next_point - prev_point;
					
					rotate_cap 			= v2_angle(c_peak_incline);
					v2_travel_dir 		= peak_circulation();
					
					set_elevated_line(next_point, beyond_point);
				}
				else
					v2_travel_dir = v2_infront_dir;
				// Figures if the player is in contact with snow (now testing the plain to the right).
				snow_contact = Master_creator.snowy_indexes [indx + 1];
			}
		}
		else
			detect_snow (); // Figures if the player is in contact with snow.

		// Angle in degrees of the relevant plain.
		float rotation_degree = v2_angle(new Vector2(v2_travel_dir.x, v2_travel_dir.y));
		if(v2_travel_dir.y < 0)
			rotation_degree *= -1;

		travel_dir_current = new Vector3 (Mathf.Cos (Mathf.Deg2Rad * rotation_degree),
		                                  Mathf.Sin (Mathf.Deg2Rad * rotation_degree),
		                         		  0);
	}

	/* Applies a burden on the player when it is in contact with the snow.
	 */
	void snow_burden()
	{
		if (snow_contact) {
			floor_speed = Mathf.MoveTowards(floor_speed, snow_speed,     snow_speed_dim * Time.deltaTime);
			snow_drain 	= Mathf.MoveTowards(snow_drain,  snow_drain_cap, snow_drain_dim * Time.deltaTime);
			stamina    -= snow_drain;
		}else{
			floor_speed = Mathf.MoveTowards(floor_speed, ground_speed, snow_speed_inc * Time.deltaTime);
			snow_drain  = 0f;
		}
	}

	// Figures if the player is in contact with snow.
	void detect_snow()
	{
		snow_contact = Master_creator.snowy_indexes [nearest_vector()];
	}

	// Index of surface vector beneath the player.
	void update_indx()
	{
		indx = binary_search(Master_creator.surface_layer,
		                     v2_player_pos.x,
		                     0, 0, 
		                     (Master_creator.plain_count));
	}

	/* Builds up the jump power.
	 * The jump power can be build up linearly or exponentialy,
	 * this is triggered by exponent_buildup.
	 */
	void jump_build_upp()
	{
		if (exponent_buildup) {
			// Build up is exponential.

			// The build up of this scope is fast at the beginning and
			// becomes slower a by time.

			build_time = Time.time - build_since;

			if (build_time < build_time_max) {
				jump_power = jump_power_cap * Mathf.Log (build_time + 1) / (Mathf.Log (build_time_max + 1));
			} else
				jump_power = jump_power_cap;
		} else {
			// Build up is linear.

			if (jump_power < jump_power_cap)
				jump_power += jump_power_inc;
		}
	}

	/* Moves the direction arrow in the apropriate dirrection.
	 * Position and size indicates the build up od the jump.
	 */
	void direction_arrow()
	{
		Vector3 v3_player_pos, v3_push_point;
		float   arrow_scope;

		if (build_jump) {
			// Assumed to be on ground.

			v3_player_pos = Master_creator.player.transform.position;
			v3_push_point = Touchscreen_input.touch_control.push_point;

			// Locates the arrow in the middle of the player object and the touch point.
			p_arrow.transform.position = (v3_player_pos + ((v3_push_point - v3_player_pos) / 2));
			
			float degree = Vector3.Angle (new Vector3 (1, 0, 0), (v3_push_point - v3_player_pos));
			
			if (v3_push_point.y < v3_player_pos.y)
				degree *= -1;

			if (jump_power > 0f) {

				p_arrow.transform.rotation = Quaternion.AngleAxis (degree, new Vector3 (0, 0, 1));

				arrow_scope = (0.2f + PowerBar.PB.power) * arrow_size;

				if (jump_power < jump_power_cap) 
					p_arrow.transform.localScale = new Vector3(arrow_scope, arrow_scope, 1);

			} else{
				// In air, wind direction arrows.
				if(degree > -90 && degree < 90)
					p_arrow.transform.rotation = Quaternion.AngleAxis(0, new Vector3 (0, 0, 1));
				else
					p_arrow.transform.rotation = Quaternion.AngleAxis(180, new Vector3 (0, 0, 1));
			}

			// Overlaps a red canvas over the arrow sprite.
			red_arrow[0].fillAmount = jump_power / jump_power_cap;

		} else {
			p_arrow.transform.position   = new Vector3 (-500, 0, 0);
			p_arrow.transform.localScale = new Vector3 (1, 1, 1);
		}
	}

	/* Used to remove the direction arrow when gameplay is over.
	 * Shrinks the arrow over time.
	 */
	void shrink_arrow()
	{
		p_arrow.transform.localScale = Vector3.MoveTowards(p_arrow.transform.localScale,
		                                                   Vector3.zero,
		                                                   Time.deltaTime * arrow_speed);
	}

	/* Launches the player object in the air.
	 * Applies a power making it "jump".
	 */
	void jump ()
	{
		jumped_power    = takeoff_multiplier * jump_power;
		Vector2 takeoff_force = touch_vector * jumped_power;

		Master_creator.player.GetComponent<Rigidbody2D> ().velocity    += takeoff_force;
		Master_creator.player.GetComponent<Rigidbody2D> ().gravityScale = grav;

		stamina 		   -= jump_cost;

		build_jump		 	= false;
		jump_release 	 	= false;
		in_contact 		 	= false;
		on_rail 		 	= false;
		in_circulation 	 	= false;
		direction_lock   	= false;
		v2_travel_dir    	= Vector2.zero;
		circulation_peak 	= Vector2.zero;
		travel_dir_current 	= Vector3.zero;

		jump_power  = 0f;
		ground_time = 0;
		air_time 	= 0;
	}

	/* Checks if the player has won the round.
	 */
	void goal_detection()
	{
		if (v2_player_pos.x > (Master_creator.surface_layer [Master_creator.plain_count - 1].x + 10f) && !won) {

			// Stops the players x veclocity.
			Vector2 stop_x_vel = player_velocity;
			stop_x_vel.x = 0f;
			Master_creator.player.GetComponent<Rigidbody2D> ().velocity = stop_x_vel;

			// Stores the win.
			wins++;
			Save ();
			won = true;
		}
	}

	/* Checks if the player has lost the round.
	 */
	void lose_detection()
	{
		if (stamina <= 0 && !won && !lost && snow_contact) {

			// Stops the players x veclocity.
			Vector2 stop_x_vel = player_velocity;
			stop_x_vel.x = 0f;
			Master_creator.player.GetComponent<Rigidbody2D> ().velocity     = stop_x_vel;
			Master_creator.player.GetComponent<Rigidbody2D> ().gravityScale = grav;

			// Stores the loss.
			losses++;
			Save ();
			lost = true;
		}
	}
	
	/* Keeps the player object from jumping off the level.
	 */
	void start_bound()
	{
		if (v2_player_pos.x < start_pos.x) {
			Vector3 wall_bounce = player_velocity;
			if(wall_bounce.x < 0)
			{
				wall_bounce.x *= -1;
				Master_creator.player.GetComponent<Rigidbody2D> ().velocity = wall_bounce;
			}
		}
	}

	/* Steers the player object while in the air.
	 */
	void in_air_maneuver()
	{
		Vector2 steer = Vector2.zero;

		if (build_jump) {
			steer = touch_vector * float_power;
			steer.y = 0f;
			Master_creator.player.GetComponent<Rigidbody2D> ().velocity += steer;
		}
	}

	void Update(){

		player_velocity = Master_creator.player.GetComponent<Rigidbody2D> ().velocity;

		if (game_paused && !won && !lost) {
			// Stores speed and direction variables on pause.
			if (just_paused) {
				just_paused = false;

				stored_velocity = player_velocity;
				Master_creator.player.GetComponent<Rigidbody2D> ().gravityScale = 0f;
				Master_creator.player.GetComponent<Rigidbody2D> ().velocity		= Vector2.zero;
			}

		} else if (Master_creator.player && !won && !lost) {
			// Core game play!!!

			if (just_un_paused) {
				// Restores speed and direction variables on un-pause.
				just_un_paused = false;
				
				Master_creator.player.GetComponent<Rigidbody2D> ().gravityScale = grav;
				Master_creator.player.GetComponent<Rigidbody2D> ().velocity 	= stored_velocity;
			}

			// Player position.
			v2_player_pos = new Vector2 (Master_creator.player.transform.position.x,
			                             Master_creator.player.transform.position.y);

			// Updates the direction arrow.
			direction_arrow ();
			// Checks if the user has won or lost.
			goal_detection  ();
			lose_detection  ();

			// Index of surface vector beneath the player.
			update_indx ();

			// Timeout in surface detection to avoid on_rail triggering on jump.
			if (air_time > disable_air_time) {
				// Checks if the player is in contact with the ground.
				detect_surface ();
			} else
				air_time++;

			// The player object is dirrected accross the ground.
			if (on_rail) {

				// Increases the on release jump force.
				if (build_jump)
					jump_build_upp ();
				else if (jump_release) // Releases the jump.
					jump ();

				// Checks if the playerobject is enabled to walk accross the ground.
				if (ground_time > disable_ground_time) {
					// Finds the direction the player object must move to traverce the terrain.
					walk_direction ();

					// Enables gravity and negates any upwards force grater than the gravity.
					player_velocity.y = player_velocity.y < 0 ? player_velocity.y : 0f;
					Master_creator.player.GetComponent<Rigidbody2D> ().velocity = new Vector2(0f, player_velocity.y);
				}
				else
					detect_snow ();
				// Applies a burden on the player when it is in contact with the snow.
				snow_burden();
			} else {
				// Steers the player object while in the air.
				in_air_maneuver ();
			}

			// Moves the player to its new position.
			Master_creator.player.transform.position += (travel_dir_current * floor_speed);
		
			// Keeps the player object from jumping off the level.
			start_bound ();

		} else if (Master_creator.player) {
			// Gameplay over.
			shrink_arrow();
			detect_surface ();
		}
	}

	/* Stores player data to drive.
	 */
	public void Save()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file;

		// Makes sure there is a file to store the data to.
		if (!File.Exists (Application.persistentDataPath + "/playerInfo.dat"))
			file = File.Create (Application.persistentDataPath + "/playerInfo.dat");
		else
			file = File.Open (Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);

		PlayerData data = new PlayerData ();
		data.wins = wins;
		data.losses = losses;
		data.coins = coins;

		// Stores data to hard storage.
		bf.Serialize (file, data);
		file.Close ();
		file.Dispose ();
	}

	/* Restores player data from drive.
	 */
	public void Load()
	{
		if (File.Exists (Application.persistentDataPath + "/playerInfo.dat")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open (Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
			PlayerData data = (PlayerData)bf.Deserialize(file);
			file.Close ();

			wins	= data.wins;
			losses	= data.losses;
			coins 	= data.coins;
		}
	}
}

// Variables ready for serialization / to be stroed on disk.
[Serializable]
class PlayerData
{
	public int wins;
	public int losses;
	public int coins;
}