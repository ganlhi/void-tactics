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
    public Transform Target;

    #endregion Settings

    #region Private members

    private Vector2 input;

    [MessageHandler(typeof(MessageBus.SelectShip), AllowPartialParameters = true)]
    private void OnSelectShip()
    {
        //TODO fly to new target
        Target = selectedShip.Value == null ? null : selectedShip.Value.transform;
        PlaceCamera();
    }

    private void PlaceCamera()
    {
        var wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel != 0)
        {
            distance = Mathf.Max(distance - zoomSpeed * wheel, 1.5f);
            MessageBus.RadialMenu.Broadcast(null);
        }

        if (Target != null)
        {
            if (Input.GetMouseButton(2))
            {
                input += new Vector2(Input.GetAxis("Mouse X") * speed, Input.GetAxis("Mouse Y") * speed);
                transform.localRotation = Quaternion.Euler(-input.y, input.x, 0);
                MessageBus.RadialMenu.Broadcast(null);
            }

            transform.localPosition = Target.position - (transform.localRotation * Vector3.forward * distance);
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
        PlaceCamera();
    }

    #endregion Unity callbacks
}