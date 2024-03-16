using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayersControllers : MonoBehaviour
{
    public float speed = 5;
    private Vector2 MovementInput;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(MovementInput.x, MovementInput.y, 0) * speed * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    { 
        MovementInput = ctx.ReadValue<Vector2>();
    }
}
