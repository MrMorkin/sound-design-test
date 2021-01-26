using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("This is a FindTargetDecision decision", UnityEditor.MessageType.Info)]
#endif
    public class FindTargetDecision : vStateDecision
    {
        public override string defaultName
        {
            get
            {
                return "FindTargetDecision";
            }
        }

        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            if(fsmBehaviour.aiController!=null)
            {
                fsmBehaviour.aiController.FindTarget();
                return fsmBehaviour.aiController.currentTarget.transform != null;
            }
            return true;
        }
    }
}
