namespace MVVM.Runtime.CharacterSystem.Messages
{
    public class PlayAnimationMessage<T> : CharacterMessage
    {
        public PlayAnimationMessage(string id, T animation) : base(id)
        {
            Animation = animation;
        }
        
        public T Animation { get; set; }
    }
}