using System.Collections;
using UnityEngine;

[RequireComponent (typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    Player player;
	RaycastController ray;
	//bool crouch = false;
   void Start()
   {
        player = GetComponent<Player>();
		ray = GetComponent<RaycastController>();
   }
    void Update()
    {
		Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		player.SetDirectionalInput(directionalInput);

		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			player.OnJumpInputDown();
		}
		if (Input.GetKeyUp(KeyCode.UpArrow))
		{
			player.OnJumpInputUp();
		}
		if (Input.GetKeyDown(KeyCode.W))
		{
			player.OnJumpInputDown();
		}
		if (Input.GetKeyUp(KeyCode.W))
		{
			player.OnJumpInputUp();
		}
		/*
        if (Input.GetKeyDown(KeyCode.LeftControl)) 
		{
			crouch = true;			
		}		
		if (Input.GetKeyUp(KeyCode.LeftControl))
		{
			crouch = false;	
		}
		ray.UpdateColi(crouch);
		*/
		if (Input.GetKeyDown(KeyCode.Space)) 
		{
			player.ContatoObjeto();
		}
	}
}
