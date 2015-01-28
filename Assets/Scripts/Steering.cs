using UnityEngine;
using System.Collections;
//including some .NET for dynamic arrays called List in C#
using System.Collections.Generic;


public class Steering : Object
{
	//movement variables - exposed in inspector panel
	//maximum speed of vehicle
	//public float maxSpeed = 15.0f;
	// maximum force allowed
	//public float maxForce = 10.0f;
	
	//movement variables - updated by this component
	//current speed of vehicle
	//private float speed = 0.0f;
	//change in position per second
	//private Vector3 velocity;
	
	//public Vector3 Velocity {
	//	get { return velocity; }
	//	set { velocity = value;}
	//}
		
	//public float Speed {
	//	get { return speed; }
	//	set { speed = Mathf.Clamp (value, 0, maxSpeed); }
	//}
	
	
	// improve this so we only do it once
	//public float Radius {
	//	get {
	//		float x = renderer.bounds.extents.x;
	//		float z = renderer.bounds.extents.z;
	//		return Mathf.Sqrt (x * x + z * z);
	//	}
	//}

	public static Vector3 Seek (Transform self, Vector3 pos, float speed = 0.0f, float maxSpeed = 15.0f)
	{
		// find dv, the desired velocity
		Vector3 dv = pos - self.position;
		dv.y = 0; //only steer in the x/z plane
		dv = dv.normalized * maxSpeed;//scale by maxSpeed
		dv -= self.forward * speed;//subtract velocity to get vector in that direction
		return dv;
	}
	
	// same as seek pos above, but parameter is game object
	public static Vector3 Seek(Transform self, GameObject gO, float speed = 0.0f, float maxSpeed = 15.0f)
	{
		return Seek(self, gO.transform.position, speed, maxSpeed);
	}

	public static Vector3 Flee(Transform self, Vector3 pos, float speed = 0.0f, float maxSpeed = 15.0f)
	{
		Vector3 dv = self.position - pos;//opposite direction from seek 
		dv.y = 0;
		dv = dv.normalized * maxSpeed;
		dv -= self.forward * speed;
		return dv;
	}

	public static Vector3 Flee(Transform self, GameObject go, float speed = 0.0f, float maxSpeed = 15.0f)
	{
		Vector3 targetPos = go.transform.position;
		targetPos.y = self.position.y;
		Vector3 dv = self.position - targetPos;
		dv = dv.normalized * maxSpeed;
		return dv - self.forward * speed;
	}

	public static Vector3 AlignTo(Transform self, Vector3 direction, float speed = 0.0f, float maxSpeed = 15.0f)
	{
		// useful for aligning with flock direction
		Vector3 dv = direction.normalized;
		return dv * maxSpeed - self.forward * speed;
		
	}

	public static Vector3 Arrival()
	{
		Vector3 dv = Vector3.zero;
			//vector3direction.normalized;



		return dv;
	}
		/*protected function arrival(target:Vector2):Vector2
		{
			var steeringForce:Vector2 = new Vector2();
			var futurePosition:Vector2 = Vector2.add(position, Vector2.divide(Vector2.multiply(fwd, (speed * _manager.futureFrames)), maxSpeed));
			
			//Get the proper direction.
			steeringForce.plusEquals(seek(target));
			
			//Make it a unit vector so we can weaken it based on its distance.
			steeringForce.normalize();
			
			var distance = Vector2.distance(target, futurePosition);
			
			//Adjust the speed based on how far we are from the target. Slowing dist is a tuning knob for changing arrival.
			var ramped_speed = _maxSpeed * (distance / (_manager.pathSize * _manager.slowingDist));
			
								//NOTE: I wanted to make the arrival distance base
								//on the length of the path but there were too many
								//times when it didn't work out so I reverted it.
			
			//We don't need the min anymore because we need to do logic based on which is smaller.
			if (ramped_speed < maxSpeed)
			{
				//We have to force hack and use velocity here. We need the leader to slow down in EVERY regard for reaching the arrival.
				_velocity.normalize();
				_velocity.timesEquals(ramped_speed);
				steeringForce.timesEquals(ramped_speed);
			}
			else
			{
				//If we aren't getting there soon, just go as fast as you can. Next we figure out if its the right direction
				steeringForce.timesEquals(_maxSpeed);
				
				//Need to make a check to see if we're facing towards our objective.
				var futureDist = Vector2.subtract(target, Vector2.add(position, _velocity)).magnitude();
				var currentDist = Vector2.subtract(target, position).magnitude();
				
				//If the direction we're going is greater than our current distance
				if(futureDist > currentDist)
				{
					//trace("Future Dist: " + ((int)(futureDist * 100)) / 100 + "   Current dist: " + ((int)(currentDist * 100)) / 100);
					//We want to cut our velocity down a lot
					_velocity.normalize();
					_velocity.timesEquals(maxSpeed / 10);
					//This will let the leader turn around till he has the proper heading.
				}
			}
			
			if (_manager.debugLevel > 3)
			{
				_manager.drawCircle(_manager.wayArray[1], _manager.pathSize * _manager.slowingDist, 0x006400)
			}
			
			return steeringForce
		}*/

	//Assumtions:
	// we can access radius of obstacle
	// we have CharacterController component
	public static Vector3 AvoidObstacle(Transform self, GameObject obst, float safeDistance, float speed = 0.0f, float maxSpeed = 15.0f, float radius = 1.0f)
	{
		Vector3 dv = Vector3.zero;
		//compute a vector from charactor to center of obstacle
		Vector3 vecToCenter = obst.transform.position - self.position;
		//eliminate y component so we have a 2D vector in the x, z plane
		vecToCenter.y = 0;
		float dist = vecToCenter.magnitude;
		
		//return zero vector if too far to worry about
		if (dist > safeDistance + obst.GetComponent<Dimensions> ().Radius + radius)
			return dv;
		
		//return zero vector if behind us
		if (Vector3.Dot (vecToCenter, self.forward) < 0)
			return dv;
		
		//return zero vector if we can pass safely
		float rightDotVTC = Vector3.Dot (vecToCenter, self.right);
		if (Mathf.Abs (rightDotVTC) > obst.GetComponent<Dimensions> ().Radius + radius)
			return dv;
		
		//obstacle on right so we steer to left
		if (rightDotVTC > 0)
			dv = self.right * -maxSpeed * safeDistance / dist;
		else
		//obstacle on left so we steer to right
			dv = self.right * maxSpeed * safeDistance / dist;
		
		//stay in x/z plane
		dv.y = 0;
		
		//compute the force
		dv -= self.forward * speed;
		//renderer.material.color = Color.yellow;
		return dv;
	}
}