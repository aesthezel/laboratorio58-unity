using ContainerDI.Runtime.Presenters;
using VContainer;
using VContainer.Unity;
using VitalRouter.VContainer;
using TowerController = ContainerDI.Runtime.Controllers.TowerController;

namespace ContainerDI.Runtime.Directors
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