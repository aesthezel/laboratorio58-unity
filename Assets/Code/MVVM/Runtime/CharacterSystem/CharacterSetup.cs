using Frictionless;
using MVVM.Runtime.CharacterSystem.Models;
using MVVM.Runtime.CharacterSystem.Services;
using MVVM.Runtime.CharacterSystem.ViewModels;
using MVVM.Runtime.CharacterSystem.Views;
using UnityEngine;

namespace MVVM.Runtime.CharacterSystem
{
    public class CharacterSetup : MonoBehaviour
    {
        [SerializeField] private CharacterScriptable characterData;
        [SerializeField] private CharacterViewModel characterViewModel;
        
        private void Start()
        {
            characterData.CurrentHealth = characterData.MaxHealth;
            
            characterViewModel.Setup(characterData);
            var views = characterViewModel.GetComponentsInChildren<BaseCharacterComponentView>();

            foreach (var view in views)
            {
                view.Initialize(characterData.Id);
            }
        }
    }
}