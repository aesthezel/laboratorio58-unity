using UnityEngine;
using VitalRouter;

namespace ContainerDI.Runtime.Commands
{
    public struct EnemyDetectedCommand : ICommand
    {
        public GameObject Enemy { get; set; }
    }

    public struct FireAtEnemyCommand : ICommand
    {
        public GameObject Enemy { get; set; }
    }
}