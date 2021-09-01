using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class ClientLoading : MonoBehaviour
{
    [SerializeField] Slider progressBar;
    [SerializeField] TMP_Text progressText;
    [SerializeField] string progressTextPrefix = "Loading... ";

    public AsyncOperation loadingOperation;

    void Awake ()
    {
        DontDestroyOnLoad (this);
    }

    void Update ()
    {
        if (loadingOperation == null)
            return;

        progressBar.value = loadingOperation.progress;
        progressText.text = $"{progressTextPrefix}{Mathf.RoundToInt(loadingOperation.progress * 100f)}%";
    }

    void OnDisable ()
    {
        progressBar.value = 0f;
        progressText.text = $"{progressTextPrefix}0%";
    }

    void OnEnable ()
    {
        progressBar.value = 0f;
        progressText.text = $"{progressTextPrefix}0%";
    }
}
