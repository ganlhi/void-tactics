using UnityEngine;
using Vdev.Messaging;

[RequireComponent(typeof(Ship_Movement))]
public class Ship_Vector : MonoBehaviour
{
    #region Private variables

    private Ship_Movement movement;
    private LineRenderer lineRenderer;
    private LineRenderer lineRendererFuture;
    private Vector3 startPos;

    [SerializeField]
    private Material material;

    [SerializeField]
    private Material materialFuture;

    [SerializeField]
    private float lineWidth = .05f;

    #endregion Private variables

    #region Private methods

    private LineRenderer CreateRenderer(bool isFuture)
    {
        var lineWrapper = new GameObject(isFuture ? "VectorFuture" : "Vector");
        lineWrapper.transform.parent = transform;

        var lr = lineWrapper.AddComponent<LineRenderer>();
        lr.material = isFuture ? materialFuture : material;
        lr.positionCount = isFuture ? 2 : movement.Trajectory.Length + 1;
        lr.widthMultiplier = lineWidth;

        return lr;
    }

    [MessageHandler(typeof(MessageBus.NextTurn))]
    public void OnTurnStart()
    {
        startPos = transform.position;
    }

    #endregion Private methods

    #region Unity callbacks

    private void Start()
    {
        movement = GetComponent<Ship_Movement>();
        lineRenderer = CreateRenderer(false);
        lineRendererFuture = CreateRenderer(true);
        OnTurnStart();
    }

    private void Update()
    {
        var nb = movement.Trajectory.Length;

        lineRenderer.SetPosition(0, startPos);
        for (var i = 0; i < nb; i++)
        {
            lineRenderer.SetPosition(i + 1, movement.Trajectory[i]);
        }

        lineRendererFuture.SetPosition(0, movement.Trajectory[nb - 1]);
        lineRendererFuture.SetPosition(1, movement.Trajectory[nb - 1] + movement.FutureVelocity * nb);
    }

    #endregion Unity callbacks
}