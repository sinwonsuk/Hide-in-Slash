using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
        //SoundManager.GetInstance().PlayBgm(SoundManager.bgm.Stage1);
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
