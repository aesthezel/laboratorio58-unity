using UnityEngine;

namespace Code.Runtime.Interactions
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float lifeTime = 5f;
        private void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;
            Debug.Log($"Bullet hit {collision.gameObject.name}");
            Destroy(gameObject);
        }
    }
}