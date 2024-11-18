using UnityEngine;
using System.Collections.Generic; // Importing to use Dictionary

public class ButlerX : MonoBehaviour
{
    public Rigidbody2D rb;
    public float jumpforce;
    public float detectionRadius = 5.0f; // Radius to detect nearby ingredients
    private GameObject attachedIngredient = null; // The ingredient currently attached to the player
    public LayerMask groundLayer; // Layer for ground objects

    public struct PlayerStats
    {
        public int health;
        public float speed;

        public PlayerStats(int health, float speed)
        {
            this.health = health;
            this.speed = speed;
        }
    }

    public PlayerStats playerStats = new PlayerStats(100, 5.0f);
    private Vector3 startPosition;

    public enum PlayerState
    {
        Idle,
        Running,
        Jumping
    }

    public PlayerState currentState; // Current state of the player

    // Dictionary to track collected ingredients and their quantities
    private Dictionary<string, int> collectedIngredients = new Dictionary<string, int>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position; // Store the initial position as the respawn point
        currentState = PlayerState.Idle; // Initialize state to Idle
    }

    void Update()
    {
        if (playerStats.health > 0)
        {
            HandlePlayerMovement();
        }
        else
        {
            Respawn();
        }

        GameObject nearestIngredient = GetNearestIngredient(detectionRadius);
        if (nearestIngredient != null && Input.GetKeyDown(KeyCode.K))
        {
            AttachIngredientToBack(nearestIngredient);
        }
    }

    void HandlePlayerMovement()
    {
        float horizontalInput = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalInput = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            horizontalInput = 1f;
        }

        rb.linearVelocity = new Vector2(horizontalInput * playerStats.speed, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentState = PlayerState.Jumping;
            Jump();
        }

        // Example of using a switch statement to handle player states
        switch (currentState)
        {
            case PlayerState.Idle:
                Debug.Log("Player is idle.");
                break;
            case PlayerState.Running:
                Debug.Log("Player is running.");
                break;
            case PlayerState.Jumping:
                Debug.Log("Player is jumping.");
                break;
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jumpforce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (playerStats.health > 0)
            {
                TakeDamage(20);
            }
            else
            {
                Respawn();
            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            currentState = PlayerState.Idle;
        }
    }

    void TakeDamage(int damage)
    {
        playerStats.health -= damage;

        if (playerStats.health <= 0)
        {
            Respawn();
        }
    }

    void Respawn()
    {
        playerStats.health = 100;
        transform.position = startPosition;
        currentState = PlayerState.Idle;
    }

    GameObject GetNearestIngredient(float radius)
    {
        GameObject[] ingredients = GameObject.FindGameObjectsWithTag("Ingredient");
        GameObject nearestIngredient = null;
        float closestDistance = radius;

        Vector2 playerPosition = transform.position;

        foreach (GameObject ingredient in ingredients)
        {
            float ingredientDistance = Vector3.Distance(playerPosition, ingredient.transform.position);

            if (ingredientDistance <= closestDistance)
            {
                closestDistance = ingredientDistance;
                nearestIngredient = ingredient;

                if (ingredientDistance == 0f)
                {
                    break;
                }
            }
        }

        return nearestIngredient;
    }

    void AttachIngredientToBack(GameObject ingredient)
    {
        string ingredientName = ingredient.name;

        // Update the dictionary
        if (collectedIngredients.ContainsKey(ingredientName))
        {
            collectedIngredients[ingredientName]++;
        }
        else
        {
            collectedIngredients[ingredientName] = 1;
        }

        // Log the updated inventory
        Debug.Log($"Collected {ingredientName}. Total: {collectedIngredients[ingredientName]}");

        // Attach the ingredient visually
        attachedIngredient = ingredient;
        ingredient.transform.SetParent(transform);
        ingredient.transform.localPosition = new Vector3(0, -0.5f, 0);
        ingredient.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    }

    // New function with parameters
    void ModifyPlayerStats(int healthChange, float speedChange)
    {
        playerStats.health += healthChange;
        playerStats.speed += speedChange;

        Debug.Log($"Player stats updated. Health: {playerStats.health}, Speed: {playerStats.speed}");
    }

    // New nested loops example
    void AnalyzeCollectedIngredients()
    {
        foreach (var ingredient in collectedIngredients)
        {
            for (int i = 0; i < ingredient.Value; i++)
            {
                Debug.Log($"Analyzing {ingredient.Key}, instance {i + 1}");
            }
        }
    }

    public void DisplayCollectedIngredients()
    {
        Debug.Log("Collected Ingredients:");
        foreach (var ingredient in collectedIngredients)
        {
            Debug.Log($"{ingredient.Key}: {ingredient.Value}");
        }
    }
}
