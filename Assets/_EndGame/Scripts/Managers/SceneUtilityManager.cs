using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneUtilityManager : MonoBehaviour
{

    private static SceneUtilityManager _instance;
    public static SceneUtilityManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<SceneUtilityManager> ();
            return _instance;
        }
    }

    public string CurrentScene
    {
        get
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name;
        }
    }

    // Loading UI
    [SerializeField] GameObject loadingScreen;
    ClientLoading ClientLoading;
    CanvasGroup canvasGroup;
    int targetSceneID = -1;

    void Awake ()
    {
        GameObject.DontDestroyOnLoad(this);

        SceneManager.activeSceneChanged += DisableLoadingUI;
        if(loadingScreen == null)
            loadingScreen = GameObject.Find ("LoadingCanvas");
        ClientLoading = loadingScreen.GetComponent<ClientLoading> ();
        canvasGroup = loadingScreen.GetComponent<CanvasGroup> ();

        StartCoroutine(LoadClientScene(1));
    }

    public void LoadSceneAsync(int sceneId, LoadSceneMode mode = LoadSceneMode.Single)
    {
        StartCoroutine(LoadClientScene(sceneId, mode));
    }

    public IEnumerator LoadClientScene (int sceneId, LoadSceneMode mode = LoadSceneMode.Single)
    {

        Debug.Log ("Fading loading Scene at timestamp: " + Time.time);
        targetSceneID = sceneId;
        loadingScreen.SetActive (true);
        yield return StartCoroutine (FadeLoadingScreen (1f, 1f));

        Debug.Log ("Started loading Scene at timestamp : " + Time.time);

        ClientLoading.enabled = true;
        AsyncOperation async = SceneManager.LoadSceneAsync (sceneId, mode);
        ClientLoading.loadingOperation = async;

        while (!async.isDone)
            yield return null;

    }

    IEnumerator FadeLoadingScreen (float targetValue, float duration)
    {
        float startValue = canvasGroup.alpha;
        float time = 0f;

        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp (startValue, targetValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = targetValue;
    }

    void DisableLoadingUI (UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.Scene newScene)
    {
        Debug.Log ($"SCENE_LOADED: {newScene.name}: {newScene.buildIndex}, target: {targetSceneID}");

        if (newScene.buildIndex != targetSceneID)
            return;

        targetSceneID = -1;

        Debug.Log ("Fading loading Scene at timestamp: " + Time.time);
        StartCoroutine (FadeLoadingScreen (0f, 1f));

        // Disable loading stuff
        ClientLoading.loadingOperation = null;
        ClientLoading.enabled = false;
        loadingScreen.SetActive (false);
        Debug.Log ("Finished loading Scene at timestamp: " + Time.time);

    }
}
