using UnityEngine;

namespace Vdev.Messaging.Samples {


[RequireComponent(typeof(MessageAutoSubscriber))]
public class SampleShape : MonoBehaviour
{

	void Update()
	{
		if (transform.position.y < -5.0f)
		{
			GameObject.Destroy(gameObject);
		}
	}


	[MessageHandler(typeof(SampleController.ShapeTouched))]
	void OnShapeTouched(GameObject shapeGameObject)
	{
		if (gameObject != shapeGameObject)
		{
			return;
		}

		gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 3.0f, ForceMode.Impulse);
	}
}

}
