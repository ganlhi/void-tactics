using UnityEngine;

public class Ship_Marker : MonoBehaviour
{
    #region Editor customization

    [SerializeField]
    private IntVariable markerPosIndex;

    #endregion Editor customization

    #region Private variables

    private Ship_Movement movement;
    private GameObject marker;

    #endregion Private variables

    #region Private methods

    private void CreateMarker()
    {
        marker = new GameObject(gameObject.name + " [M]");
        marker.tag = "Marker";

        var model = Instantiate(transform.Find("Model"), marker.transform);
        model.transform.localScale = new Vector3(.8f, .8f, .8f);
        foreach (var mr in model.GetComponentsInChildren<MeshRenderer>())
        {
            mr.material.color = new Color(.5f, .5f, .5f, .5f);
        }

        var bc = marker.AddComponent<BoxCollider>();
        var parentBC = gameObject.GetComponent<BoxCollider>();
        bc.center = parentBC.center;
        bc.size = parentBC.size * .8f;
    }

    #endregion Private methods

    #region Unity callbacks

    private void Start()
    {
        movement = GetComponent<Ship_Movement>();
        CreateMarker();
    }

    private void Update()
    {
        var i = Mathf.Clamp(markerPosIndex.Value, 0, movement.Trajectory.Length - 1);
        marker.transform.position = movement.Trajectory[i];
        marker.transform.rotation = movement.Attitudes[i];
        marker.SetActive(transform.position != marker.transform.position || transform.rotation != marker.transform.rotation);
    }

    #endregion Unity callbacks
}