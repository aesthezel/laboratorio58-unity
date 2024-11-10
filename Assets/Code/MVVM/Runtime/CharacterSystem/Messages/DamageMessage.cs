namespace MVVM.Runtime.CharacterSystem.Messages
{
    public class DamageMessage : CharacterMessage
    {
        public float Damage { get; set; }
        
        public DamageMessage(string id, float damage) : base(id)
        {
            Damage = damage;
        }
    }
}