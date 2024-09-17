using Laboratorio58.Runtime.Commands;
using Laboratorio58.Runtime.Controllers;
using VContainer;
using VContainer.Unity;
using VitalRouter.VContainer;

namespace Laboratorio58.Runtime.Directors
{
    public class TowerLifetimeDirector : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Rutas configuradas para mandar y recibir mensajes
            builder.RegisterVitalRouter(routing =>
            {
                routing.Map<TowerController>();
            });
            
            builder.RegisterComponentInHierarchy<TowerPresenter>().UnderTransform(transform).AsImplementedInterfaces();
            builder.RegisterComponentInHierarchy<EnemyDetectionPresenter>().UnderTransform(transform);
        }
    }
}