using UnityEngine;

public class DynamicElementHolder : MonoBehaviour
{
    public DynamicMeshScript Ramp;

    // this is trivial for now. But will be important when we have hundreds of assets to update simultaneously.

    public void updateDynamicElements()
    {
        Ramp.nextRampState();
    }
}