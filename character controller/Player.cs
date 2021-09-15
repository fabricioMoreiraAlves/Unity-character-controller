using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{

	public float maxJumpHeight = 1;
	public float minJumpHeight = 0.1f;

	public float timeToJumpApex = .4f;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	float moveSpeed = 2;

	float gravity;

	float maxJumpVelocity;
	float minJumpVelocity;

	Vector2 velocity;
	float velocityXSmoothing;

	Controller2D controller;
	public LayerMask collisionMask;

	public Animator animator;
	bool faceRight = true;

	//bool crouchAnimation = false;
	bool interac = false;
	public bool puxando = false;
	GameObject obj;
	public GameObject death;
	public GameObject nextScreen;
	public string proximaArea;

	Vector2 directionalInput;

	Transform ceilingCheck;


	

	void Start()
	{
		ceilingCheck = this.gameObject.transform.GetChild(0);

		controller = GetComponent<Controller2D>();
		animator = GetComponent<Animator>();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
	}
	void Update()
	{
		if(death.transform.position.y < nextScreen.transform.position.y) 
		{
			if (transform.position.y <= death.transform.position.y)
			{
				SceneManager.LoadScene("Morte");
			}
			if (transform.position.x >= nextScreen.transform.position.x)
			{
				SceneManager.LoadScene(proximaArea);
			}
        }
        else
        {
			if (transform.position.y <= death.transform.position.y)
			{
				SceneManager.LoadScene("menuFim");
			}
		}
		
		float targetVelocityX = directionalInput.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
		velocity.y += gravity * Time.deltaTime;

		controller.Move(velocity * Time.deltaTime, directionalInput,puxando);

		if (controller.collisions.above || controller.collisions.below)
		{
			velocity.y = 0;
		}

        if (puxando) 
		{
			controller.DragObjeto(obj);
        }
		if(controller.collisions.left || controller.collisions.right) 
		{
			animator.SetFloat("speedX", 0);
        }
        else 
		{
			animator.SetFloat("speedX", Mathf.Abs(directionalInput.x));
		}

        if (directionalInput.x > 0 && !faceRight) 
		{
			Flip();
		}
        else if(directionalInput.x < 0 && faceRight)
		{
			Flip();
		}

		if(velocity.y > 0 && !controller.collisions.below) 
		{
			animator.SetBool("jumpUp",true);
			animator.SetBool("jumpDown", false);
		}
		else if (velocity.y < -0.01 && !Physics2D.OverlapCircle(ceilingCheck.position, 0.15f, collisionMask)) 
		{
			animator.SetBool("jumpUp", false);
			animator.SetBool("jumpDown", true);
        }

        if (Physics2D.OverlapCircle(ceilingCheck.position, 0.15f, collisionMask)) 
		{
			animator.SetBool("jumpUp", false);
			animator.SetBool("jumpDown", false);
		}
		if (puxando && Mathf.Abs(directionalInput.x)>0)
		{
			animator.SetBool("push", true);
        }
        else 
		{
			animator.SetBool("push", false);
		}
		if (puxando)
		{
			animator.SetBool("pushI", true);
		}
		else
		{
			animator.SetBool("pushI", false);
		}

	}
	public void SetDirectionalInput(Vector2 input) 
	{
		directionalInput = input;
	}
	public void SetCrouch(bool crouch)
	{
		if (!puxando)
		{
			//crouchAnimation = crouch;
		}
	}
	public void OnJumpInputDown()
	{
		if (!puxando)
		{
			if (Physics2D.OverlapCircle(ceilingCheck.position, 0.15f, collisionMask))
			{
				velocity.y = maxJumpVelocity;
			}
		}
	}

	public void OnJumpInputUp()
	{
		if (!puxando)
		{
			if (velocity.y > minJumpVelocity)
			{
				velocity.y = minJumpVelocity;
			}
		}
	}
	public void OnTriggerEnter2D(Collider2D hit)
	{
        if (hit.tag == "Box") 
		{
			interac = true;
			obj = hit.gameObject;
		}
	}
	public void OnTriggerExit2D(Collider2D hit)
	{
		if (hit.tag == "Box")
		{
			interac = false;
			obj = null;
			puxando = false;
		}
	}
	public void ContatoObjeto() 
	{
        if (interac && !puxando ) 
		{
			puxando = true;
		}
        else 
		{
			puxando = false;
		}
	}
	private void Flip()
	{
        if (!puxando) 
		{
			faceRight = !faceRight;
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}
		
	}

}
