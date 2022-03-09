using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody2D playerRB;
    private float rigidBodySpeed;
    public float speed;
    public float turnSpeed;
    // Start is called before the first frame update
    void Start()
    {
        playerRB = gameObject.GetComponent<Rigidbody2D>();
        //rigidBodySpeed = playerRB.velocity;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }
    void MovePlayer()
    {
        if (Input.GetKey(KeyCode.W))
        {
            playerRB.AddRelativeForce(Vector2.up * speed * Time.deltaTime, ForceMode2D.Impulse);
        }
        if (Input.GetKey(KeyCode.D))
        {
            playerRB.AddTorque(-turnSpeed * Time.deltaTime, ForceMode2D.Impulse);
        }
        if (Input.GetKey(KeyCode.A))
        {
            playerRB.AddTorque(turnSpeed * Time.deltaTime, ForceMode2D.Impulse);
        }
        if (Input.GetKey(KeyCode.S))
        {
            playerRB.AddRelativeForce(-Vector2.up * speed * Time.deltaTime, ForceMode2D.Impulse);
        }
    }
}
