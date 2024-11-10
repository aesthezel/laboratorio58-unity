using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Serialization;

namespace MVVM.Runtime.CharacterSystem.Models
{
    [CreateAssetMenu(fileName = "New CharacterScriptable", menuName = "Laboratorio58/MVVM (Frictionless)/Character System/Character", order = 0)]
    public class CharacterScriptable : ScriptableObject
    {
        [NonSerialized] 
        private string _id = string.Empty;
        public string Id
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                {
                    _id = $"{name}::{Guid.NewGuid().ToString()}";
                }

                return _id;
            }
        }
        
        public float CurrentHealth;
        public float MinHealth;
        public float MaxHealth;
    }
}