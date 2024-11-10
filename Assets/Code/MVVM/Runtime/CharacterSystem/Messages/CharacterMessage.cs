namespace MVVM.Runtime.CharacterSystem.Messages
{
    public class CharacterMessage
    {
        public string Id { get; private set; }
        
        public CharacterMessage(string id)
        {
            Id = id;
        }
    }
}