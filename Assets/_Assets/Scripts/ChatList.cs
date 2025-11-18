using UnityEngine;
using System.Collections.Generic;

namespace Neocortex.Samples
{
    public class ChatList : MonoBehaviour
    {
        [Header("Chat Management")]
        [SerializeField] private List<ChatCharacter> chatCharacters = new List<ChatCharacter>();
        
        [Header("UI References")]
        [SerializeField] private Transform chatItemsContainer;
        [SerializeField] private GameObject chatItemPrefab;

        private List<ChatItem> chatItems = new List<ChatItem>();
        private ChatCharacter currentActiveChat;

        void Start()
        {
            InitializeChatList();
        }

        private void InitializeChatList()
        {
            // Clear existing items
            foreach (var item in chatItems)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
            chatItems.Clear();

            // Create chat items for each chat character
            foreach (var chatChar in chatCharacters)
            {
                if (chatChar == null) continue;

                // Create chat item UI
                GameObject itemObj = Instantiate(chatItemPrefab, chatItemsContainer);
                ChatItem chatItem = itemObj.GetComponent<ChatItem>();
                
                if (chatItem != null)
                {
                    chatItem.Initialize(chatChar);
                    chatItem.OnChatSelected.AddListener(SelectChat);
                    chatItems.Add(chatItem);

                    // Subscribe to message updates
                    chatChar.OnMessageUpdated.AddListener(() => UpdateChatItem(chatChar));
                }

                // Hide all chats initially
                chatChar.Hide();
            }
        }

        private void SelectChat(ChatCharacter chatCharacter)
        {
            if (chatCharacter == null) return;

            // Hide current active chat
            if (currentActiveChat != null)
            {
                currentActiveChat.Hide();
            }

            // Show selected chat
            currentActiveChat = chatCharacter;
            currentActiveChat.Show();

            Debug.Log($"Selected chat: {chatCharacter.CharacterData.CharacterName}");
        }

        private void UpdateChatItem(ChatCharacter chatCharacter)
        {
            // Find the chat item index that corresponds to this chat character
            int index = chatCharacters.IndexOf(chatCharacter);
            if (index >= 0 && index < chatItems.Count)
            {
                ChatItem item = chatItems[index];
                if (item != null)
                {
                    item.UpdateUI();
                }
            }
        }

        public void AddChat(ChatCharacter chatCharacter)
        {
            if (chatCharacter == null || chatCharacters.Contains(chatCharacter))
                return;

            chatCharacters.Add(chatCharacter);
            
            // Create chat item
            GameObject itemObj = Instantiate(chatItemPrefab, chatItemsContainer);
            ChatItem chatItem = itemObj.GetComponent<ChatItem>();
            
            if (chatItem != null)
            {
                chatItem.Initialize(chatCharacter);
                chatItem.OnChatSelected.AddListener(SelectChat);
                chatItems.Add(chatItem);
                
                chatCharacter.OnMessageUpdated.AddListener(() => UpdateChatItem(chatCharacter));
            }

            chatCharacter.Hide();
        }

        public void RemoveChat(ChatCharacter chatCharacter)
        {
            if (chatCharacter == null) return;

            int index = chatCharacters.IndexOf(chatCharacter);
            if (index >= 0)
            {
                chatCharacters.RemoveAt(index);
                
                if (index < chatItems.Count)
                {
                    Destroy(chatItems[index].gameObject);
                    chatItems.RemoveAt(index);
                }

                if (currentActiveChat == chatCharacter && chatCharacters.Count > 0)
                {
                    SelectChat(chatCharacters[0]);
                }
            }
        }

        private void OnDestroy()
        {
            foreach (var item in chatItems)
            {
                if (item != null)
                {
                    item.OnChatSelected.RemoveListener(SelectChat);
                }
            }
        }
    }
}

