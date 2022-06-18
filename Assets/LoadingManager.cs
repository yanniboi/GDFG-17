using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private Image ProgressBar;

    private float _progress = 0;
    
    protected AsyncOperation _asyncOperation;
    private static string _sceneToLoad = "";
    private static string _seed = "";

    public static void LoadScene(string sceneToLoad, string seed)
    {
        _sceneToLoad = sceneToLoad;
        _seed = seed;
        Application.backgroundLoadingPriority = ThreadPriority.High;
        SceneManager.LoadScene("LoadingScene");
    }
    
    private void Start()
    {
        if (!string.IsNullOrEmpty(_sceneToLoad))
        {
            StartCoroutine(nameof(BuildLevelFromSeed));
        }   
    }

    private IEnumerator BuildLevelFromSeed()
    {
        _asyncOperation = SceneManager.LoadSceneAsync(_sceneToLoad,LoadSceneMode.Single );
        _asyncOperation.allowSceneActivation = false;

        // while the scene loads, we assign its progress to a target that we'll use to fill the progress bar smoothly
        while (_asyncOperation.progress < 0.9f) 
        {
            _progress = _asyncOperation.progress;
            yield return null;
        }

        _progress = 1;
        
        _asyncOperation.allowSceneActivation = true;
    }

    // Update is called once per frame
    void Update()
    {
        ProgressBar.fillAmount = _progress;
    }

    private void Finish()
    {
        _progress = 1;
    }
    
    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }
}
