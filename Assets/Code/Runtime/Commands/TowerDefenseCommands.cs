using VitalRouter;
using UnityEngine;

namespace Laboratorio58.Runtime.Commands
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