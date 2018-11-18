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

    public int Thrust;
    public Vector3 Velocity;
    public Vector3 FutureVelocity;
    public Quaternion PlannedRotation;

    #endregion Public variables

    #region Public Methods

    [MessageHandler(typeof(MessageBus.NextTurn))]
    public void OnTurnStart()
    {
        Thrust = 0;
        PlannedRotation = Quaternion.identity;
        applyPlotting = true;

        marker.transform.position = transform.position + Velocity;
        marker.transform.rotation = transform.rotation;
        marker.SetActive(true);
    }

    [MessageHandler(typeof(MessageBus.RunningTurn))]
    public void OnTurnRunning(bool running)
    {
        Debug.Log("SHIP: " + gameObject.name);
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

    #endregion Public Methods

    #region Private methods

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
            var midMoveRot = Quaternion.Slerp(Quaternion.identity, PlannedRotation, .5f);
            thrustVector = (midMoveRot * transform.forward).normalized * Thrust;
            FutureVelocity = Velocity + thrustVector;
            marker.transform.rotation = transform.rotation * PlannedRotation;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + thrustVector);
    }

    #endregion Unity callbacks
}