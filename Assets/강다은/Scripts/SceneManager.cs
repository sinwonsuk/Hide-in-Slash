using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Photon.Pun;

public enum SceneName
{
	TitleScene,
	LoginScene,
	RobbyScene,
	WaitingRoom,
	GameScene,
}

public class SceneManager : MonoBehaviour
{
	public static SceneManager Instance { get; private set; }

	private void Awake()
	{
        PhotonNetwork.SendRate = 30; // 기본값은 10
        PhotonNetwork.SerializationRate = 30; // 기본값은 10

        if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
    private void Start()
    {
        Screen.SetResolution(1920, 1080, false);
        SoundManager.GetInstance().PlayBgm(SoundManager.bgm.PleaseFind);
    }

    public void LoadSceneAsync(SceneName scene)
	{
		StartCoroutine(LoadSceneCoroutine(scene));
	}

	private IEnumerator LoadSceneCoroutine(SceneName scene)
	{
		string sceneStr = System.Enum.GetName(typeof(SceneName), scene);
		AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneStr);

		// 여기서 로딩 중 진행도 표시 가능
		while (!asyncLoad.isDone)
		{
			Debug.Log($"Loading progress: {asyncLoad.progress}");
			yield return null;
		}
	}
}
