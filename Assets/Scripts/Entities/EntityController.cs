using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityController : MonoBehaviour
{
    [HideInInspector] public float health = 100;
    [HideInInspector] public float maxHealth = 100;

    public virtual void DamageEntity(float damage)
    {
        health -= damage;
        //Debug.Log($"{gameObject.name} took {damage} points of damage");
        if(health <= 0)
        {
            KillEntity();
        }
    }
    public virtual void KillEntity()
    {
        gameObject.SetActive(false);
    }
}
