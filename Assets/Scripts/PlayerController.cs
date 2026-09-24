using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region Movement_variables
    public float moveSpeed = 2f;
    float x_input;
    float y_input;
    #endregion

    #region Physics_components
    Rigidbody2D PlayerRB;
    #endregion

    #region Health_variables
    public float maxHealth = 5;
    float currHealth = 5;
    public Slider HPSlider;
    #endregion

    #region Animation_components
    Animator anim;
    #endregion

    #region dash
    private float dashSpeed = 250f;     
    private float dashDuration = 0.2f;   
    private float dashCooldown = 1.5f;   
    private bool isDashing = false;                        
    private bool canDash = true;

    #endregion
    

    #region Unity_functions
    private void Awake() {
        /* TODO: Update your Awake function to initialize all variables needed. This includes your attackTimer, and your HPSlider.value.*/
        /* TODO 4.1: Set HPSlider.value to a ratio between the 
            player's current health and maximum health. */
            
        PlayerRB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currHealth = maxHealth;

        attackTimer = 0f;
        if (HPSlider != null)
        {
            HPSlider.value = currHealth / maxHealth;
        }
    }
    private void Update() {
        /*TODO 1.1: Write an Update function that will call the Move() helper function while also updating the x_input and y_input values.
        You will also need to edit this function when you call attacks, and interacting with chests.*/
        if (isAttacking)
        {
            PlayerRB.linearVelocity = Vector2.zero;
            return;
        }
        /* TODO 1.2: Check if the attack key is being pressed. If so, attack by calling your Attack() function
         * IMPORTANT:  You will need to use `Input.GetKeyDown(KeyCode key)` to determine if the key is being pressed
        */
        x_input = Input.GetAxisRaw("Horizontal");
        y_input = Input.GetAxisRaw("Vertical");
        
        Move();
        
        /* TODO 1.3: Modify your attack conditional statement to only attack when attackTimer < 0. Otherwise, decrement the attackTimer. */
        if (attackTimer < 0)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                Attack();
            }
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
        if ((Input.GetKeyDown(KeyCode.Space)) && canDash)
        {
            StartCoroutine(DashRoutine());
        }
    }
    #endregion



    #region Attack_variables
    public float damage = 2;
    public float attackSpeed = 1;
    float attackTimer;
    public float hitboxTiming = 0.1f;
    public float endAnimationTiming = 0.1f;
    bool isAttacking;
    Vector2 currDirection;
    #endregion

    #region Attack_functions
    private void Attack()
    {
        // TODO 1.3: Set the attackTimer to attackSpeed to reset the attack cooldown 
        attackTimer = attackSpeed;
        Debug.Log("Attacking now");
        Debug.Log("Current Direction: " + currDirection);
        FindObjectOfType<AudioManager>().Play("PlayerAttack");
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        /* Note: You will need to edit this function in the animation section, the enemy section, and in the sound section.*/
        isAttacking = true;
        PlayerRB.linearVelocity = Vector2.zero;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(hitboxTiming);
        Debug.Log("Casting hitbox now");
        RaycastHit2D[] hits = Physics2D.BoxCastAll(PlayerRB.position + currDirection, Vector2.one, 0f, Vector2.zero);

        foreach (RaycastHit2D hit in hits)
        {
            if(hit.transform.CompareTag("Enemy"))
            {
                Debug.Log("Tons of Damage");
                /* TODO 3.2: Call TakeDamage() inside of the enemy's Enemy script using
                the "hit" reference variable */
                Enemy enemyScript = hit.transform.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.TakeDamage(damage);
                }
            }
        }

        yield return new WaitForSeconds(hitboxTiming);
        isAttacking = false;
    }
    
    #endregion

    #region Movement_functions
    private void Move()
    {
        /*TODO 1.1: Edit the Move() function which will set PlayerRB.velocity to a vector based on which input the player is pressing.*/

        /* TODO 1.4: Set currDirection to the correct Vector direction i.e. Vector2.left.
         * HINT: there are four cardinal directions. */
        if (x_input > 0)
        {
            PlayerRB.linearVelocity = Vector2.right * moveSpeed;
            currDirection = Vector2.right;
        }
        else if (x_input < 0)
        {
            PlayerRB.linearVelocity = Vector2.left * moveSpeed;
            currDirection = Vector2.left;
        }
        else if (y_input > 0)
        {
            PlayerRB.linearVelocity = Vector2.up * moveSpeed;
            currDirection = Vector2.up;
        }
        else if (y_input < 0)
        {
            PlayerRB.linearVelocity = Vector2.down * moveSpeed;
            currDirection = Vector2.down;
        }
        else
        {
            PlayerRB.linearVelocity = Vector2.zero;
        }

        /* DO NOT MODIFY ANYTHING BELOW THIS LINE UNLESS YOU REALLY KNOW WHAT YOU'RE DOING */

        if (x_input == 0 && y_input == 0)
        {
            anim.SetBool("Moving", false);
        }
        else
        {
            anim.SetBool("Moving", true);

        }

        anim.SetFloat("DirX", currDirection.x);
        anim.SetFloat("DirY", currDirection.y);

    }
    #endregion

    #region Health_functions

    public void TakeDamage(float value)
    {
        /* TODO 3.1: Adjust currHealth when the player takes damage
        IMPORTANT: What happens when the player's health reaches 0? */
        if (isDashing) return;
        currHealth -= value;
        Debug.Log("Player Health: " + currHealth);

        if (currHealth <= 0)
        {
            Die();
        }

        /* TODO 4.1: Update the value of HPSlider after the player's health changes. */
        if (HPSlider != null)
        {
            HPSlider.value = currHealth / maxHealth;
            FindObjectOfType<AudioManager>().Play("PlayerHurt");
        }

        if (currHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float value)
    {
        /* TODO 3.1: Adjust currHealth when the player heals
        IMPORTANT: What happens when the player's health surpasses their max health? Should currHealth be above maxHealth?*/
        currHealth = Mathf.Min(currHealth + value, maxHealth);
        Debug.Log("Player Healed! Current Health: " + currHealth);

        /* TODO 4.1: Update the value of HPSlider after the player's health changes. */
        if (HPSlider != null)
        {
            HPSlider.value = currHealth / maxHealth;
        }
    }

    public void Die()
    {
        FindObjectOfType<AudioManager>().Play("PlayerDeath");
        GameObject gm = GameObject.FindWithTag("GameController");
        if (gm != null)
        {
            gm.GetComponent<GameManager>().LoseGame();
        }
        Destroy(this.gameObject);
    }
    #endregion

    #region Interact_functions
    private void Interact()
    {
        /* TODO 6.3: Use a BoxCastAll raycast to check what is infront of the player. 
         * If there is a chest game object, open the chest by calling it's Open() function */
        RaycastHit2D[] hits = Physics2D.BoxCastAll(PlayerRB.position + currDirection, new Vector2(0.5f, 0.5f), 0f, Vector2.zero);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.CompareTag("Chest"))
            {
                Chest chestScript = hit.transform.GetComponent<Chest>();
                if (chestScript != null)
                {
                    chestScript.Open();
                }
            }
        }

    }
    private IEnumerator DashRoutine()
    {
        Debug.Log("<color=cyan>[Dash] 冲刺触发！</color>");
        canDash = false;
        isDashing = true;

        Vector2 dashDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        
        if (dashDir == Vector2.zero)
        {
            dashDir = currDirection != Vector2.zero ? currDirection : Vector2.down;
        }
        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            PlayerRB.MovePosition(PlayerRB.position + dashDir * dashSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    #endregion
}
