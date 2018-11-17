using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    #region Public methods

    public void OnPlayerReady(bool ready)
    {
        Debug.Log("Local Player Ready: " + ready);
    }

    #endregion Public methods

    #region Unity callbacks

    private void Start()
    {
    }

    private void Update()
    {
    }

    #endregion Unity callbacks
}