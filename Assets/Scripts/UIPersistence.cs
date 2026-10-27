using UnityEngine;

public class UIPersistence : MonoBehaviour
{
    private static UIPersistence instance;

    private void Awake()
    {
        // Destroy duplicate UI if Forest_01
        // or another scene creates another copy later.
        if (instance != null &&
            instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Keep Canvas + EventSystem
        // alive between scene changes.
        DontDestroyOnLoad(gameObject);
    }
}