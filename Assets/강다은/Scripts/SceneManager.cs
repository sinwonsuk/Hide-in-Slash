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

	public void LoadSceneAsync(SceneName scene)
	{
		StartCoroutine(LoadSceneCoroutine(scene));
	}

	private IEnumerator LoadSceneCoroutine(SceneName scene)
	{
		string sceneStr = System.Enum.GetName(typeof(SceneName), scene);
		AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneStr);

		// ���⼭ �ε� �� ���൵ ǥ�� ����
		while (!asyncLoad.isDone)
		{
			Debug.Log($"Loading progress: {asyncLoad.progress}");
			yield return null;
		}
	}
}
