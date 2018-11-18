using System;
using UnityEngine;

namespace Vdev.Messaging {

/// <summary>
/// Internal Component that used to provide default MessagesHub instance for run time.
/// </summary>
sealed class MessagesHubRuntimeInstance : MonoBehaviour
{
	static MessagesHubRuntimeInstance s_instance;

	MessagesHub DefaultRuntimeHub {get; set;}

	void Awake()
	{
		if (s_instance != null)
		{
			throw new Exception("MessagesHubRuntimeInstance already created !");
		}

		DefaultRuntimeHub = new MessagesHub("Runtime Default");
	}

	void OnDestroy()
	{
		DefaultRuntimeHub = null;

		s_instance = null;

		GC.Collect();
	}

	public static MessagesHub RuntimeHub
	{
		get
		{
#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying)
			{
				throw new Exception("Default hub instance can be used only in play mode.");
			}
#endif

			if (s_instance == null)
			{
				var gameObject = new GameObject("Messages Hub Runtime Instance", typeof(MessagesHubRuntimeInstance));

				GameObject.DontDestroyOnLoad(gameObject);

				s_instance = gameObject.GetComponent<MessagesHubRuntimeInstance>();
			}

			return s_instance.DefaultRuntimeHub;
		}
	}
}


}
