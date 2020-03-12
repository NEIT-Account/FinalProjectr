using UnityEngine;
using UnityEngine.Events;

public class PlayerController : Controller
{
    public UnityAction<RaycastHit> onRayHit;
    public void HandleInput()
    {
        if(Input.GetMouseButton(0))
        {
            ShootRay();
        }
    }

    void ShootRay()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layer = 1 << 8;
        if(Physics.Raycast(ray, out RaycastHit hit, 1000, layer))
        {
            onRayHit?.Invoke(hit);
        }
    }
}
