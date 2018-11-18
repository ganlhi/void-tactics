using Photon.Pun;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    #region Editor customization

    [SerializeField]
    private GameObjectVariable selectedShip;

    [SerializeField]
    private GameObjectVariable hoveredShip;

    #endregion Editor customization

    #region Unity callbacks

    private void Start()
    {
    }

    private void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        hoveredShip.Value = null;

        if (Physics.Raycast(ray, out hit))
        {
            var go = hit.transform.gameObject;

            if (go.tag == "Ship")
            {
                hoveredShip.Value = go;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (hoveredShip.Value == null)
            {
                selectedShip.Value = null;
            }
            else
            {
                var ship = hoveredShip.Value.GetComponent<Ship_Data>().Data;
                if (ship.Owner == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    selectedShip.Value = hoveredShip.Value;
                }
            }
        }
    }

    #endregion Unity callbacks
}