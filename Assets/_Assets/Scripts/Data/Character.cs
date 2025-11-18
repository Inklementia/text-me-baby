using UnityEngine;

namespace Neocortex.Samples
{
    [CreateAssetMenu(fileName = "Character", menuName = "Chat/Character")]
    public class Character : ScriptableObject
    {
        [SerializeField] private string characterName;
        [SerializeField] private Sprite characterImage;
        [SerializeField] private string model;
        [SerializeField, TextArea(3, 20)] private string systemPrompt;

        public string CharacterName => characterName;
        public Sprite CharacterImage => characterImage;
        public string SystemPrompt => systemPrompt;

        public string GetSelectedModel()
        {
            return !string.IsNullOrEmpty(model) ? model : string.Empty;
        }
    }
}

