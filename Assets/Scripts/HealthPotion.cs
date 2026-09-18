using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    #region HealthPotion_variables
    [SerializeField]
    [Tooltip("amount the player heals")]
    private int healAmount;
    #endregion

    #region Heal_functions
    private void OnTriggerEnter2D(Collider2D other)
    {
        /* TODO Part 6.1: If this object collides with the player, heal the player by healAmount by calling the player's Heal() function.
         * After healing this health potion should be destroyed.
         * HINT: The variable, other, contains a reference to the object that collides with this health potion. */
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Heal(healAmount);
                FindObjectOfType<AudioManager>().Play("Potion");
                Destroy(this.gameObject);
            }
        }

    }
    #endregion
}

