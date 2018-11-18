using System;

namespace Vdev.Messaging {

/// <summary>
/// Base class for parameterless messages.
/// </summary>
/// <typeparam name="XMsg"></typeparam>
public class Message<XMsg> : MessageBase<XMsg>
	where XMsg:Message<XMsg>, new()
{
	/// <summary>
	/// Broadcast parameterless message.
	/// </summary>
	/// <param name="hub"></param>
	public static void Broadcast(MessagesHub hub = null)
	{
		MessagesUtils.HubOrRuntimeDefault(hub).Broadcast(Instance());
	}


	/// <summary>
	/// Add message subscription.
	/// </summary>
	/// <param name="act">Handler callback</param>
	/// <param name="hub">MessagesHub or MessagesHub::DefaultInstance if hub is null</param>
	/// <returns></returns>
	public static ISubscription Subscribe(Action act, object owner = null, MessagesHub hub = null)
	{
		return MessagesUtils.HubOrRuntimeDefault(hub).AddSubscription(Instance(), parameters =>
		{
			Instance().CheckIncomingParameters(parameters);

			act();
		},
		owner);
	}


	override protected Type[] GetParameterTypes()
	{
		return new Type[] {};
	}
}


/// <summary>
/// Base class for messages with single parameter.
/// </summary>
/// <typeparam name="XMsg"></typeparam>
/// <typeparam name="P"></typeparam>
public class Message<XMsg,P> : MessageBase<XMsg>
	where XMsg:Message<XMsg,P>, new()
{
	/// <summary>
	/// /// Broadcast parameterless message.
	/// </summary>
	/// <param name="param"></param>
	/// <param name="hub"></param>
	public static void Broadcast(P param, MessagesHub hub = null)
	{
		MessagesUtils.HubOrRuntimeDefault(hub).Broadcast(Instance(), param);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="act"></param>
	/// <param name="hub"></param>
	/// <returns></returns>
	public static ISubscription Subscribe(Action<P> act, object owner = null, MessagesHub hub = null)
	{
		return MessagesUtils.HubOrRuntimeDefault(hub).AddSubscription(Instance(), parameters =>
		{
			Instance().CheckIncomingParameters(parameters);

			var p0 = (P)parameters[0];

			act(p0);
		},
		owner);
	}


	override protected Type[] GetParameterTypes()
	{
		return new Type[] {typeof(P)};
	}
}


/// <summary>
/// Base class for messages with two parameters.
/// </summary>
/// <typeparam name="XMsg"></typeparam>
/// <typeparam name="P1"></typeparam>
/// <typeparam name="P2"></typeparam>

public class Message<XMsg,P1,P2> : MessageBase<XMsg>
	where XMsg:Message<XMsg,P1,P2>, new()
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="param1"></param>
	/// <param name="param2"></param>
	/// <param name="hub"></param>
	public static void Broadcast(P1 param1, P2 param2, MessagesHub hub = null)
	{
		MessagesUtils.HubOrRuntimeDefault(hub).Broadcast(Instance(), param1, param2);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="act"></param>
	/// <param name="hub"></param>
	/// <returns></returns>
	public static IDisposable Subscribe(Action<P1, P2> act, object owner = null, MessagesHub hub = null)
	{
		return MessagesUtils.HubOrRuntimeDefault(hub).AddSubscription(Instance(), parameters =>
		{
			Instance().CheckIncomingParameters(parameters);

			var p0 = (P1)parameters[0];
			var p1 = (P2)parameters[1];

			act(p0, p1);
		},
		owner);
	}


	protected override Type[] GetParameterTypes()
	{
		return new Type[] { typeof(P1), typeof(P2) };
	}
}


/// <summary>
/// Base class for messages with three parameters.
/// </summary>
/// <typeparam name="XMsg"></typeparam>
/// <typeparam name="P1"></typeparam>
/// <typeparam name="P2"></typeparam>
/// <typeparam name="P3"></typeparam>
public class Message<XMsg,P1,P2,P3> : MessageBase<XMsg>
	where XMsg:Message<XMsg,P1,P2,P3>, new()
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="param1"></param>
	/// <param name="param2"></param>
	/// <param name="param3"></param>
	/// <param name="hub"></param>
	public static void Broadcast(P1 param1, P2 param2, P3 param3, MessagesHub hub = null)
	{
		MessagesUtils.HubOrRuntimeDefault(hub).Broadcast(Instance(), param1, param2, param3);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="act"></param>
	/// <param name="hub"></param>
	/// <returns></returns>
	public static IDisposable Subscribe(Action<P1, P2, P3> act, object owner = null, MessagesHub hub = null)
	{
		return MessagesUtils.HubOrRuntimeDefault(hub).AddSubscription(Instance(), parameters =>
		{
			Instance().CheckIncomingParameters(parameters);

			var p0 = (P1)parameters[0];
			var p1 = (P2)parameters[1];
			var p2 = (P3)parameters[2];

			act(p0, p1, p2);
		},
		owner);
	}


	protected override Type[] GetParameterTypes()
	{
		return new Type[] { typeof(P1), typeof(P2), typeof(P3) };
	}
}
}