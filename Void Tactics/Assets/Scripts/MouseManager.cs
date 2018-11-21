using Photon.Pun;
using System.Linq;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    #region Editor customization

    [SerializeField]
    private GameObjectVariable selectedShip;

    [SerializeField]
    private GameObjectVariable hoveredShip;

    #endregion Editor customization

    #region Private methods

    private void SelectShip(GameObject ship)
    {
        selectedShip.Value = ship;
        MessageBus.SelectShip.Broadcast();
    }

    #endregion Private methods

    #region Unity callbacks

    private void Start()
    {
        // At start, select first player's ship
        var playerShip = PhotonNetwork.FindGameObjectsWithComponent(typeof(Ship_Data))
            .Where(go => go.GetComponent<Ship_Data>().Data.Owner == PhotonNetwork.LocalPlayer.ActorNumber)
            .FirstOrDefault();

        SelectShip(playerShip);
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
            if (hoveredShip.Value != null)
            {
                var ship = hoveredShip.Value.GetComponent<Ship_Data>().Data;
                if (ship.Owner == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    SelectShip(hoveredShip.Value);
                }
            }
        }
    }

    #endregion Unity callbacks
}