using System;

namespace Vdev.Messaging {


/// <summary>
/// If method of the class should be used as message subscriber (with auto subscription) it must be annotated by this attribute.
/// </summary>
/// <example>
/// <code>
/// sealed class MyMessage : Vdev.Messaging.Message<MyMessage, int, float>{} //...
/// 
/// class MyHandlerClass
/// {
///		[MessageHandlerAttribute(typeof(MyMessage), AllowPartialParameters = true)]
///		void MyMessageHandler(int value)
///		{
///		//...
///		}
/// }
/// 
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class MessageHandlerAttribute : Attribute
{
	/// <summary>
	/// Type of message that must be handled by method;
	/// </summary>
	public Type MessageType {get; private set; }

	/// <summary>
	/// If set to true, then method can be invoked with a partial set of parameters (in order of its appearance)
	/// </summary>
	/// <example>
	/// </example>
	public bool AllowPartialParameters { get; set; }


	/// <summary>
	/// Type of message that must be handled by method;
	/// </summary>
	/// <param name="messageType">Type of the message, on which this method should be subscribed</param>
	public MessageHandlerAttribute(Type messageType)
	{
		MessageType = messageType;

		AllowPartialParameters = false;
	}
}

}
