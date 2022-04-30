using UnityEngine;

public class UIView : MonoBehaviour
{
    public GameStates identifier;
    [SerializeField] private GameObject view;

    public void EnableView()
    {
        view.SetActive(true);
    }

    public void DisableView()
    {
        view.SetActive(false);
    }
}
