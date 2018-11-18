using System;

namespace Vdev.Utils {


/// <summary>
/// Utility class that invoke specified action on dispose.
/// </summary>
public sealed class Disposable : IDisposable
{
	Action m_disposeAction;
	bool m_disposeOnlyOnce;

	public Disposable(Action disposeAction, bool disposeOnlyOnce = true )
	{
		m_disposeAction = disposeAction;
		m_disposeOnlyOnce = disposeOnlyOnce;
	}

	public void Dispose()
	{
		if (m_disposeAction != null)
		{
			try
			{
				m_disposeAction();
			}

			finally
			{
				if (m_disposeOnlyOnce)
					m_disposeAction = null;
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="f"></param>
	/// <returns>IDisposable thar 'wraps' Action @ref f</returns>
	public static IDisposable FromAction(Action f)
	{
		return new Disposable(f, true);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="f"></param>
	/// <returns></returns>
	public static IDisposable FromActionAllowManyDispose(Action f)
	{
		return new Disposable(f,false);
	}


	static readonly  IDisposable m_emptyDisposable = new Disposable(null);

	public static IDisposable Empty
	{
		get
		{
			return m_emptyDisposable;
		}
	}
}

}
