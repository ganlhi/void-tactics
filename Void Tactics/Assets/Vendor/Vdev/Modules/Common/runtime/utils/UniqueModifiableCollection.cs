using System;
using System.Collections.Generic;


namespace Vdev.Utils {

/// <summary>
/// Represents collections of the unique elements.
/// Collection can be modified during @ref Iterate operation.
/// </summary>
public sealed class UniqueModifiableCollection<T>
{
	enum IterationState
	{
		Continue,
		Break
	}

	List<T> m_list = new List<T>();

	List<T> m_removePending;

	int m_iteratorsCounter = 0;


	void Iterate(Func<T,IterationState> iterator)
	{
		++m_iteratorsCounter;

		try
		{
			int i = 0;

			while (i < m_list.Count)
			{
				var element = m_list[i++];

				if (m_removePending != null && m_removePending.Contains(element))
				{
					continue;
				}

				if (iterator(element) == IterationState.Break)
				{
					break;
				}
			}
		}

		finally
		{
			if (--m_iteratorsCounter == 0)
			{
				ApplyPendingOperations();
			}
		}
	}

	/// <summary>
	/// Iterate over collection
	/// </summary>
	/// <param name="iterator">Callback that called for each element of the collection.</param>
	public void Iterate(Action<T> iterator)
	{
		Iterate(element =>
		{
			iterator(element);

			return IterationState.Continue;
		});
	}

	public void CopyTo<Y>(ICollection<Y> collection, Func<T,Y> accessor = null) where Y:class
	{
		if (collection.IsReadOnly)
		{
			throw new Exception("Can not copy into readonly collection");
		}

		Iterate(element =>
		{
			var value = accessor == null ? element as Y: accessor(element);

			collection.Add(value);
		});
	}


	/// <summary>
	/// Try to find element that satisfy condition specified by predicate.
	/// </summary>
	/// <param name="predicate">Condition predicate</param>
	/// <param name="element">Result element or default(T) if not found</param>
	/// <returns>true if element are found, false in other case.</returns>
	public bool TryFind(Func<T, bool> predicate, out T element)
	{
		T result = default(T);

		bool found = false;

		Iterate(e =>
		{
			found = predicate(e);

			if (found)
			{
				result = e;

				return IterationState.Break;
			}

			return IterationState.Continue;
		});

		element = result;

		return found;
	}


	public int Count
	{
		get
		{
			var removedCount = m_removePending == null ? 0 : m_removePending.Count;

			return m_list.Count - removedCount;

		}
	}

	/// <summary>
	/// Add new element. Element must not exists within collection;
	/// </summary>
	/// <param name="element"></param>
	public void AddUnique(T element)
	{
		if (m_removePending != null)
		{
			if (m_removePending.Contains(element))
			{
				m_removePending.Remove(element);
			}
		}

		var elementIndex = m_list.IndexOf(element);

		if (elementIndex >= 0)
		{
			m_list[elementIndex] = element;
		}
		else
		{
			m_list.Add(element);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="element"></param>
	/// <returns></returns>
	public bool Remove(T element)
	{
		if (m_iteratorsCounter == 0)
		{
			return m_list.Remove(element);
		}

		if (m_removePending == null)
		{
			m_removePending = new List<T>();
		}

		if (m_removePending.Contains(element))
		{
			return false;
		}

		m_removePending.Add(element);

		return true;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="index"></param>
	public void RemoveAt(int index)
	{
		if (m_iteratorsCounter == 0)
		{
			m_list.RemoveAt(index);

			return;
		}

		var element = m_list[index];

		Remove(element);
	}


	void ApplyPendingOperations()
	{
		if (m_removePending == null)
		{
			return;
		}

		foreach (var element in m_removePending)
		{
			m_list.Remove(element);
		}

		m_removePending.Clear();
	}
}

}
