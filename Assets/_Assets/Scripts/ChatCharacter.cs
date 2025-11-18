using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Neocortex.Data;

namespace Neocortex.Samples
{
    public class ChatCharacter : MonoBehaviour
    {
        [SerializeField] private NeocortexChatPanel chatPanel;
        [SerializeField] private NeocortexTextChatInput chatInput;
        [SerializeField] private Character character;
        [SerializeField] private Canvas chatCanvas;
        
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private Image characterImage;
        [SerializeField] private Button backButton;

        public UnityEvent OnMessageUpdated = new UnityEvent();

        private OllamaRequest request;
        private string lastMessage = "";

        public Character CharacterData => character;
        public string LastMessage => lastMessage;

        void Start()
        {
            Debug.Log("=== ChatCharacter Start() called ===");
            
            if (character == null)
            {
                Debug.LogError("Character is not assigned in ChatCharacter!");
                if (chatPanel != null)
                {
                    chatPanel.AddMessage("Error: No character assigned!", false);
                }
                return;
            }

            Debug.Log($"Character: {character.CharacterName}");
            Debug.Log($"System Prompt: {character.SystemPrompt}");

            // Initialize UI
            if (characterNameText != null)
            {
                characterNameText.text = character.CharacterName;
            }

            if (characterImage != null && character.CharacterImage != null)
            {
                characterImage.sprite = character.CharacterImage;
            }

            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackButtonClicked);
            }

            string modelName = character.GetSelectedModel();
            if (string.IsNullOrEmpty(modelName))
            {
                Debug.LogError($"Model is not set in Character: {character.CharacterName}!");
                if (chatPanel != null)
                {
                    chatPanel.AddMessage("No model assigned. Please set a model in the Character settings.", false);
                }
                
                // Still allow input to show error message if user tries to send
                if (chatInput != null)
                {
                    chatInput.OnSendButtonClicked.AddListener(OnUserMessageSentNoModel);
                }
                return;
            }

            Debug.Log($"Creating OllamaRequest with model: {modelName}");
            request = new OllamaRequest();
            request.OnChatResponseReceived += OnChatResponseReceived;
            request.ModelName = modelName;
            chatInput.OnSendButtonClicked.AddListener(OnUserMessageSent);

            // Set initial prompt
            if (!string.IsNullOrEmpty(character.SystemPrompt))
            {
                Debug.Log($"Setting initial system prompt: {character.SystemPrompt}");
                request.AddSystemMessage(character.SystemPrompt);
            }
            else
            {
                Debug.LogWarning("System prompt is empty!");
            }
        }

        private void OnUserMessageSentNoModel(string message)
        {
            chatPanel.AddMessage(message, true);
            chatPanel.AddMessage("Error: No model configured. Cannot send messages.", false);
        }

        private void OnBackButtonClicked()
        {
            Hide();
        }

        private void OnChatResponseReceived(ChatResponse response)
        {
            chatPanel.AddMessage(response.message, false);
            lastMessage = response.message;
            OnMessageUpdated?.Invoke();
        }

        private void OnUserMessageSent(string message)
        {
            request.Send(message);
            chatPanel.AddMessage(message, true);
            lastMessage = message;
            OnMessageUpdated?.Invoke();
        }

        public void Show()
        {
            if (chatCanvas != null)
            {
                chatCanvas.gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            if (chatCanvas != null)
            {
                chatCanvas.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (request != null)
            {
                request.OnChatResponseReceived -= OnChatResponseReceived;
            }
            
            if (chatInput != null)
            {
                chatInput.OnSendButtonClicked.RemoveListener(OnUserMessageSent);
            }

            if (backButton != null)
            {
                backButton.onClick.RemoveListener(OnBackButtonClicked);
            }
        }
    }
}
