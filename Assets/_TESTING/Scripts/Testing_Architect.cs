using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;
namespace TESTING
{
    public class Testing_Architect : MonoBehaviour
    {
        DialogueSystem ds;
        TextArchitect architect;

        public TextArchitect.BuildMethod be = TextArchitect.BuildMethod.instant;

        string[] lines = new string[3]
        {
            "Hello, World",
            "I want to say something, come over here",
            "This is a random line of dialogue"
        };
        
        void Start()
        {
            ds = DialogueSystem.instance;
            architect = new TextArchitect(ds.dialogueContainer.dialogueText);
            architect.buildMethod = TextArchitect.BuildMethod.fade;
        }

        
        void Update()
        {
            if (be != architect.buildMethod)
            {
                architect.buildMethod=be;
                architect.Stop();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                architect.Stop();
            }

            string longLine ="I want to say something, come over here. This is a random line of dialogue. Hello, World. This is really so long line, you know, men. I realle wanna go home";
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (architect.isBuilding)
                {
                    if (!architect.hurryUp)
                    {
                          architect.hurryUp = true;  
                    }
                    else
                    {
                        architect.ForseComplit();
                    }
                }
                else
                architect.Build(longLine);
            }
            else if(Input.GetKeyDown(KeyCode.A))
            {
                architect.Append(longLine);
            }
        }
    }
}

