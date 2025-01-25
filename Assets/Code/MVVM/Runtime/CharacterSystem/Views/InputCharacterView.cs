using System;
using Frictionless;
using MVVM.Runtime.CharacterSystem.Messages;
using MVVM.Runtime.CharacterSystem.Services;
using UnityEngine;

namespace MVVM.Runtime.CharacterSystem.Views
{
    public class InputCharacterView : BaseCharacterComponentView
    {
        private CharacterMessageRouter _characterMessageRouter;
        private InputMovementMessage _inputMovementMessage;
        
        public override void Initialize(string id)
        {
            Id = id;
            _characterMessageRouter = ServiceFactory.Instance.Resolve<CharacterMessageRouter>();
            _inputMovementMessage = new InputMovementMessage(id, Vector3.zero);
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                _inputMovementMessage.Movement = Vector3.forward;
            }
            else
            {
                _inputMovementMessage.Movement = Vector3.zero;
            }
            
            _characterMessageRouter.RaiseMessage(_inputMovementMessage);
        }
    }
}