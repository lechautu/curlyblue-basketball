using UnityEngine;

/// <summary>
/// Sets up physics materials at runtime for all gameplay colliders.
/// Provides tuning values in the Inspector.
/// Auto-finds rim and ball colliders by tag if not assigned.
/// </summary>
public class PhysicsSetup : MonoBehaviour
{
    [Header("Ball Physics Material")]
    [SerializeField] private float _ballBounciness = 0.6f;
    [SerializeField] private float _ballFriction = 0.4f;

    [Header("Rim Physics Material")]
    [SerializeField] private float _rimBounciness = 0.3f;
    [SerializeField] private float _rimFriction = 0.6f;

    [Header("Backboard Physics Material")]
    [SerializeField] private float _boardBounciness = 0.4f;
    [SerializeField] private float _boardFriction = 0.5f;

    [Header("Floor Physics Material")]
    [SerializeField] private float _floorBounciness = 0.3f;
    [SerializeField] private float _floorFriction = 0.6f;

    [Header("References (Must be assigned)")]
    [SerializeField] private Collider _ballCollider;
    [SerializeField] private Collider _backboardCollider;
    [SerializeField] private Collider _floorCollider;
    [SerializeField] private Collider[] _rimColliders;

    private void Awake()
    {
        // Apply materials to core objects
        ApplyMaterial(_ballCollider, _ballBounciness, _ballFriction);
        ApplyMaterial(_backboardCollider, _boardBounciness, _boardFriction);
        ApplyMaterial(_floorCollider, _floorBounciness, _floorFriction);

        // Apply to all rims
        if (_rimColliders != null)
        {
            foreach (var col in _rimColliders)
            {
                ApplyMaterial(col, _rimBounciness, _rimFriction);
            }
        }
        
        // Log errors if missing
        if (_ballCollider == null) Debug.LogError("[PhysicsSetup] Ball Collider is not assigned!");
        if (_backboardCollider == null) Debug.LogError("[PhysicsSetup] Backboard Collider is not assigned!");
    }

    private void ApplyMaterial(Collider col, float bounciness, float friction)
    {
        if (col == null) return;

        PhysicsMaterial mat = new PhysicsMaterial();
        mat.bounciness = bounciness;
        mat.dynamicFriction = friction;
        mat.staticFriction = friction;
        mat.bounceCombine = PhysicsMaterialCombine.Average;
        mat.frictionCombine = PhysicsMaterialCombine.Average;
        col.sharedMaterial = mat;
    }
}
