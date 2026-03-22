using UnityEngine;

/// <summary>
/// Detects ball collision with rim and backboard, fires GameEvents.
/// Attach to the Basketball object.
/// </summary>
public class BallCollisionFeedback : MonoBehaviour
{
    [Header("Tags")]
    [SerializeField] private string _rimTag = "Rim";
    [SerializeField] private string _backboardTag = "Backboard";
    [SerializeField] private string _groundTag = "Ground";

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 contact = collision.contacts.Length > 0 ? collision.contacts[0].point : transform.position;

        if (collision.gameObject.CompareTag(_rimTag))
        {
            GameEvents.RimHit(contact);
        }
        else if (collision.gameObject.CompareTag(_backboardTag))
        {
            GameEvents.BackboardHit(contact);
        }
        else if (collision.gameObject.CompareTag(_groundTag))
        {
            GameEvents.BallHitGround(contact);
        }
    }
}
