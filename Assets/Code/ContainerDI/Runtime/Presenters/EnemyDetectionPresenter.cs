using ContainerDI.Runtime.Commands;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VitalRouter;

namespace ContainerDI.Runtime.Presenters
{
    public class EnemyDetectionPresenter : MonoBehaviour
    {
        [SerializeField] private float detectionRadius = 10f;
        [SerializeField] private LayerMask enemyLayer;

        [Inject] 
        private ICommandPublisher _publisher;

        private const int MaxEnemies = 10;
        private readonly Collider[] _detectedEnemies = new Collider[MaxEnemies];

        // [Inject]
        // public void Construct(ICommandPublisher publisher)
        // {
        //     _publisher = publisher;
        // }

        private void Update()
        {
            DetectEnemiesInRange();
        }

        private void DetectEnemiesInRange()
        {
            int enemyCount = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, _detectedEnemies, enemyLayer);

            for (int i = 0; i < enemyCount; i++)
            {
                Collider enemyCollider = _detectedEnemies[i];

                if (enemyCollider == null) continue;
                _publisher.PublishAsync(new EnemyDetectedCommand { Enemy = enemyCollider.gameObject }).Forget();
            }
        }
    }
}