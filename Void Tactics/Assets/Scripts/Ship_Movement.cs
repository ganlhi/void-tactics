using System.Collections;
using UnityEngine;
using Vdev.Messaging;

[RequireComponent(typeof(MessageAutoSubscriber))]
public class Ship_Movement : MonoBehaviour
{
    #region Private variables

    private bool applyPlotting;
    private GameObject marker;
    private Vector3 thrustVector;

    [SerializeField]
    private GameObjectVariable selectedShip;

    [SerializeField]
    private IntVariable turnDuration;

    [SerializeField]
    private IntVariable speedFactor;

    private int currentTurnDuration
    {
        get
        {
            return turnDuration / speedFactor;
        }
    }

    #endregion Private variables

    #region Public variables

    public float Thrust;
    public Vector3 Velocity;
    public Vector3 FutureVelocity;
    public int PlottedPitch;
    public int PlottedYaw;
    public int PlottedRoll;

    #endregion Public variables

    #region Private methods

    [MessageHandler(typeof(MessageBus.NextTurn))]
    private void OnTurnStart()
    {
        Thrust = 0;
        PlottedPitch = 0;
        PlottedYaw = 0;
        PlottedRoll = 0;
        applyPlotting = true;
        marker.transform.position = transform.position + Velocity;
        marker.transform.rotation = transform.rotation;
        marker.SetActive(true);
    }

    [MessageHandler(typeof(MessageBus.RunningTurn))]
    private void OnTurnRunning(bool running)
    {
        applyPlotting = false;

        if (running)
        {
            StartCoroutine(MakeMove());
        }
        else
        {
            Velocity = FutureVelocity;
        }
    }

    [MessageHandler(typeof(MessageBus.Plot))]
    private void OnPlot(ManeuverAxis axis, int amount)
    {
        if (selectedShip.Value == gameObject)
        {
            switch (axis)
            {
                case ManeuverAxis.Pitch:
                    PlottedPitch = amount;
                    break;

                case ManeuverAxis.Yaw:
                    PlottedYaw = amount;
                    break;

                case ManeuverAxis.Roll:
                    PlottedRoll = amount;
                    break;
            }
        }
    }

    [MessageHandler(typeof(MessageBus.PlotThrust))]
    private void OnPlotThrust(float thrust)
    {
        if (selectedShip.Value == gameObject)
        {
            Thrust = thrust;
        }
    }

    private IEnumerator MakeMove()
    {
        var startPos = transform.position;
        var toPos = marker.transform.position;

        var startRot = transform.rotation;
        var toRot = marker.transform.rotation;

        var duration = currentTurnDuration;
        var elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, toPos, elapsedTime / duration);
            transform.rotation = Quaternion.Slerp(startRot, toRot, elapsedTime / duration);
            yield return null;
        }

        marker.SetActive(false);
    }

    private void CreateMarker()
    {
        marker = new GameObject(gameObject.name + " [M]");

        var model = Instantiate(transform.Find("Model"), marker.transform);
        model.transform.localScale = new Vector3(.8f, .8f, .8f);
        foreach (var mr in model.GetComponentsInChildren<MeshRenderer>())
        {
            mr.material.color = new Color(.5f, .5f, .5f, .5f);
        }
    }

    #endregion Private methods

    #region Unity callbacks

    private void Awake()
    {
        CreateMarker();
        OnTurnStart();
    }

    private void Update()
    {
        if (applyPlotting)
        {
            var midMoveRot = Quaternion.identity
                * Quaternion.AngleAxis(0.5f * PlottedYaw, transform.up)
                * Quaternion.AngleAxis(0.5f * PlottedPitch, -transform.right)
                * Quaternion.AngleAxis(0.5f * PlottedRoll, -transform.forward);

            thrustVector = (midMoveRot * transform.forward).normalized * Thrust;
            FutureVelocity = Velocity + thrustVector;

            var plannedRotation = Quaternion.identity
                * Quaternion.AngleAxis(PlottedYaw, Vector3.up)
                * Quaternion.AngleAxis(PlottedPitch, -Vector3.right)
                * Quaternion.AngleAxis(PlottedRoll, -Vector3.forward);

            marker.transform.rotation = transform.rotation * plannedRotation;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + thrustVector);

        var pos = transform.position + 0.5f * Velocity;
        var midMoveRot = Quaternion.identity
                * Quaternion.AngleAxis(0.5f * PlottedYaw, transform.up)
                * Quaternion.AngleAxis(0.5f * PlottedPitch, -transform.right)
                * Quaternion.AngleAxis(0.5f * PlottedRoll, -transform.forward);

        var fwd = (midMoveRot * transform.forward).normalized;
        var up = (midMoveRot * transform.up).normalized;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pos, pos + fwd);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(pos, pos + up);
    }

    #endregion Unity callbacks
}