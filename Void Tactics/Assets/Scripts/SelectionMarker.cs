using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionMarker : MonoBehaviour
{
    #region Editor customization

    [SerializeField]
    private Image markerImage;

    [SerializeField]
    private TMP_Text markerText;

    [SerializeField]
    private int padding;

    [SerializeField]
    private GameObjectVariable selectedShip;

    #endregion Editor customization

    #region Unity callbacks

    private void Update()
    {
        if (selectedShip.Value != null)
        {
            markerImage.enabled = true;
            markerText.enabled = true;

            markerText.text = selectedShip.Value.GetComponent<Ship_Data>().Data.Name;

            Vector3[] corners = new Vector3[8];
            var bounds = selectedShip.Value.GetComponent<Collider>().bounds;
            var cam = Camera.main;

            var cx = bounds.center.x;
            var cy = bounds.center.y;
            var cz = bounds.center.z;
            var ex = bounds.extents.x;
            var ey = bounds.extents.y;
            var ez = bounds.extents.z;

            corners[0] = cam.WorldToScreenPoint(new Vector3(cx + ex, cy + ey, cz + ez));
            corners[1] = cam.WorldToScreenPoint(new Vector3(cx + ex, cy + ey, cz - ez));
            corners[2] = cam.WorldToScreenPoint(new Vector3(cx + ex, cy - ey, cz + ez));
            corners[3] = cam.WorldToScreenPoint(new Vector3(cx + ex, cy - ey, cz - ez));
            corners[4] = cam.WorldToScreenPoint(new Vector3(cx - ex, cy + ey, cz + ez));
            corners[5] = cam.WorldToScreenPoint(new Vector3(cx - ex, cy + ey, cz - ez));
            corners[6] = cam.WorldToScreenPoint(new Vector3(cx - ex, cy - ey, cz + ez));
            corners[7] = cam.WorldToScreenPoint(new Vector3(cx - ex, cy - ey, cz - ez));

            float min_x = corners[0].x;
            float min_y = corners[0].y;
            float max_x = corners[0].x;
            float max_y = corners[0].y;

            for (int i = 1; i < 8; i++)
            {
                if (corners[i].x < min_x)
                {
                    min_x = corners[i].x;
                }
                if (corners[i].y < min_y)
                {
                    min_y = corners[i].y;
                }
                if (corners[i].x > max_x)
                {
                    max_x = corners[i].x;
                }
                if (corners[i].y > max_y)
                {
                    max_y = corners[i].y;
                }
            }

            Rect visualRect = Rect.MinMaxRect(min_x - padding, min_y - padding, max_x + padding, max_y + padding);

            RectTransform rt = GetComponent<RectTransform>();

            rt.position = new Vector2(visualRect.xMin, visualRect.yMin);
            rt.sizeDelta = new Vector2(visualRect.width, visualRect.height);
        }
        else
        {
            markerImage.enabled = false;
            markerText.enabled = false;
        }
    }

    private void OnDestroy()
    {
        // If this is destroyed it means the scene is unloaded so currently selected ship should be forgotten
        selectedShip.Value = null;
    }

    #endregion Unity callbacks
}