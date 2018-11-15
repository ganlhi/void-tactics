using UnityEngine;

[RequireComponent(typeof(Ship_Movement))]
public class Ship_Vector : MonoBehaviour
{
    #region Private variables

    private Vector3 startPos;
    private Ship_Movement movement;
    private LineRenderer lineRenderer;
    private GameObject lineWrapper;

    [SerializeField]
    private Material material;

    [SerializeField]
    private bool IsFuture;

    #endregion Private variables

    #region Unity callbacks

    private void Start()
    {
        movement = GetComponent<Ship_Movement>();
        startPos = transform.position;

        lineWrapper = new GameObject(IsFuture ? "VectorFuture" : "Vector");
        lineWrapper.transform.parent = transform;

        lineRenderer = lineWrapper.AddComponent<LineRenderer>();
        lineRenderer.material = material;
        lineRenderer.positionCount = 2;
        lineRenderer.widthMultiplier = .05f;
    }

    private void Update()
    {
        if (IsFuture)
        {
            lineRenderer.SetPosition(0, startPos + movement.Velocity);
            lineRenderer.SetPosition(1, startPos + movement.Velocity + movement.FutureVelocity);
        }
        else
        {
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, startPos + movement.Velocity);
        }
    }

    #endregion Unity callbacks
}