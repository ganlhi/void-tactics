using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Vdev.Messaging {

/// <summary>
/// 
/// </summary>
class MessageViewer : EditorWindow
{
	static MessageViewer s_instance;


	public sealed class MessageEntry
	{
		public Type MessageType {get; private set;}

		public List<ISubscription> Subscriptions {get; private set;}

		public bool IsFoldUnrolled {get; set;}


		public MessageEntry(Type messageType)
		{
			MessageType = messageType;

			Subscriptions = new List<ISubscription>();

			IsFoldUnrolled = false;
		}
	}


	public sealed class MessageEntriesState
	{
		List<MessageEntry> Entries { get; set;}

		public MessageEntriesState()
		{
			Entries = new List<MessageEntry>();
		}

		public ICollection<MessageEntry> CollectMessages(MessagesHub hub)
		{
			if (Entries == null)
			{
				Entries = new List<MessageEntry>();
			}

			foreach (var entry in Entries)
			{
				entry.Subscriptions.Clear();
			}

			foreach (var s in hub.Subscriptions)
			{
				var entry = Entries.Find(e => e.MessageType == s.MessageType);

				if (entry == null)
				{
					entry = new MessageEntry(s.MessageType);

					Entries.Add(entry);
				}

				entry.Subscriptions.Add(s);
			}

			return Entries;
		}
	}


	sealed class HubEntry
	{
		public WeakReference HubWeakRef {get; private set;}

		public bool IsSelected {get; set;}

		public Vector2 MessagesScroll {get; set;}

		public MessageEntriesState State {get; private set;}


		public HubEntry(MessagesHub hub)
		{
			HubWeakRef = new WeakReference(hub);

			IsSelected = false;

			MessagesScroll = Vector2.zero;

			State = new MessageEntriesState();
		}
	}


	List<HubEntry> m_hubs;

	Vector2 m_hubsScroll = Vector2.zero;

	string m_messageTypeFilter = string.Empty;

	string m_objectNameFilter = string.Empty;


	[MenuItem("Window/Vdev/Message Viewer")]
	public static void ShowWindow()
	{
		if (s_instance == null)
		{
			s_instance = ScriptableObject.CreateInstance<MessageViewer>();

			s_instance.titleContent = new GUIContent("Message viewer");
		}

		s_instance.ShowUtility();
	}


	static void MessageEntryGui(MessageEntry entry, string objectNameFilter)
	{
		entry.IsFoldUnrolled = EditorGUILayout.Foldout(entry.IsFoldUnrolled, entry.IsFoldUnrolled ? string.Empty : entry.MessageType.Name);

		if (!entry.IsFoldUnrolled)
		{
			return ;
		}

		var lowerNameFilter = objectNameFilter.ToLower();

		GUILayout.BeginVertical(GUI.skin.box);

		GUI.color = Color.green;
		GUILayout.Label(entry.MessageType.Name);
		GUI.color = Color.white;

		foreach (var subscription in entry.Subscriptions)
		{
			var owner = subscription.OwnerObject;

			MonoBehaviour monoBehaviour = (owner != null && owner is MonoBehaviour) ? (owner as MonoBehaviour) : null;

			var objectName = monoBehaviour == null ?
				(owner == null ? "[Owner not specified]" : owner.ToString()) :
				monoBehaviour.gameObject.name;

			if (!string.IsNullOrEmpty(objectNameFilter) && !objectName.ToLower().Contains(lowerNameFilter))
			{
				continue;
			}

			GUILayout.BeginHorizontal(GUI.skin.box);

			var enabled = subscription.Enabled;

			if (enabled != GUILayout.Toggle(subscription.Enabled, "On", GUI.skin.button, GUILayout.MaxWidth(50)))
			{
				subscription.Enabled = !enabled;
			}

			if (monoBehaviour != null)
			{
				GUILayout.Label(string.Format("{0}/[{1}]", monoBehaviour.gameObject.name, monoBehaviour.GetType().Name));

				if (GUILayout.Button(">", GUILayout.MaxWidth(30)))
				{
					Selection.activeGameObject = monoBehaviour.gameObject;
				}
			}
			else
			{
				GUILayout.Label(objectName);
			}

			GUILayout.EndHorizontal();
		}

		GUILayout.EndVertical();
	}


	void FiltersGui()
	{
		GUILayout.BeginVertical(GUI.skin.box);

		GUILayout.Label("Filters:");

		m_messageTypeFilter = EditorGUILayout.TextField("Message Type", m_messageTypeFilter);

		m_objectNameFilter = EditorGUILayout.TextField("Object Name", m_objectNameFilter);

		GUILayout.EndVertical();
	}


	void OnGUI()
	{
		if (m_hubs == null)
		{
			m_hubs = new List<HubEntry>();
		}

		m_hubs.RemoveAll(hubEntry => !hubEntry.HubWeakRef.IsAlive);

		foreach (var hub in MessagesHubRegistry.Hubs)
		{
			if (!m_hubs.Exists(hubEntry => ReferenceEquals(hubEntry.HubWeakRef.Target, hub)))
			{
				m_hubs.Add(new HubEntry(hub));
			}
		}

		if (m_hubs.Count > 0 && !m_hubs.Exists(he => he.IsSelected))
		{
			m_hubs[0].IsSelected = true;
		}

		GUILayout.BeginHorizontal();

#region Render registered hubs

		GUILayout.BeginVertical(GUI.skin.box, GUILayout.MaxWidth(150));

		m_hubsScroll = GUILayout.BeginScrollView(m_hubsScroll);

		GUILayout.Label("Message hubs:");
		GUILayout.Space(3);

		foreach (var hubEntry in m_hubs)
		{
			var hubRef = hubEntry.HubWeakRef.Target;

			if (hubRef == null)
			{
				continue;
			}

			var hub = (MessagesHub)hubRef;

			var hubName = string.IsNullOrEmpty(hub.Name) ? "[Unnamed]" : hub.Name;

			hubEntry.IsSelected = GUILayout.Toggle(hubEntry.IsSelected, hubName, GUI.skin.button);
		}

		GUILayout.EndScrollView();

		GUILayout.EndVertical();

#endregion

		var selectedHubEntry = m_hubs.Find(he => he.IsSelected);

		var selectedHub = selectedHubEntry != null ? selectedHubEntry.HubWeakRef.Target as MessagesHub : null;


		if (selectedHub != null)
		{//render messages state for the selected hub.
			GUILayout.BeginVertical(GUI.skin.box);

			FiltersGui();

			GUILayout.Label("Subscriptions:");

			selectedHubEntry.MessagesScroll = GUILayout.BeginScrollView(selectedHubEntry.MessagesScroll);

			foreach (var entry in selectedHubEntry.State.CollectMessages(selectedHub))
			{
				if (!string.IsNullOrEmpty(m_messageTypeFilter) && !entry.MessageType.Name.Contains(m_messageTypeFilter))
				{
					continue;
				}

				MessageEntryGui(entry, m_objectNameFilter);
			}

			GUILayout.EndScrollView();

			GUILayout.EndVertical();
		}

		GUILayout.EndHorizontal();
	}

}


}
