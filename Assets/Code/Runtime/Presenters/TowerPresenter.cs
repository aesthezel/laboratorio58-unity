using Laboratorio58.Runtime.Commands.Interfaces;
using UnityEngine;

namespace Laboratorio58.Runtime.Commands
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