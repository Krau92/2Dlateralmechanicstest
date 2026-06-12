using Unity.Cinemachine;
using UnityEngine;

public class CameraChanger : MonoBehaviour
{
    public CinemachineCamera vCamera;
    public float newLens;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            vCamera.Lens.OrthographicSize = newLens;
        }
    }

}
