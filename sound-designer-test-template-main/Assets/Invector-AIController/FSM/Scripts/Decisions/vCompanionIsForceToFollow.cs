using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("This is a vCompanionIsForceToFollow decision", UnityEditor.MessageType.Info)]
#endif
    public class vCompanionIsForceToFollow : vStateDecision
    {
        public override string defaultName
        {
            get
            {
                return "vCompanionIsForceToFollow";
            }
        }

        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            if (fsmBehaviour.aiController.HasComponent<vAICompanion>())
            {
               return fsmBehaviour.aiController.GetAIComponent<vAICompanion>().forceFollow;
            }
            return true;
        }
    }
}
