using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;
using System.Data;
public class TestDialogueFiles : MonoBehaviour
{
    void Start()
        {
            StartConversation();
        }
        void StartConversation()
        {
            List<string> lines = FileManager.ReadTextAsset("testFile");
            DialogueSystem.instance.Say(lines);
        }
}