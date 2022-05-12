using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopsShark : MonoBehaviour
{
    [SerializeField] float sharkSpeed = 2;
    [SerializeField] float turnTorque = 60;
    private Rigidbody2D sharkRB;
    public void Start()
    {
        sharkRB = gameObject.GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        MoveForwards();
        TurnLeft();
    }
    public void MoveForwards()
    {
        sharkRB.AddRelativeForce(Vector2.up * sharkSpeed * Time.deltaTime, ForceMode2D.Impulse);
    }
    public void TurnLeft()
    {
        sharkRB.AddTorque(turnTorque * Time.deltaTime);
    }
}
