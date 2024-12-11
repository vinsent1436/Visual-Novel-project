using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//Управляет логикой отображения диалогов и обработкой списка реплик.
namespace DIALOGUE
{
    public class DialogueSystem : MonoBehaviour
    {
        public DialogueContainer dialogueContainer = new DialogueContainer(); //содержит ссылки на UI-объекты
        private ConversationManager conversationManager; //управляет процессом диалога 
        private TextArchitect architect; //помогает отображать текст постепенно
        public static DialogueSystem instance;//используется для реализации паттерна Singleton (единственный экземпляр).
        public bool isRunningConversation =>conversationManager.isRunning;
        private void Awake() //Если экземпляра системы диалогов еще нет, создает его и инициализирует систему.
        {
            if(instance == null)
            {
                instance = this;
                Initialize();
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }
        bool _initialized = false;
        private void Initialize() //Создает экземпляры TextArchitect и ConversationManager, если система еще не была инициализирована.
        {
            if (_initialized)
            {
                return;
            }
            architect = new TextArchitect(dialogueContainer.dialogueText);
            conversationManager = new ConversationManager(architect);
        }
        public void ShowSpeakerName(string speakerName = "") => dialogueContainer.nameContainer.Show(speakerName);
        public void HideSpeakerName()=>dialogueContainer.nameContainer.Hide();
        public void Say(string speaker, string dialogue)//принимает имя говорящего и текст диалога, создает список строк из одной реплики и вызывает вторую.
        {
            List<string> conversation = new List<string>() {$"{speaker}\"{dialogue}\""};
            Say(conversation);
        }
        public void Say(List<string> conversation) //принимает список строк (реплик) и запускает их через conversationManager.
        {
                conversationManager.StartConversation(conversation);
        }
    }
}