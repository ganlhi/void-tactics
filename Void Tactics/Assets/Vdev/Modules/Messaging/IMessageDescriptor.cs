using System;


namespace Vdev.Messaging {

/// <summary>
/// Each message type must implement IMessageDescriptor interface.
/// Used to provide basic description of the message.
/// </summary>
public interface IMessageDescriptor
{
	/// <summary>
	/// Array of the parameters of the message.
	/// </summary>
	Type[] ParameterTypes { get; }

}

}
