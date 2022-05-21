using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityController : MonoBehaviour
{
    [HideInInspector] public float Health = 100;

    public void DamageEntity(float damage)
    {
        Health -= damage;
        Debug.Log($"{gameObject.name} took {damage} points of damage");
    }
}
