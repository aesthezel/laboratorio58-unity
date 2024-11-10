namespace MVVM.Runtime.CharacterSystem.Messages
{
    public class PickUpMessage : CharacterMessage
    {
        public PickUpMessage(string id, bool isPickedUp) : base(id)
        {
            IsPickedUp = isPickedUp;
        }

        public bool IsPickedUp { get; set; }
    }
}