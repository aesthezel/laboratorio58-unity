using UnityEngine;

namespace MVVM.Runtime.CharacterSystem.Messages
{
    public class InputMovementMessage : CharacterMessage
    {
        public Vector3 Movement { get; set; }

        public InputMovementMessage(string id, Vector3 movement) : base(id)
        {
            Movement = movement;
        }
    }
}