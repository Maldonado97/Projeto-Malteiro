using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockZone : MonoBehaviour
{
    public string dockID;
    public GameObject dockedShipSpot;
    [SerializeField] protected DockUIManager dockUIManager;
    [HideInInspector] public bool playerInZone = false;
    private GameObject player = null;

    private void Start()
    {
        FindPlayerGameObject();
    }
    private void Update()
    {
        if(playerInZone && Input.GetKeyDown(KeyCode.V))
        {
            DockShip(player);
            dockUIManager.OnPlayerDocked();
        }
    }
    private void FindPlayerGameObject()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public void DockShip(GameObject shipToDock) //Undock method is exectued by PlayerControl script
    {
        var playerRB = player.GetComponent<Rigidbody2D>();
        player.transform.position = dockedShipSpot.transform.position;
        player.transform.rotation = dockedShipSpot.transform.rotation;
        playerRB.velocity = Vector2.zero;
        playerRB.angularVelocity = 0;
        UIManager.instance.leftEngineSlider.value = 0;
        UIManager.instance.rightEngineSlider.value = 0;
        PlayerControl.instance.playerDocked = true;
    }
    //COLLIDER TRIGGERS
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Ship entered " + dockID);
            playerInZone = true;
            UIManager.instance.ActivateDockPrompt();
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Ship exited " + dockID);
            playerInZone = false;
            UIManager.instance.DeactivateDockPrompt();
        }
    }
}
