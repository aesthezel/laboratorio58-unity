using Frictionless;
using MVVM.Runtime.CharacterSystem.Messages;
using MVVM.Runtime.CharacterSystem.Models;
using MVVM.Runtime.CharacterSystem.Services;
using UnityEngine;
using UnityEngine.UI;

namespace MVVM.Runtime.CharacterSystem.Views.UI
{
    public class HealthUIView : MonoBehaviour
    {
        [SerializeField] private CharacterScriptable characterScriptable;
        [SerializeField] private Slider slider;
        private CharacterMessageRouter _characterMessageRouter;
        
        private void Start()
        {
            _characterMessageRouter = ServiceFactory.Instance.Resolve<CharacterMessageRouter>();
            _characterMessageRouter.AddHandler<HealthChangeMessage>(HealthChangeReceptor);

            slider.value = characterScriptable.CurrentHealth;
            slider.maxValue = characterScriptable.MaxHealth;
            slider.minValue = characterScriptable.MinHealth;
        }

        private void OnDestroy()
        {
            _characterMessageRouter.RemoveHandler<HealthChangeMessage>(HealthChangeReceptor);
        }

        private void HealthChangeReceptor(HealthChangeMessage message)
        {
            if (message.Id != characterScriptable.Id) return;
            slider.value = message.CurrentHealth;
        }
    }
}