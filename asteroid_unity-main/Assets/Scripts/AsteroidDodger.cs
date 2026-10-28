using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AsteroidDodger : MonoBehaviour
{
    public Camera mainCamera;

    public GameObject shipGameObject;
    public GameObject asteroidGameObject;

    public float scrollSpeed = 5f; // Speed at which world moves left

    public int initialAsteroidCount = 6;
    public float asteroidMinY = -4f;
    public float asteroidMaxY = 4f;
    public float extraSpawnDistance = 3f;

    public float minSpawnInterval = 0.8f;
    public float maxSpawnInterval = 2f;

    private float viewLeftX;
    private float viewRightX;

    private GameObject player;

    private List<GameObject> asteroids = new List<GameObject>();

    private bool gameOver = false;

    private float spawnTimer = 0f;

    // The target time until the next spawn (randomized)
    private float currentSpawnInterval = 0f;

    // Blink tracking
    private SpriteRenderer playerSpriteRenderer;
    private bool doBlink = false;
    private float blinkStartTime = 0f;
    private bool isVisible = true;
    private float nextBlinkToggle = 0f;

    public float blinkDuration = 1f;
    public float blinkInterval = 0.1f;

    public bool GetShipIsVisible()
    {
        return isVisible;
    }

    void Start()
    {
        UpdateViewportLimits();
        StartGame();
    }

    void StartGame()
    {
        SpawnPlayer();

        CreateAsteroid();
    }

    public void SetShipY(float targetY)
    {
        if (gameOver)
            return;

        player.transform.position = new Vector3(player.transform.position.x, targetY, 0);
    }

    void UpdateViewportLimits()
    {
        // Viewport coordinates: (0,0) is bottom-left, (1,1) is top-right
        Vector3 leftEdge = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0));

        viewLeftX = leftEdge.x;
        viewRightX = rightEdge.x;

        // Debug.Log($"View Left: {viewLeftX:F2}, View Right: {viewRightX:F2}");
    }

    void Update()
    {
        UpdateViewportLimits();

        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
                RestartGame();
            return;
        }

        UpdateAsteroids();
        CheckCollisions();

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= currentSpawnInterval)
        {

            spawnTimer -= currentSpawnInterval;

            CreateAsteroid();

            currentSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        }

        if (doBlink)
        {
            HandleBlinking();
        }
    }

    void HandleBlinking()
    {
        float elapsed = Time.time - blinkStartTime;


        if (Time.time >= nextBlinkToggle)
        {
           
            isVisible = !isVisible;
      
            playerSpriteRenderer.enabled = isVisible;

            nextBlinkToggle = Time.time + blinkInterval;
        }

        if (elapsed >= blinkDuration)
        {
         
            doBlink = false;
            isVisible = true;

            playerSpriteRenderer.enabled = true;

            return;
        }
    }

    void SpawnPlayer()
    {
        player = Instantiate(shipGameObject);
        
        player.transform.position = new Vector3(-3, 0, 0);
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
    }

    void CreateAsteroid()
    {
        GameObject go = Instantiate(asteroidGameObject);


        float startY = Random.Range(asteroidMinY, asteroidMaxY);

        float scale = Random.Range(0.8f, 1.5f);
        go.transform.localScale = Vector3.one * scale;
        go.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        Collider2D astCol = go.GetComponent<Collider2D>();
        float worldWidth = astCol.bounds.size.x;
        go.transform.position = new Vector3(
            viewRightX + extraSpawnDistance + worldWidth * 0.5f,
            startY,
            0
        );


        asteroids.Add(go);
    }

    void UpdateAsteroids()
    {
        for (int i = asteroids.Count - 1; i >= 0; i--)
        {
            GameObject a = asteroids[i];


            a.transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime, Space.World);

  
            a.transform.Rotate(0, 0, 30 * Time.deltaTime);

            Collider2D astCol = a.GetComponent<Collider2D>();
            float worldWidth = astCol.bounds.size.x;

            if (a.transform.position.x < viewLeftX - worldWidth * 0.5f)
            {
                Destroy(a);
                asteroids.RemoveAt(i);
            }
        }
    }

    void CheckCollisions()
    {
        if (player == null)
            return;

        Collider2D playerCol = player.GetComponent<Collider2D>();

        foreach (GameObject asteroid in asteroids)
        {
            Collider2D astCol = asteroid.GetComponent<Collider2D>();

            
            if (playerCol.IsTouching(astCol))
            {
                doBlink = true;
                blinkStartTime = Time.time;
                
                //TriggerGameOver();
                break;
            }
        }
    }

    void TriggerGameOver()
    {
        gameOver = true;
        Debug.Log("GAME OVER! Press 'R' to restart.");

        // Optional: Disable player rendering
        // if (player != null)
        // {
        //     var sr = player.GetComponent<SpriteRenderer>();
        //     if (sr)
        //         sr.enabled = false;
        // }
    }

    void RestartGame()
    {
        foreach (var a in asteroids)
            if (a)
                Destroy(a);

        asteroids.Clear();

        if (player)
            Destroy(player);

        // Reset flags
        gameOver = false;

        StartGame();
    }
}
