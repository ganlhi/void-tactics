using UnityEngine;
using Vdev.Messaging;

[RequireComponent(typeof(MessageAutoSubscriber))]
public class CameraOrbit : MonoBehaviour
{
    #region Settings

    [SerializeField]
    private GameObjectVariable selectedShip;

    [SerializeField]
    private float speed = 8f;

    [SerializeField]
    private float zoomSpeed = 8f;

    [SerializeField]
    private float distance = 5f;

    [SerializeField]
    private Transform target;

    #endregion Settings

    #region Private members

    private Vector2 input;

    [MessageHandler(typeof(MessageBus.SelectShip))]
    private void OnSelectShip()
    {
        //TODO fly to new target
        target = selectedShip.Value == null ? null : selectedShip.Value.transform;
        PlaceCamera();
    }

    private void PlaceCamera()
    {
        if (target != null)
        {
            input += new Vector2(Input.GetAxis("Mouse X") * speed, Input.GetAxis("Mouse Y") * speed);
            transform.localRotation = Quaternion.Euler(-input.y, input.x, 0);
            transform.localPosition = target.position - (transform.localRotation * Vector3.forward * distance);
        }
    }

    #endregion Private members

    #region Unity callbacks

    private void Start()
    {
        PlaceCamera();
    }

    private void Update()
    {
        var wheel = Input.GetAxis("Mouse ScrollWheel");
        distance = Mathf.Max(distance - zoomSpeed * wheel, 1.5f);

        if (Input.GetMouseButton(2) || wheel != 0)
        {
            PlaceCamera();
        }
    }

    #endregion Unity callbacks
}