using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : RaycastController
{
	float maxClimbAngle = 55f;
	float maxDescendAngle = 55f;

	float moveObj;


	public CollisionInfo collisions;


	[HideInInspector]
	public Vector2 playerInput;

	public override void Start() 
	{
		base.Start();
	}
	public void Move(Vector2 moveAmount,Vector2 input,bool puxando)
	{
		if (puxando) 
		{
			moveAmount = moveAmount * 0.4f;
		}
		CalculateRaySpacing();
		UpdateRaycastOrigins();
		collisions.Reset();
		collisions.velocityOld = moveAmount;
		playerInput = input;		

		if (moveAmount.y < 0) 
		{
			DescendSlope(ref moveAmount);
		}
		if (moveAmount.x != 0)
		{
			HorizontalCollisions(ref moveAmount);
		}
		if (moveAmount.y != 0)
		{
			VerticalCollisions(ref moveAmount);
		}
		moveObj = moveAmount.x;

			transform.Translate(moveAmount);
	}

	void HorizontalCollisions(ref Vector2 velocity)
	{
		float directionX = Mathf.Sign(velocity.x);
		float rayLength = Mathf.Abs(velocity.x) + skinWidth;

		for (int i = 0; i < horizontalRayCount; i++)
		{
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

			if (hit)
			{
				if (hit.distance == 0)
				{
					continue;
				}


				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxClimbAngle) 
				{
                    if (collisions.descendingSlope) 
					{
						collisions.descendingSlope = false;
						velocity = collisions.velocityOld;
					}
					float distanceToSlopeStart = 0;
					if(slopeAngle != collisions.slopeAngleOld) 
					{
						distanceToSlopeStart = hit.distance - skinWidth;
						velocity.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope(ref velocity, slopeAngle);
					velocity.x += distanceToSlopeStart * directionX;
				}

				if(!collisions.climbingSlope || slopeAngle > maxClimbAngle) 
				{
					velocity.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

                    if (collisions.climbingSlope) 
					{
						velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
					}
					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				}				
			}
		}
	}

	void VerticalCollisions(ref Vector2 velocity)
	{
		float directionY = Mathf.Sign(velocity.y);
		float rayLength = Mathf.Abs(velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++)
		{
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.up * directionY , Color.red);

			if (hit)
			{
				if(hit.collider.tag == "Through") 
				{
					if(directionY == 1 ||hit.distance == 0) 
					{
						continue;
					}
                    if (playerInput.y == -1 ) 
					{
						continue;
					}
					
				}
				velocity.y = (hit.distance - skinWidth) * directionY;

				rayLength = hit.distance;

                if (collisions.climbingSlope) 
				{
					velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x); 
				}

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}
        if (collisions.climbingSlope) 
		{
			float directionX = Mathf.Sign(velocity.x);
			rayLength = Mathf.Abs(velocity.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit) 
			{
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				if(slopeAngle!= collisions.slopeAngle) 
				{
					velocity.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}
 		}
	}

	void ClimbSlope(ref Vector2 velocity,float slopeAngle) 
	{
		float moveDistance = Mathf.Abs(velocity.x);
		float climbVelocity = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
		if(velocity.y <= climbVelocity) 
		{
			velocity.y = climbVelocity;
			velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		}		
	}

	void DescendSlope(ref Vector2 velocity) 
	{
		float directionX = Mathf.Sign(velocity.x);
		Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft);
		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);
        if (hit) 
		{
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if(slopeAngle !=0 && slopeAngle <= maxDescendAngle) 
			{
                if (Mathf.Sign(hit.normal.x) == directionX) 
				{
					if(hit.distance - skinWidth <= Mathf.Tan(slopeAngle* Mathf.Deg2Rad) * Mathf.Abs(velocity.x)) 
					{
						float moveDistance = Mathf.Abs(velocity.x);
						float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
						velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
						velocity.y -= descendVelocityY;

						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true; 
					}
				}
			}
		}
	}
	public void ObjectPush(Transform objeto,Vector2 forcaPush)
	{
		objeto.Translate(forcaPush, Space.World);
	}
	public void DragObjeto(GameObject objeto)
	{
		objeto.transform.Translate(new Vector2(moveObj, 0), Space.World);
				
	}
	

	public struct CollisionInfo
	{
		public bool above, below;
		public bool left, right;

		public bool climbingSlope;
		public bool descendingSlope;
		public float slopeAngle, slopeAngleOld;
		public Vector2 velocityOld;

		public void Reset()
		{
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}		
	}

}
