using System;
using System.Collections.Generic;

using UnityEngine;

namespace Vdev.Utils {

/// <summary>
/// Combine multiple disposable into the single one.
/// </summary>
public sealed class CompositeDisposable : IDisposable
{
	List<IDisposable> m_disposables = new List<IDisposable>();
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="disposables"></param>
	public CompositeDisposable(params IDisposable[] disposables)
	{
		if (disposables.Length > 0)
		{
			m_disposables.AddRange(disposables);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="disposables"></param>
	public CompositeDisposable(IEnumerable<IDisposable> disposables)
	{
		m_disposables.AddRange(disposables);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="disposable"></param>
	public void Add(IDisposable disposable)
	{
		m_disposables.Add(disposable);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="disposables"></param>
	public void Add(IEnumerable<IDisposable> disposables)
	{
		m_disposables.AddRange(disposables);
	}


	/// <summary>
	/// 
	/// </summary>
	public void Dispose()
	{
		foreach (var disposable in m_disposables)
		{
			try
			{
				disposable.Dispose();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		m_disposables.Clear();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="X"></typeparam>
	/// <param name="collection"></param>
	/// <returns></returns>
	public static IDisposable FromCollection<X>(IEnumerable<X> collection) where X : IDisposable
	{
		var compositeDisposable = new CompositeDisposable();

		foreach (var disposable in collection)
		{
			compositeDisposable.Add(disposable);
		}

		return compositeDisposable;
	}
}

}
