using System;
using System.Collections.Generic;

#if NETFX_CORE
using System.Reflection;
#endif


using UnityEngine;


namespace Vdev.Messaging {


/// <summary>
/// Unity component that automatically (<see cref="AutoSubscription"/> is enabled) call MessagesUtils.AutoSubscribe with default hub for each component of tge GameObject
/// </summary>
[DisallowMultipleComponent]
public sealed class MessageAutoSubscriber : MonoBehaviour
{
	public sealed class SubscriptionEntry
	{
		public string ComponentName;

		public IList<MessagesUtils.MethodSubscription> MethodSubscriptions;


		public void UnsubscribeAll()
		{
			foreach (var methodSubscription in MethodSubscriptions)
			{
				methodSubscription.Subscription.Dispose();
			}
		}

	}


	List<SubscriptionEntry> m_subscriptions;

	/// <summary>
	/// If true then <see cref="Subscribe"/> should be invoked within OnEnable and <see cref="Unsubscribe"/> within OnDisable.
	/// </summary>
	public bool AutoSubscription = true;

	/// <summary>
	/// Collection of the current subscriptions.
	/// </summary>
	public ICollection<SubscriptionEntry> Subscriptions
	{
		get
		{
			return m_subscriptions;
		}
	}


	/// <summary>
	/// Subscribe all GameObject's components that annotate with MessageHandlerAttribute to the messages 
	/// </summary>
	/// <param name="messagesHub">Hub on which subscription will be occur. If null then defaul hub (MessagesUtils.Hub) will be used. </param>
	public void Subscribe(MessagesHub messagesHub = null)
	{
		Unsubscribe();

#if !NETFX_CORE
		var engineAssembly = typeof(UnityEngine.Application).Assembly;
#else
		var engineAssembly = typeof(UnityEngine.Application).GetTypeInfo().Assembly;
#endif

		m_subscriptions = new List<SubscriptionEntry>();

		foreach (Component component in GetComponents<Component>())
		{
			if (component == null)
			{
				Debug.LogWarningFormat("There is null component while attempt to auto subscribe. GameObject = [{0}]", gameObject.name);
				continue;
			}

			var componentType = component.GetType();

			if (componentType == typeof(MessageAutoSubscriber) ||
#if !NETFX_CORE
				component.GetType().Assembly == engineAssembly)
#else
				componentType.GetTypeInfo().Assembly == engineAssembly
#endif
			{
				continue;
			}

			var subscriptions = MessagesUtils.AutoSubscribe(component, messagesHub);
			if (subscriptions != null && subscriptions.Count > 0)
			{
				var entry = new SubscriptionEntry() { ComponentName = componentType.Name, MethodSubscriptions = subscriptions};

				m_subscriptions.Add(entry);
			}
		}
	}


	/// <summary>
	/// Dispose all subscriptions.
	/// </summary>
	public void Unsubscribe()
	{
		if (Subscriptions == null)
		{
			return;
		}

		foreach (var entry in Subscriptions)
		{
			entry.UnsubscribeAll();
		}

		Subscriptions.Clear();
	}


	void OnEnable()
	{
		if (AutoSubscription)
		{
			Subscribe();
		}
	}


	void OnDisable()
	{
		if (AutoSubscription)
		{
			Unsubscribe();
		}
	}

}

}
