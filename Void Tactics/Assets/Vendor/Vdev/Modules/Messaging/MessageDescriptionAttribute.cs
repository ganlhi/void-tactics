using System;


namespace Vdev.Messaging {

/// <summary>
/// Specify behavior of the broadcaster that does not have handlers at broadcasting time.
/// </summary>
public enum HandlerRequirement
{
	/// <summary>
	/// If there is no handler of the message within hub, the warning will be writhed.
	/// </summary>
	Default,

	/// <summary>
	/// If there is no handler of the message within hub, exception will be thrown
	/// </summary>
	Required,

	/// <summary>
	/// If there is no handler of the message within hub, then do nothing.
	/// </summary>
	NotRequired
}


/// <summary>
/// Settings for the message class.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class MessageDescriptionAttribute : Attribute
{
	/// <summary>
	/// HandlerRequirment for the annotated message.
	/// </summary>
	public HandlerRequirement HandlerRequirement { get; private set; }


	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="handlerRequirement">Specified HandlerRequirment for the annotated message.</param>
	public MessageDescriptionAttribute(HandlerRequirement handlerRequirement)
	{
		HandlerRequirement = handlerRequirement;
	}
}

}
