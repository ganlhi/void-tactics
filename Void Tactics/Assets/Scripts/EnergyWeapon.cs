using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyWeapon : MonoBehaviour
{
    #region Public variables

    public List<GameObject> Targets = new List<GameObject>();

    #endregion Public variables

    #region Private variables

    [SerializeField]
    private int range;

    [SerializeField]
    private int trackAngle;

    [SerializeField]
    private int shotDuration;

    [SerializeField]
    private IntVariable speedFactor;

    [SerializeField]
    private Material material;

    [SerializeField]
    private float lineWidth = .05f;

    private Coroutine trackingCoroutine;
    private LineRenderer lineRenderer;
    private float lastShotRatio;

    #endregion Private variables

    #region Public methods

    public void StartTrackingTargets()
    {
        lineRenderer.enabled = false;
        trackingCoroutine = StartCoroutine(TrackTargets());
    }

    public void StopTrackingTargets()
    {
        lineRenderer.enabled = false;
        if (trackingCoroutine != null)
        {
            StopCoroutine(trackingCoroutine);
        }

        Debug.LogFormat("SHOT RATIO {0}", lastShotRatio);
    }

    #endregion Public methods

    #region Private methods

    private void GetAngleAndDistance(GameObject target, out float angle, out float distance)
    {
        var toTarget = target.transform.position - transform.position;
        angle = Mathf.Abs(Vector3.Angle(transform.forward, toTarget));
        distance = toTarget.magnitude;
    }

    private IEnumerator TrackTargets()
    {
        GameObject validTarget = null;
        float targetDistance = float.MaxValue;

        while (validTarget == null)
        {
            foreach (var target in Targets)
            {
                float angle, distance;
                GetAngleAndDistance(target, out angle, out distance);

                Debug.LogFormat("[{0}] T: {1} / A: {2} / D: {3}", gameObject.name, target.name, angle, distance);

                if (angle <= trackAngle && distance <= range && distance < targetDistance)
                {
                    targetDistance = distance;
                    validTarget = target;
                }
            }

            if (validTarget != null)
            {
                break;
            }

            yield return null;
        }

        Targets.Clear();

        var elapsedTime = 0f;
        var duration = (float)shotDuration / speedFactor;

        Debug.LogFormat("[{0}] VT: {1} / S: {2}", gameObject.name, validTarget == null ? "" : validTarget.name, duration);

        lineRenderer.enabled = true;

        while (elapsedTime <= duration)
        {
            elapsedTime += Time.deltaTime;
            lastShotRatio = elapsedTime / shotDuration;

            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, validTarget.transform.position);

            // Check angle and range again
            float angle, distance;
            GetAngleAndDistance(validTarget, out angle, out distance);

            if (angle > trackAngle || distance > range)
            {
                break;
            }

            yield return null;
        }

        lineRenderer.enabled = false;
    }

    #endregion Private methods

    #region Unity callbacks

    private void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = material;
        lineRenderer.positionCount = 2;
        lineRenderer.widthMultiplier = lineWidth;
        lineRenderer.enabled = false;
    }

    #endregion Unity callbacks
}