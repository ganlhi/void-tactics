
using UnityEngine;

namespace Vdev.Messaging.Samples {


[RequireComponent(typeof(MessageAutoSubscriber))]
public class ShapesController : MonoBehaviour
{
	public GameObject Scene;

	public MeshRenderer Ground;

	public GameObject SphereShapePrefab;

	public GameObject BoxShapePrefab;

	int m_sphereCounter = 0;

	int m_boxCounter = 0;

	void OnEnable()
	{
		SphereShapePrefab.SetActive(false);

		BoxShapePrefab.SetActive(false);
	}

	Vector3 SpawnPosition()
	{
		var minX = Ground.bounds.center.x - Ground.bounds.extents.x;
		var maxX = Ground.bounds.center.x + Ground.bounds.extents.x;
		var minZ = Ground.bounds.center.z - Ground.bounds.extents.z;
		var maxZ = Ground.bounds.center.z + Ground.bounds.extents.z;

		return new Vector3(
			Random.Range(minX + 0.1f, maxX - 0.1f),
			2.5f,
			Random.Range(minZ + 0.1f, maxZ - 0.1f));
	}

	Quaternion SpawnRotation()
	{
		return Quaternion.Euler(
			Random.Range(-45.0f, 45.0f),
			Random.Range(-45.0f, 45.0f),
			Random.Range(-45.0f, 45.0f));
	}

	[MessageHandler(typeof(SampleController.SpawnShape))]
	void OnSpawnShape(SampleController.ShapeType what)
	{
		var prefab = what == SampleController.ShapeType.Box ? BoxShapePrefab : SphereShapePrefab;

		var instance = GameObject.Instantiate(prefab, SpawnPosition(), SpawnRotation(), Scene.transform);

		instance.name = what == SampleController.ShapeType.Box ?
		string.Format("Box_{0}", ++m_boxCounter) : 
		string.Format("Sphere_{0}", ++m_sphereCounter);

		instance.SetActive(true);

	}
}

}