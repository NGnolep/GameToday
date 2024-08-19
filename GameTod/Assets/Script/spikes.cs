using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public float damageAmount = 20f;  // Amount of damage the spike does

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with the player
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ApplyDamage(damageAmount);
        }
    }
}
