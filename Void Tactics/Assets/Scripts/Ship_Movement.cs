using System.Collections;
using UnityEngine;

public class Ship_Movement : MonoBehaviour
{
    #region Public variables

    public int Thrust { get; private set; }
    public Vector3 Velocity { get; private set; }
    public Vector3 FutureVelocity { get; private set; }
    public Quaternion PlannedRotation { get; private set; }
    public bool IsMoving { get; private set; }

    #endregion Public variables

    #region Public Methods

    public IEnumerator MakeMove()
    {
        var startPos = transform.position;
        var toPos = transform.position + Velocity;

        var startRot = transform.rotation;
        var toRot = transform.rotation * PlannedRotation;

        var duration = 10f;
        var elapsedTime = 0f;

        IsMoving = true;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, toPos, elapsedTime / duration);
            transform.rotation = Quaternion.Lerp(startRot, toRot, elapsedTime / duration);
            yield return null;
        }

        //marker.SetActive(false);
        IsMoving = false;
    }

    #endregion Public Methods

    #region Unity callbacks

    private void Start()
    {
        Velocity = transform.forward * 3;
        PlannedRotation = Quaternion.identity;
    }

    private void Update()
    {
        var midMoveRot = Quaternion.Lerp(transform.rotation, transform.rotation * PlannedRotation, .5f);
        var thrustVector = midMoveRot * transform.forward * Thrust;
        FutureVelocity = Velocity + thrustVector;
    }

    #endregion Unity callbacks
}