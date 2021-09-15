using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
	public LayerMask collisionMask;
	[HideInInspector]
	public int horizontalRayCount;
	[HideInInspector]
	public int verticalRayCount;
	public const float skinWidth = .15f;

	const float dstBetweenRays = 0.05f;

	[HideInInspector]
	public float horizontalRaySpacing;
	[HideInInspector]
	public float verticalRaySpacing;

	[HideInInspector]
	public BoxCollider2D coli;

	//float coliSizeStand;
	//float coliOffsetStand;
	//bool crouching = false;


	//Transform ceilingCheck;

	public RaycastOrigins raycastOrigins;

	//Player player;

	public virtual void Start()
	{
		//player = GetComponent<Player>();
		//ceilingCheck =this.gameObject.transform.GetChild(0);


		coli = GetComponent<BoxCollider2D>();

		CalculateRaySpacing();
		//coliSizeStand = coli.size.y;
		//coliOffsetStand = coli.offset.y;
	}

	/*public void UpdateColi(bool crouch)
	{
		if (!player.puxando)
		{
			if (crouch && !crouching)
			{
				coli.size = new Vector2(coli.size.x, coli.size.y * 0.5f);
				coli.offset = new Vector2(coli.offset.x, coli.offset.y * ??);
				crouching = true;
				player.SetCrouch(crouching);

			}
			else if (!crouch && !Physics2D.OverlapCircle(ceilingCheck.position, ??, collisionMask))
			{
				coli.size = new Vector2(coli.size.x, coliSizeStand);
				coli.offset = new Vector2(coli.offset.x, coliOffsetStand);
				crouching = false;
				player.SetCrouch(crouching);
			}
		}
	}
	*/
	public void UpdateRaycastOrigins()
	{
		Bounds bounds = coli.bounds;
		bounds.Expand(skinWidth * -2);

		raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
	}

	public void CalculateRaySpacing()
	{
		Bounds bounds = coli.bounds;
		bounds.Expand(skinWidth * -2);

		float boundsWidth = bounds.size.x;
		float boundsHeight = bounds.size.y;

		horizontalRayCount = Mathf.RoundToInt(boundsHeight / dstBetweenRays);
		verticalRayCount = Mathf.RoundToInt(boundsWidth / dstBetweenRays);


		horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	public struct RaycastOrigins
	{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}
}
