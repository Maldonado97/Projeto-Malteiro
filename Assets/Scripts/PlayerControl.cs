using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody2D playerRB;
    public float forwardImpulse;
    public float turnTorque;
    private float shipHeading;
    // Start is called before the first frame update
    void Start()
    {
        playerRB = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        GetShipHeading();
        //Debug.Log(GetShipSpeed());
    }
    void MovePlayer()
    {
        if (Input.GetKey(KeyCode.W))
        {
            playerRB.AddRelativeForce(Vector2.up * forwardImpulse * Time.deltaTime, ForceMode2D.Impulse);
        }
        if (Input.GetKey(KeyCode.D))
        {
            playerRB.AddTorque(-turnTorque * GetShipSpeed() * Time.deltaTime, ForceMode2D.Impulse);
        }
        if (Input.GetKey(KeyCode.A))
        {
            playerRB.AddTorque(turnTorque * GetShipSpeed() * Time.deltaTime, ForceMode2D.Impulse);
        }
        if (Input.GetKey(KeyCode.S))
        {
            playerRB.AddRelativeForce(-Vector2.up * (forwardImpulse/2) * Time.deltaTime, ForceMode2D.Impulse);
        }
    }
    void GetShipHeading()
    {
        shipHeading = 360 - transform.eulerAngles.z;
        //Debug.Log(Mathf.Sin(shipHeading));
    }
    float GetShipSpeed()
    {
        float forwardSpeed;
        float xSpeed;
        float ySpeed;
        float headingRad;

        xSpeed = playerRB.velocity.x;
        ySpeed = playerRB.velocity.y;
        headingRad = Mathf.Deg2Rad * shipHeading;
        forwardSpeed = (xSpeed * Mathf.Sin(headingRad)) + (ySpeed * Mathf.Cos(headingRad));


        return forwardSpeed;
    }
}
