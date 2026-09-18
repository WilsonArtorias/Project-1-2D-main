using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineofSight : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col) {
       /* TODO 2.1: When the player triggers the collider, set the enemy's player reference to the player's Transform. 
        HINT: Unity has a GetComponentInParent<ComponentName>() function that searches the parent object for the ComponentName passed in.
        */
        if (col.CompareTag("Player"))
        {
            Enemy enemy = GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.player = col.transform;
            }
        }

    }
}
