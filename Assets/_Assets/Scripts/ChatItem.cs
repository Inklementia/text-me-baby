using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace Neocortex.Samples
{
    public class ChatItem : MonoBehaviour
    {
        [SerializeField] private Image avatarImage;
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI lastMessageText;
        [SerializeField] private Button selectButton;

        public UnityEvent<ChatCharacter> OnChatSelected = new UnityEvent<ChatCharacter>();

        private ChatCharacter chatCharacter;

        public void Initialize(ChatCharacter character)
        {
            chatCharacter = character;
            UpdateUI();

            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(() => OnChatSelected?.Invoke(chatCharacter));
            }
        }

        public void UpdateUI()
        {
            if (chatCharacter == null) return;

            Character characterData = chatCharacter.CharacterData;
            if (characterData == null) return;

            // Update avatar
            if (avatarImage != null && characterData.CharacterImage != null)
            {
                avatarImage.sprite = characterData.CharacterImage;
            }

            // Update name
            if (characterNameText != null)
            {
                characterNameText.text = characterData.CharacterName;
            }

            // Update last message
            if (lastMessageText != null)
            {
                string lastMsg = chatCharacter.LastMessage;
                string displayText = string.IsNullOrEmpty(lastMsg) ? "No messages yet" : TruncateMessage(lastMsg, 50);
                lastMessageText.text = displayText;
            }
        }

        private string TruncateMessage(string message, int maxLength)
        {
            if (message.Length <= maxLength)
                return message;
            return message.Substring(0, maxLength) + "...";
        }

        private void OnDestroy()
        {
            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
            }
        }
    }
}

