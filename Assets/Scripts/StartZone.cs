using UnityEngine;
using UnityEngine.InputSystem;

public class StartZone : MonoBehaviour
{
    [SerializeField]
    MenuManager menuManager;

    [SerializeField]
    GameObject progressBar;

    [SerializeField]
    AnimationCurve progressBarCurve;

    [SerializeField]
    LayerMask playerMask;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!LayerMaskUtils.HasLayer(playerMask, collision.gameObject.layer)) return;
        menuManager.PlayerEnterStartZone(collision.gameObject.GetComponent<PlayerInput>()); 
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!LayerMaskUtils.HasLayer(playerMask, collision.gameObject.layer)) return;
        menuManager.PlayerLeaveStartZone(collision.gameObject.GetComponent<PlayerInput>());
    }

    public void ChangeProgressBar(float value)
    {
        value = progressBarCurve.Evaluate(value);
        progressBar.transform.localScale = new Vector3(value * 7.5f, 0.4f, 1.0f);
    }
}
