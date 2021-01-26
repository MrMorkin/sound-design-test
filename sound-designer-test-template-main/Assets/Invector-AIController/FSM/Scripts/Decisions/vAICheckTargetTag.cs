using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("This is a vAICheckTargetTag decision", UnityEditor.MessageType.Info)]
#endif
    public class vAICheckTargetTag : vStateDecision
    {
        public override string defaultName
        {
            get
            {
                return "vAICheckTargetTag";
            }
        }
        public vTagMask targetTags;
        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            if (fsmBehaviour.aiController.currentTarget.transform != null)
                return targetTags.Contains(fsmBehaviour.aiController.currentTarget.transform.gameObject.tag);
            return true;
        }
    }
}
