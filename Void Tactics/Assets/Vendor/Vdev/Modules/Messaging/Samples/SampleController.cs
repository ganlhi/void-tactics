using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Vdev.Messaging.Samples {


/// <summary>
/// Draw and handle UI, handle screen touch.
/// Also detect which shape was touched by the user.
/// </summary>
[RequireComponent(typeof(MessageAutoSubscriber))]
public class SampleController : MonoBehaviour
{
	public enum ShapeType
	{
		Sphere,
		Box
	}


	[MessageDescription(HandlerRequirement.Default)]
	public sealed class SpawnShape : Message<SpawnShape, ShapeType>
	{}

	[MessageDescription(HandlerRequirement.Default)]
	public sealed class ShapeTouched : Message<ShapeTouched, GameObject>
	{}

	[MessageDescription(HandlerRequirement.Required)]
	public sealed class CriticalMessage : Message<CriticalMessage>
	{}


	bool m_allowToSpawnShape = true;

	string m_message = "Nothing is touched";

	ISubscription m_criticalMessageSubscription;

	IEnumerator WaitForNextSpawn()
	{
		m_allowToSpawnShape = false;

		yield return new WaitForSeconds(0.5f);

		m_allowToSpawnShape = true;
	}

	void Start()
	{
		m_criticalMessageSubscription = CriticalMessage.Subscribe(() =>
		{
			Debug.Log("Handle critical message");
		}
		, null);
	}


	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(10,10,120, 170));

		GUI.enabled = m_allowToSpawnShape;

		GUILayout.Label(m_message);

		if (GUILayout.Button("Spawn box"))
		{
			SpawnShape.Broadcast(ShapeType.Box);
		}

		if (GUILayout.Button("Spawn sphere"))
		{
			SpawnShape.Broadcast(ShapeType.Sphere);
		}

		GUI.enabled = true;

		GUILayout.Space(5);

		if (GUILayout.Button("Critical Message"))
		{
			CriticalMessage.Broadcast();
		}

		m_criticalMessageSubscription.Enabled = GUILayout.Toggle(m_criticalMessageSubscription.Enabled, "Handle critical");


		GUILayout.EndArea();
	}


	[MessageHandler(typeof(SampleController.SpawnShape), AllowPartialParameters = true)]
	void OnSpawnShape()
	{
		if (!m_allowToSpawnShape)
		{
			return;
		}

		StartCoroutine(WaitForNextSpawn());
	}


	[MessageHandler(typeof(ShapeTouched))]
	void OnShapeTouched(GameObject shapeGameObject)
	{
		m_message = string.Format("Touched:{0}", shapeGameObject.name);
	}


	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, 10.0f))
			{
				var go = hit.collider.gameObject;

				if (go.GetComponent<SampleShape>() != null)
				{
					ShapeTouched.Broadcast(go);
				}
			}
		}
	}
}


}
