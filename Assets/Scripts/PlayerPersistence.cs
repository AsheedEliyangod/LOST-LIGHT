using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPersistence : MonoBehaviour
{
    private static PlayerPersistence instance;

    private Rigidbody2D rb;

    private void Awake()
    {
        // Prevent duplicate players when another scene
        // containing a Player is loaded.
        if (instance != null &&
            instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        rb = GetComponent<Rigidbody2D>();

        // Keep this Player alive between scenes.
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded +=
            OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -=
            OnSceneLoaded;
    }

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode)
    {
        // Look for a PlayerSpawn in the newly loaded scene.
        GameObject spawnPoint =
            GameObject.Find("PlayerSpawn");

        if (spawnPoint == null)
            return;

        // Stop old movement before repositioning.
        if (rb != null)
        {
            rb.linearVelocity =
                Vector2.zero;

            rb.angularVelocity = 0f;
        }

        transform.position =
            spawnPoint.transform.position;

        Debug.Log(
            "Player moved to PlayerSpawn in: " +
            scene.name
        );
    }
}