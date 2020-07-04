using UnityEngine;

public class EditorInput: MonoBehaviour
{
    [SerializeField] public float playerSpeed = 0.0005f;
    [SerializeField] public float NSSAllSpeedMultipyer = 0.5f;
    [SerializeField] public float NSSAllSlowConstantsMultiplyer = 0.8f;
    [SerializeField] public float extraGravity;
    [SerializeField] public float jumpForce;
    [SerializeField] public float forceToVelocity  = 0.05f;

    [SerializeField] public GameObject shipPrefab;
    [SerializeField] public GameObject cylinderPrefab;
    [SerializeField] public GameObject playerKinematicPrefab;
    [SerializeField] public GameObject playerPrefab;
    [SerializeField] public GameObject buttonPreFab;
    [SerializeField] public GameObject labelPreFab;
    [SerializeField] public GameObject inputPrefab;
}

