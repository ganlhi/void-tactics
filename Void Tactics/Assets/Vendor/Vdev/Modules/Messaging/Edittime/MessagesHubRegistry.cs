#if UNITY_EDITOR

using System;
using System.Collections.Generic;

namespace Vdev.Messaging {

public static class MessagesHubRegistry
{
	static List<WeakReference> s_hubs = new List<WeakReference>();


	public static IEnumerable<MessagesHub> Hubs
	{
		get
		{
			s_hubs.RemoveAll(weakRef => !weakRef.IsAlive);

			foreach (var hub in s_hubs)
			{
				if (hub.IsAlive)
				{
					yield return (MessagesHub)hub.Target;
				}
			}
		}
	}

	
	public static void Register(MessagesHub hub)
	{
		s_hubs.Add(new WeakReference(hub));
	}

}

}


#endif
