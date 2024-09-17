using Cysharp.Threading.Tasks;
using Laboratorio58.Runtime.Commands;
using Laboratorio58.Runtime.Commands.Interfaces;
using VitalRouter;

namespace Laboratorio58.Runtime.Controllers
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