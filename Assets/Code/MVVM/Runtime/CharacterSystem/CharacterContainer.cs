using System;
using Frictionless;
using MVVM.Runtime.CharacterSystem.Services;
using UnityEngine;

namespace MVVM.Runtime.CharacterSystem
{
    public class CharacterContainer : MonoBehaviour
    {
        private void Awake()
        {
            ServiceFactory.Instance.RegisterSingleton(new CharacterMessageRouter()); // Hacer global esta clase, en todo el juego
        }
    }
}