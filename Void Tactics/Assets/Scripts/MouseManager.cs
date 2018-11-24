using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    #region Editor customization

    [SerializeField]
    private GameObjectVariable selectedShip;

    [SerializeField]
    private GameObjectVariable hoveredObject;

    [SerializeField]
    private List<string> allowedTags;

    #endregion Editor customization

    #region Private methods

    private void SelectShip(GameObject ship)
    {
        selectedShip.Value = ship;
        MessageBus.SelectShip.Broadcast(ship);
    }

    #endregion Private methods

    #region Unity callbacks

    private void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        hoveredObject.Value = null;

        if (Physics.Raycast(ray, out hit))
        {
            var go = hit.transform.gameObject;

            if (allowedTags.Contains(go.tag))
            {
                hoveredObject.Value = go;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (hoveredObject.Value != null)
            {
                var ship = hoveredObject.Value.GetComponent<Ship_Data>().Data;
                if (ship != null && ship.Owner == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    SelectShip(hoveredObject.Value);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            MessageBus.RadialMenu.Broadcast(hoveredObject.Value);
        }
    }

    #endregion Unity callbacks
}