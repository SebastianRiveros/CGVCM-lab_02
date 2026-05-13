using UnityEngine;
using System.Collections;

public class MapRotator : MonoBehaviour
{
    public Transform player;
    public float rotationDuration = 0.25f;

    private PlayerController playerController;
    private bool isRotating = false;

    private int currentRotation = 0;

    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (isRotating) return;

        // Solo rotar en el suelo
        if (!playerController.IsGrounded()) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(RotateMap(-90));
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(RotateMap(90));
        }
    }

    IEnumerator RotateMap(int angle)
    {
        isRotating = true;

        currentRotation += angle;

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, currentRotation);

        Vector3 pivot = player.position;

        float elapsed = 0f;

        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / rotationDuration;

            Quaternion newRotation = Quaternion.Lerp(startRotation, targetRotation, t);

            // Diferencia de rotación
            Quaternion deltaRotation = newRotation * Quaternion.Inverse(transform.rotation);

            // Rotar alrededor del jugador
            Vector3 direction = transform.position - pivot;
            direction = deltaRotation * direction;

            transform.position = pivot + direction;

            transform.rotation = newRotation;

            yield return null;
        }

        // Corrección exacta final
        transform.rotation = targetRotation;

        isRotating = false;
        
    }
}