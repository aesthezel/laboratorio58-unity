using MVVM.Runtime.CharacterSystem.Models;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace MVVM.Runtime.CharacterSystem.ViewModels
{
    public class CharacterViewModel : MonoBehaviour
    {
        protected CharacterScriptable Character;

        public virtual void Setup(CharacterScriptable character)
        {
            Character = character;
        }
    }
}