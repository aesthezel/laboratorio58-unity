using Frictionless;
using MVVM.Runtime.CharacterSystem.Messages;
using MVVM.Runtime.CharacterSystem.Models;
using MVVM.Runtime.CharacterSystem.Services;
using UnityEngine;

namespace MVVM.Runtime.CharacterSystem.ViewModels
{
    public class PlayerViewModel : CharacterViewModel
    {
        private CharacterMessageRouter _characterMessageRouter;

        public override void Setup(CharacterScriptable character)
        {
            base.Setup(character);
            
            // ROUTER
            _characterMessageRouter = ServiceFactory.Instance.Resolve<CharacterMessageRouter>();
            _characterMessageRouter.AddHandler<IsAliveMessage>(AliveReceptor);
            _characterMessageRouter.AddHandler<DamageMessage>(DamageReceptor);
            _characterMessageRouter.AddHandler<InputMovementMessage>(MovementReceptor);
            
            _characterMessageRouter.RaiseMessage(new HealthChangeMessage(Character.Id, Character.CurrentHealth));
        }

        private void DamageReceptor(DamageMessage message)
        {
            if (message.Id != Character.Id) return;
            
            Character.CurrentHealth = Mathf.Clamp(Character.CurrentHealth - message.Damage, Character.MinHealth, Character.MaxHealth);
            _characterMessageRouter.RaiseMessage(new HealthChangeMessage(Character.Id, Character.CurrentHealth));

            _characterMessageRouter.RaiseMessage(Character.CurrentHealth <= 0
                ? new IsAliveMessage(Character.Id, false)
                : new IsAliveMessage(Character.Id, true));
        }

        private void AliveReceptor(IsAliveMessage message)
        {
            if (message.Id != Character.Id) return;
            _characterMessageRouter.RaiseMessage(new PlayAnimationMessage<string>(Character.Id, message.IsAlive ? "Player@Idle" : "Player@Die"));
        }

        private void MovementReceptor(InputMovementMessage message)
        {
            if (message.Id != Character.Id) return;
            transform.position += message.Movement * Time.deltaTime;
        }
        
        private void OnDestroy()
        {
            _characterMessageRouter.RemoveHandler<IsAliveMessage>(AliveReceptor);
        }
    }
}
