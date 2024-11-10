using ContainerDI.Runtime.Commands;
using ContainerDI.Runtime.Presenters.Interfaces;
using Cysharp.Threading.Tasks;
using VitalRouter;

namespace ContainerDI.Runtime.Controllers
{
    [Routes]
    public partial class TowerController
    {
        private readonly ITowerPresenter _presenter;
        private readonly Router _router;

        private float _lastTime;

        public TowerController(ITowerPresenter presenter, Router router)
        {
            _presenter = presenter;
            _router = router;
        }
        
        public void On(EnemyDetectedCommand command)
        {
            _router.PublishAsync(new FireAtEnemyCommand { Enemy = command.Enemy }).Forget();
        }
        
        public void On(FireAtEnemyCommand command)
        {
            _presenter.FireAt(command.Enemy);
        }
    }
}