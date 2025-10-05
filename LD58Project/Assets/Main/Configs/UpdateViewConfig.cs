using System;
using UnityEngine;

namespace Main.Configs
{
    [CreateAssetMenu(menuName = ("ViewUpdateConfig"))]
    public class UpdateViewConfig : ScriptableObject
    {
        public UpdateViewData[] ViewData;
    }
    
    [Serializable]
    public struct UpdateViewData
    {
        public string id;
        public string Description;
        public Sprite Sprite;
    }
}
