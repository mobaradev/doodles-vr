using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActionProperty paintAction;
    public RaycastPainter RaycastPainter;

    private void OnEnable()
    {
        paintAction.action.performed += StartPainting;
        paintAction.action.canceled += StopPainting;

    }

    private void OnDisable()
    {
        paintAction.action.performed -= StartPainting;
        paintAction.action.canceled -= StopPainting;
    }

    void StartPainting(InputAction.CallbackContext x)
    {
        this.RaycastPainter.AllowPainting = true;
    }
    void StopPainting(InputAction.CallbackContext x)
    {
        this.RaycastPainter.AllowPainting = false;
    }
}
