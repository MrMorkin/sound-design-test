using System;
using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("This is a vGoToFriend Action", UnityEditor.MessageType.Info)]
#endif
    public class vGoToFriend : vStateAction
    {       
        public override string defaultName
        {
            get
            {
                return "vGoToFriend";
            }
        }
        public vAIMovementSpeed speed = vAIMovementSpeed.Running;
        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            if(fsmBehaviour.aiController.HasComponent<vAICompanion>())
            {
                fsmBehaviour.aiController.SetSpeed(speed);
                MoveToFriendPosition(fsmBehaviour.aiController.GetAIComponent<vAICompanion>());
            }
        }
        public virtual void MoveToFriendPosition(vAICompanion aICompanion)
        {
            if(aICompanion)
            {

                aICompanion.GoToFriend();
            }
        }
    }
}