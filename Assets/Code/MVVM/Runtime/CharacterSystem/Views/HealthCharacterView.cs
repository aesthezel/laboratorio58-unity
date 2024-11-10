using System;
using Frictionless;
using MVVM.Runtime.CharacterSystem.Messages;
using MVVM.Runtime.CharacterSystem.Services;

namespace MVVM.Runtime.CharacterSystem.Views
{
    public class HealthCharacterView : BaseCharacterComponentView
    {
        private CharacterMessageRouter _characterMessageRouter;
        private DamageMessage _damageMessage;

        public override void Initialize(string id)
        {
            Id = id;
            _characterMessageRouter = ServiceFactory.Instance.Resolve<CharacterMessageRouter>();
            _damageMessage = new DamageMessage(id, 0f);
        }

        public void Damage(float value)
        {
            _damageMessage.Damage = value;
            _characterMessageRouter.RaiseMessage(_damageMessage);
        }
    }
}