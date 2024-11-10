using ContainerDI.Runtime.Presenters.Interfaces;
using UnityEngine;

namespace ContainerDI.Runtime.Presenters
{
    public class TowerPresenter : MonoBehaviour, ITowerPresenter
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float bulletSpeed = 10f;

        public void FireAt(GameObject target)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Vector3 direction = (target.transform.position - firePoint.position).normalized;
            
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.linearVelocity = direction * bulletSpeed;
            }
        }
    }
}