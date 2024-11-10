using UnityEngine;

namespace MVVM.Runtime.CharacterSystem.Views
{
    public abstract class BaseCharacterComponentView : MonoBehaviour
    {
        public string Id { get; protected set; }
        public abstract void Initialize(string id);
    }
}