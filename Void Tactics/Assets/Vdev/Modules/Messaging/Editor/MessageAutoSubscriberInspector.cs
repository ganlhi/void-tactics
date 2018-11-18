using System;

using UnityEngine;
using UnityEditor;


namespace Vdev.Messaging {

/// <summary>
/// Represents custom inspector for MessageAutoSubscriber component.
/// </summary>
[CustomEditor(typeof(MessageAutoSubscriber))]
class MessageAutoSubscriberInspector : Editor
{
	static Texture2D s_inspectorIcon;

	static Texture2D InspectorIcon
	{
		get
		{
			if (s_inspectorIcon == null)
			{
				s_inspectorIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Vdev/Modules/Messaging/Editor/Content/Envelope-Icon.png");
				if (s_inspectorIcon == null)
				{
					Debug.LogWarning("Can not load icon resource");
				}

			}

			return s_inspectorIcon;
		}
	}

	public override void OnInspectorGUI()
	{
		GUIStyle style = new GUIStyle ();
		style.richText = true;

		GUILayout.Box( new GUIContent(InspectorIcon), GUILayout.MaxWidth(64), GUILayout.MaxHeight(64));
		GUILayout.Space(5);

		serializedObject.Update();

		if (EditorApplication.isPlaying)
		{
			GUILayout.Label("Subscriptions:");

			var subscriber = (MessageAutoSubscriber)target;

			foreach (var subscription in subscriber.Subscriptions)
			{
				GUILayout.BeginVertical(GUI.skin.box);
				GUILayout.Label(string.Format("<color=green><b>Component</b></color><color=lime> [{0}]</color>", subscription.ComponentName), style);

				GUILayout.BeginHorizontal();
				GUILayout.Space(3);

				GUILayout.BeginVertical();

				GUILayout.Label("Message handlers:");

				foreach (var method in subscription.MethodSubscriptions)
				{
					GUILayout.BeginHorizontal(GUI.skin.box);

					GUILayout.Label(string.Format("{0}({1})", method.MethodName, "message"));

					bool subscribed = method.Subscription.Enabled;

					if (GUILayout.Toggle(subscribed, "Enabled", GUI.skin.button) != subscribed)
					{
						method.Subscription.Enabled = !subscribed;
					}

					GUILayout.EndHorizontal();
				}

				GUILayout.EndVertical();
				GUILayout.EndHorizontal();

				GUILayout.EndVertical();
			}
		}
		else
		{
			var property = serializedObject.FindProperty("AutoSubscription");

			EditorGUILayout.PropertyField(property);
		}

		serializedObject.ApplyModifiedProperties();
	}

}

}
