using System;
using System.Collections.Generic;

using Vdev.Utils;

namespace Vdev.Messaging {

/// <summary>
/// Represents single subscription
/// </summary>
public interface ISubscription : IDisposable
{
	/// <summary>
	/// Handler that specified for MessagesHub.AddSubscription
	/// </summary>
	Action<object[]> Handler {get;}

	/// <summary>
	/// Set/Get state of the subscription. If is false then this handler will be ignored (like there is no subscription) on MessagesHub.Broadcast
	/// </summary>
	bool Enabled {get;set;}

	/// <summary>
	/// Type of the message.
	/// </summary>
	Type MessageType {get;}

	/// <summary>
	/// Owner that specified for MessagesHub.AddSubscription
	/// </summary>
	object OwnerObject {get;}
}



/// <summary>
/// Messages Hub. Provide interaction between subscribers and broadcasters.
/// </summary>
public sealed class MessagesHub
{
	static MessagesHub s_defaultInstance;

	/// <summary>
	/// Represents active subscription.
	/// </summary>

	class Subscription : ISubscription
	{
		MessageDelegate m_messageDelegate;

		bool m_enabled = true;

		Action<object[]> m_handler;

		WeakReference m_ownerReference;

		public Action<object[]> Handler
		{
			get
			{
				return m_handler;
			}
		}

		public bool Enabled
		{
			get
			{
				return m_messageDelegate != null && m_enabled;
			}

			set
			{
				m_enabled = value;
			}
		}

		public Type MessageType
		{
			get
			{
				return m_messageDelegate == null ? null : m_messageDelegate.MessageType;
			}
		}

		public object OwnerObject
		{
			get
			{
				return m_ownerReference == null ? null : m_ownerReference.Target;
			}
		}

		public Subscription(MessageDelegate messageDelegate, Action<object[]> handler, object owner)
		{
			m_messageDelegate = messageDelegate;

			m_handler = handler;

			if (owner != null)
			{
				m_ownerReference = new WeakReference(owner);
			}
		}

		public void Dispose()
		{
			if (m_messageDelegate != null)
			{
				m_messageDelegate.RemoveSubscription(this);

				m_messageDelegate = null;
			}
		}
	}


	/// <summary>
	/// Default hub's instance that can be used only in runtime.
	/// </summary>
	/// <exception cref="Exception">When invoked during development when not isPlaying.</exception>
	public static MessagesHub RuntimeInstance
	{
		get
		{
			return MessagesHubRuntimeInstance.RuntimeHub;
		}
	}


	sealed class MessageDelegate
	{
		public Type MessageType { get; private set; }


		public UniqueModifiableCollection<Subscription> Subscriptions {get; private set;}

		public MessageDelegate(Type messageType)
		{
			Subscriptions = new UniqueModifiableCollection<Subscription>();

			MessageType = messageType;
		}

		public ISubscription AddSubscription(Action<object[]> handler, object owner)
		{
			var subscription = new Subscription(this, handler, owner);

			Subscriptions.AddUnique(subscription);

			return subscription;
		}

		public void RemoveSubscription(Subscription subscription)
		{
			Subscriptions.Remove(subscription);
		}

		public bool Invoke(object[] parameters)
		{
			bool handlerInvoked = false;

			Subscriptions.Iterate(subscription =>
			{
				if (subscription.Enabled && subscription.Handler != null)
				{
					handlerInvoked = true;

					try
					{
						subscription.Handler.DynamicInvoke(new object[]{parameters});
					}
					catch (System.Reflection.TargetInvocationException invocationException)
					{
						if (invocationException.InnerException != null)
						{
							throw invocationException.InnerException;
						}
						else
						{
							throw;
						}
					}
				}
			});

			return handlerInvoked;
		}
	}

	/// <summary>
	/// Name of the hub instance. Used only for development/debug purposes (This name will be displayed within MessageViewer window).
	/// </summary>
	public string Name {get; set; }

	List<MessageDelegate> m_messageDelegates = new List<MessageDelegate>();
		
	public MessagesHub(string name = null)
	{
		Name = name ?? string.Empty;

#if UNITY_EDITOR

		MessagesHubRegistry.Register(this);

#endif
	}

	/// <summary>
	/// Broadcast message: invoke corresponding handlers with passed (possible partially) parameters.
	/// </summary>
	/// <param name="instance">Message instance. Same instance must be used for subscription (through @ref AddSubscription).</param>
	/// <param name="parameters">Pass directly</param>
	public void Broadcast(IMessageDescriptor instance, params object[] parameters)
	{
		var messageType = instance.GetType();

		var messageDelegate = m_messageDelegates.Find(d => d.MessageType == messageType);

		if (
			messageDelegate != null &&
			messageDelegate.Invoke(parameters))
		{
			return ;
		}

		var descAttrib = ReflectionUtils.GetCustomAttribute<MessageDescriptionAttribute>(messageType);

		var handlerRequirement = descAttrib == null ? HandlerRequirement.Default : descAttrib.HandlerRequirement;

		switch (handlerRequirement)
		{
		case HandlerRequirement.Default:
		{//#todo add ConsoleOutput Utility
#if UNITY_EDITOR
			UnityEngine.Debug.LogWarningFormat("There is no registered handlers for [{0}]", messageType);
#endif
			break;
		}

		case HandlerRequirement.NotRequired:
		{
			break;
		}

		case HandlerRequirement.Required:
		{
			throw new Exception(string.Format("Message '{0}' must be handled by its requirement ! ", messageType));
		}
		}
	}


	/// <summary>
	/// Subscribe on the message broadcasting. 
	/// </summary>
	/// <param name="descriptor">Message instance. Same instance must be used for the broadcasting (through @ref Broadcast)</param>
	/// <param name="handler">Handler that should be invoked when <see cref="Broadcast"/> will be called.</param>
	/// <param name="owner">Used only for development/debug prurposes. String representation of this parameter (if specified) will be displayed withing MessageViewer</param>
	/// <returns></returns>
	public ISubscription AddSubscription(IMessageDescriptor descriptor, Action<object[]> handler, object owner)
	{
		var messageType = descriptor.GetType();

		MessageDelegate messageDelegate = null;

		lock (m_messageDelegates)
		{
			messageDelegate = m_messageDelegates.Find(m => m.MessageType == messageType);

			if (messageDelegate == null)
			{
				messageDelegate = new MessageDelegate(messageType);

				m_messageDelegates.Add(messageDelegate);
			}
		}

		return messageDelegate.AddSubscription(handler, owner);
	}

	/// <summary>
	/// Collection of all subscriptions of that hub instance.
	/// </summary>
	public IEnumerable<ISubscription> Subscriptions
	{
		get
		{
			List<ISubscription> subscriptions = new List<ISubscription>();

			foreach (var messagesDelegate in m_messageDelegates)
			{
				messagesDelegate.Subscriptions.CopyTo(subscriptions);

				foreach (var subscription in subscriptions)
				{
					yield return subscription;
				}

				subscriptions.Clear();
			}

		}
	}

}

}
