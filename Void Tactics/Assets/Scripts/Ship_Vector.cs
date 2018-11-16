using UnityEngine;

[RequireComponent(typeof(Ship_Movement))]
public class Ship_Vector : MonoBehaviour
{
    #region Private variables

    private Vector3 startPos;
    private Vector3 endPos;
    private Ship_Movement movement;
    private LineRenderer lineRenderer;
    private LineRenderer lineRendererFuture;

    [SerializeField]
    private Material material;

    [SerializeField]
    private Material materialFuture;

    #endregion Private variables

    #region Public methods

    public void OnTurnStart()
    {
        startPos = transform.position;
        endPos = startPos + movement.Velocity;

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        lineRendererFuture.SetPosition(0, endPos);
        lineRendererFuture.SetPosition(1, endPos + movement.FutureVelocity);
    }

    #endregion Public methods

    #region Private methods

    private LineRenderer CreateRenderer(bool isFuture)
    {
        var lineWrapper = new GameObject(isFuture ? "VectorFuture" : "Vector");
        lineWrapper.transform.parent = transform;

        var lr = lineWrapper.AddComponent<LineRenderer>();
        lr.material = isFuture ? materialFuture : material;
        lr.positionCount = 2;
        lr.widthMultiplier = .05f;

        return lr;
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
        lineRendererFuture.SetPosition(1, endPos + movement.FutureVelocity);
    }

    #endregion Unity callbacks
}