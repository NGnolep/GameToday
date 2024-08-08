using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public int hp = 50;  // Starting HP for the rock

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if (player.IsDashing())  // Only reduce health if the player is dashing or within the buffer period
            {
                hp -= 10;

                if (hp <= 0)
                {
                    Debug.Log("Rock destroyed!");
                    Destroy(gameObject);  // Destroy the rock
                }
                else
                {
                    Debug.Log("Rock HP: " + hp);
                }
            }
        }
    }
}
