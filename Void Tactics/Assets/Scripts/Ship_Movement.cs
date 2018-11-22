using System.Collections;
using UnityEngine;
using Vdev.Messaging;

[RequireComponent(typeof(MessageAutoSubscriber))]
public class Ship_Movement : MonoBehaviour
{
    #region Private variables

    private GameObject marker;

    [SerializeField]
    private GameObjectVariable selectedShip;

    [SerializeField]
    private IntVariable turnDuration;

    [SerializeField]
    private IntVariable metersPerUnit;

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
    public Vector3[] Trajectory { get; private set; }
    public Quaternion[] Attitudes { get; private set; }

    #endregion Public variables

    #region Private methods

    [MessageHandler(typeof(MessageBus.NextTurn))]
    private void OnTurnStart()
    {
        Thrust = 0;
        PlottedPitch = 0;
        PlottedYaw = 0;
        PlottedRoll = 0;
        //marker.transform.position = transform.position + Velocity;
        //marker.transform.rotation = transform.rotation;
        marker.SetActive(true);
        ComputeTrajectory();
    }

    [MessageHandler(typeof(MessageBus.RunningTurn))]
    private void OnTurnRunning(bool running)
    {
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

            ComputeTrajectory();
        }
    }

    [MessageHandler(typeof(MessageBus.PlotThrust))]
    private void OnPlotThrust(float thrust)
    {
        if (selectedShip.Value == gameObject)
        {
            Thrust = thrust;

            ComputeTrajectory();
        }
    }

    private IEnumerator MakeMove()
    {
        var startPos = transform.position;
        var startRot = transform.rotation;

        var duration = currentTurnDuration;
        var elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            var progress = elapsedTime / duration;

            var i = Mathf.Min(turnDuration - 1, Mathf.Max(0, Mathf.CeilToInt(progress * turnDuration) - 1));
            var toPos = Trajectory[i];
            var toRot = Attitudes[i];
            if (i > 0)
            {
                startPos = Trajectory[i - 1];
                startRot = Attitudes[i - 1];
            }

            var progressBetweenPos = progress * turnDuration - i;

            transform.position = Vector3.Lerp(startPos, toPos, progressBetweenPos);
            transform.rotation = Quaternion.Slerp(startRot, toRot, progressBetweenPos);
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

    private void ComputeTrajectory()
    {
        var pos = transform.position;
        var rot = transform.rotation;
        var v = Velocity;
        var a_mag = Thrust * 9.81f;

        Trajectory = new Vector3[turnDuration];
        Attitudes = new Quaternion[turnDuration];

        for (var i = 0; i < turnDuration; i++)
        {
            pos += v;
            Trajectory[i] = pos;

            rot *= Quaternion.Euler(
                -1f * PlottedPitch / turnDuration,
                1f * PlottedYaw / turnDuration,
                -1f * PlottedRoll / turnDuration);

            Attitudes[i] = rot;

            var a = rot * Vector3.forward * a_mag / metersPerUnit;
            v += a;
        }

        marker.transform.position = pos;
        marker.transform.rotation = rot;
        FutureVelocity = v;
        Debug.Log("Velocity: " + v.magnitude);
        Debug.Log("Distance: " + Vector3.Distance(transform.position, pos));
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
        //if (applyPlotting)
        //{
        //    var midMoveRot = Quaternion.identity
        //        * Quaternion.AngleAxis(0.5f * PlottedYaw, transform.up)
        //        * Quaternion.AngleAxis(0.5f * PlottedPitch, -transform.right)
        //        * Quaternion.AngleAxis(0.5f * PlottedRoll, -transform.forward);

        //    thrustVector = (midMoveRot * transform.forward).normalized * Thrust;
        //    FutureVelocity = Velocity + thrustVector;

        //    var plannedRotation = Quaternion.identity
        //        * Quaternion.AngleAxis(PlottedYaw, Vector3.up)
        //        * Quaternion.AngleAxis(PlottedPitch, -Vector3.right)
        //        * Quaternion.AngleAxis(PlottedRoll, -Vector3.forward);

        //    marker.transform.rotation = transform.rotation * plannedRotation;
        //}
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.cyan;
        //Gizmos.DrawLine(transform.position, transform.position + thrustVector);

        //var pos = transform.position + 0.5f * Velocity;
        //var midMoveRot = Quaternion.identity
        //        * Quaternion.AngleAxis(0.5f * PlottedYaw, transform.up)
        //        * Quaternion.AngleAxis(0.5f * PlottedPitch, -transform.right)
        //        * Quaternion.AngleAxis(0.5f * PlottedRoll, -transform.forward);
        if (Application.isPlaying)
        {
            for (var i = 0; i < turnDuration; i++)
            {
                var fwd = (Attitudes[i] * Vector3.forward).normalized;
                var up = (Attitudes[i] * Vector3.up).normalized;
                var pos = Trajectory[i];

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(pos, pos + fwd);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(pos, pos + up);
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(marker.transform.position, marker.transform.position + FutureVelocity * turnDuration);
        }
    }

    #endregion Unity callbacks
}