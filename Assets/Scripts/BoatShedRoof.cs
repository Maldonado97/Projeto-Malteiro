using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatShedRoof : MonoBehaviour
{
    [SerializeField] float fadeLerp = .2f;
    SpriteRenderer boatShedRoof;
    bool playerInShed;
    float boatShedAlfa;

    private void Start()
    {
        boatShedRoof = gameObject.GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (playerInShed && boatShedRoof.color.a > 0)
        {
            lerpRoofIn();
        }
        if(!playerInShed && boatShedRoof.color.a < 1)
        {
            lerpRoofOut();
        }
    }
    private void lerpRoofIn()
    {
        boatShedAlfa = boatShedRoof.color.a;
        boatShedAlfa = Mathf.Lerp(boatShedAlfa, 0, fadeLerp);
        boatShedRoof.color = new Color(boatShedRoof.color.r, boatShedRoof.color.g, boatShedRoof.color.b, boatShedAlfa);
    }
    private void lerpRoofOut()
    {
        boatShedAlfa = boatShedRoof.color.a;
        boatShedAlfa = Mathf.Lerp(boatShedAlfa, 1, fadeLerp);
        boatShedRoof.color = new Color(boatShedRoof.color.r, boatShedRoof.color.g, boatShedRoof.color.b, boatShedAlfa);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInShed = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInShed = false;
        }
    }
}
