using System;
using Frictionless;
using MVVM.Runtime.CharacterSystem.Messages;
using MVVM.Runtime.CharacterSystem.Services;
using UnityEngine;

namespace MVVM.Runtime.CharacterSystem.Views
{
    public class AnimationCharacterView : BaseCharacterComponentView
    {
        [SerializeField] 
        private Animator animator;
        
        private string _id = string.Empty;
        private CharacterMessageRouter _messageRouter;

        public override void Initialize(string id)
        {
            Debug.Log($"Animation View ID: {id}");
            _id = id;
            _messageRouter = ServiceFactory.Instance.Resolve<CharacterMessageRouter>();
            _messageRouter.AddHandler<PlayAnimationMessage<string>>(PlayAnimation);
        }

        private void OnDestroy()
        {
            _messageRouter.RemoveHandler<PlayAnimationMessage<string>>(PlayAnimation);
        }

        private void PlayAnimation(PlayAnimationMessage<string> message)
        {
            if (message.Id != _id) return;
            animator.Play(message.Animation);
        }
    }
}