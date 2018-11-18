using System;
using System.Collections.Generic;
using System.Reflection;

#if NETFX_CORE
using System.Linq;
#endif

using Vdev.Utils;

namespace Vdev.Messaging {


/// <summary>
/// 
/// </summary>
public static class MessagesUtils
{
	/// <summary>
	/// Provide information of the method auto subscription. See <see cref="AutoSubscribe(object, MessagesHub)"/> .
	/// </summary>
	public sealed class MethodSubscription
	{
		/// <summary>
		/// Name of the method.
		/// </summary>
		public string MethodName {get; private set; }

		/// <summary>
		/// Associated subscription
		/// </summary>
		public ISubscription Subscription { get; private set; }

		public MethodSubscription(string methodName, ISubscription subscription)
		{
			MethodName = methodName;

			Subscription = subscription;
		}
	}


	/// <summary>
	/// 
	/// </summary>
	public sealed class MessageHandlerSignatureException : Exception
	{
		public MessageHandlerSignatureException(string message)
			: base(message)
		{}
	}


	/// <summary>
	/// Return <paramref name="hub"/> hub or MessagesHub.RuntimeInstance if null is specified.
	/// </summary>
	/// <param name="hub"></param>
	/// <returns></returns>
	public static MessagesHub HubOrRuntimeDefault(MessagesHub hub)
	{
		return hub ?? MessagesHub.RuntimeInstance;
	}


	static IMessageDescriptor MessageDescription(Type messageType)
	{
		var getInstanceMethod = messageType.GetMethod("Instance", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

		if (getInstanceMethod == null)
		{
			throw new Exception(string.Format("There is no static method 'Instance' within type [{0}]", messageType));
		}

		var instance = getInstanceMethod.Invoke(null, null);

		var description = instance as IMessageDescriptor;

		if (description == null)
		{
			throw new Exception("StatiInstance return object that is not IMessageDescription");
		}

		return description;
	}


	static IMessageDescriptor CheckHandlerAndGetDescriptor(object obj, Type messageType, MethodInfo method, MessageHandlerAttribute attrib)
	{
		var descriptor = MessageDescription(messageType);

		var methodParameters = method.GetParameters();

		if (descriptor.ParameterTypes.Length != methodParameters.Length)
		{
			if ((methodParameters.Length > descriptor.ParameterTypes.Length) || (!attrib.AllowPartialParameters))
			{
				throw new MessageHandlerSignatureException(string.Format("Expect {0} parameters on method [{1}] for message [{2}], on object [{3}]", descriptor.ParameterTypes.Length, method.Name, messageType.Name, obj.ToString()));
			}
		}
		
		//if attrib.AllowPartialParameters is set, then methodParameters.Length can be smaller than descriptor.ParameterTypes.Length
		for (var i = 0; i < methodParameters.Length; ++i)
		{
			var methodParam = methodParameters[i];
			var messageParam = descriptor.ParameterTypes[i];

			if (!ReflectionUtils.IsAssignableFrom(methodParam.ParameterType, messageParam))
			{
				throw new MessageHandlerSignatureException(string.Format("Inconsistent methods's [{0}] parameter [{1}:{2}], is not assignable from [{3}]", method.Name, methodParam.Name, methodParam.ParameterType, messageParam));
			}
		}

		return descriptor;
	}

	/// <summary>
	/// Iterate over all methods of the obj and wrap message-handlers methods as subscribers.
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="hostHub"></param>
	/// <returns>Collections of the subscriptions</returns>
	/// <exception cref="MessageHandlerSignatureException">If method's signature does not correspond to the set of the message broadcasting parameters.</exception>
	public static IList<MethodSubscription> AutoSubscribe(object obj, MessagesHub hostHub = null)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("obj");
		}

		var hub = HubOrRuntimeDefault(hostHub);

		Type t = obj.GetType();


#if !NETFX_CORE
		var inspectedMethods = ReflectionUtils.GetAllMethods(t, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#else
		var inspectedMethods =
			from m in t.GetRuntimeMethods()
			where !m.IsStatic
			select m
			;
#endif

		var subscribedMethods = new List<MethodSubscription>();

		foreach (MethodInfo mi in inspectedMethods )
		{
			var method = mi;

			var attribs = ReflectionUtils.GetCustomAttributes<MessageHandlerAttribute>(mi);

			foreach (var attrib in attribs)
			{
				var descriptor = CheckHandlerAndGetDescriptor(obj, attrib.MessageType, mi, attrib);

				var act = new Action<object[]>(parameters =>
				{
					var expectedParametersCount = method.GetParameters().Length;

					if (expectedParametersCount == parameters.Length)
					{
						method.Invoke(obj, parameters);
					}
					else
					{
						var rangeOfParameters = new object[expectedParametersCount];

						for (var i = 0; i < expectedParametersCount; ++i)
						{
							rangeOfParameters[i] = parameters[i];
						}

						method.Invoke(obj, rangeOfParameters);
					}
				});

				var subscription = hub.AddSubscription(descriptor, act, obj);

				subscribedMethods.Add(new MethodSubscription(method.Name, subscription));
			}
		}

		return subscribedMethods;
	}
}
}
