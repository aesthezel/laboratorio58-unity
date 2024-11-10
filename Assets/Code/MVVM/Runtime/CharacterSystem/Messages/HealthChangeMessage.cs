namespace MVVM.Runtime.CharacterSystem.Messages
{
    public class HealthChangeMessage : CharacterMessage
    {
        public float CurrentHealth { get; set; }
        
        public HealthChangeMessage(string id, float health) : base(id)
        {
            CurrentHealth = health;
        }
    }
}