using System;

using Vdev.Utils;

namespace Vdev.Messaging {

/// <summary>
/// Base for any message type.
/// </summary>
/// <typeparam name="XMsg"></typeparam>
public abstract class MessageBase<XMsg> : IMessageDescriptor
	where XMsg : MessageBase<XMsg>, new()
{
	static XMsg s_instance;

	Type[] m_parameterTypes;


	protected static XMsg Instance()
	{
		if (s_instance == null)
		{
			s_instance = new XMsg();
		}

		return s_instance;
	}


	protected abstract Type[] GetParameterTypes();


	protected void CheckIncomingParameters(object[] parameters)
	{
		var parameterTypes = (this as IMessageDescriptor).ParameterTypes;

		if (parameterTypes.Length != parameters.Length)
		{
			throw new Exception(string.Format("Invalid parameters count [{0}] for message [{1}]", parameters.Length, GetType().Name));
		}

		for (var i = 0; i < parameters.Length; ++i)
		{
			var param = parameters[i];
			var expectedType = parameterTypes[i];

			if (param == null)
			{
				if (ReflectionUtils.IsValueType(expectedType))
				{
					throw new Exception(string.Format("Parameter [i:{0}] is null, but expected a value type. For message [{1}]", i, GetType().Name));
				}

				continue;
			}

			if (!ReflectionUtils.IsAssignableFrom(expectedType, param.GetType()))
			{
				throw new Exception(string.Format("Parameter can't be assigned [i:{0}], message [{1}], expected [{2}], but [{3}]", i, GetType().Name, expectedType, param.GetType()));
			}
		}
	}


	Type[] IMessageDescriptor.ParameterTypes
	{
		get
		{
			if (m_parameterTypes == null)
			{
				m_parameterTypes = GetParameterTypes();
			}

			return m_parameterTypes;
		}
	}

}
}

