namespace MVVM.Runtime.CharacterSystem.Messages
{
    public class IsAliveMessage : CharacterMessage
    {
        public IsAliveMessage(string id, bool isAlive) : base(id)
        {
            IsAlive = isAlive;
        }

        public bool IsAlive { get; set; }
    }
}