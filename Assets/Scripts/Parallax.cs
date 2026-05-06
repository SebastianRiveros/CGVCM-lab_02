using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("Referencia")]
    public Transform cameraTransform;

    [Header("Velocidad Parallax")]
    [Range(0f, 1f)]
    public float parallaxEffect = 0.3f;

    private Vector3 lastCameraPosition;

    void Start()
    {
        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        // Movimiento de la cámara
        Vector3 deltaMovement =
            cameraTransform.position - lastCameraPosition;

        // Aplicar movimiento parallax
        transform.position += new Vector3(
            deltaMovement.x *   ,
            deltaMovement.y * parallaxEffect,
            0f
        );

        // Guardar posición actual
        lastCameraPosition = cameraTransform.position;
    }
}