using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;


public class SceneLoader : NetworkBehaviour
{
    public static SceneLoader Instance;

    private CustomNetworkManager manager;

    public GameObject loadingScreen;
    public Image slider;
    public TextMeshProUGUI progressText;

    


    private float sceneLoadProgress;

    public bool sceneLoadComplete = false;
    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

    }

    public async void RpcLoadScene(string sceneName)
    {
        Manager.ServerChangeScene(sceneName);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        await LoadAsync(NetworkManager.loadingSceneAsync, sceneName);

    }

    public async Task LoadAsync(AsyncOperation asyncLoad,string sceneName)
    {

        loadingScreen.SetActive(true);
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            slider.fillAmount = progress;
            progressText.text = $"{progress * 100f:0}%";
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
                sceneLoadComplete = true;
            }

            await Task.Yield();
        }
        if (asyncLoad.isDone)
        {
            //Manager.SpawnPlayer();
        }
    

        Debug.Log("Sahne yükleme iþlemi tamamlandý.");
    
    }

   


}
