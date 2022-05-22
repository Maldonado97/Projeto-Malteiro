using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ocean : MonoBehaviour
{
    [SerializeField] GameObject mainCamera;
    private Material oceanMaterial;

    private void Start()
    {
        oceanMaterial = gameObject.GetComponent<SpriteRenderer>().material;
    }
    private void Update()
    {
        float scale = transform.lossyScale.x;
        transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y);
        oceanMaterial.SetVector("Position2D", new Vector2(transform.position.x/ scale, transform.position.y/ scale));
    }
}
