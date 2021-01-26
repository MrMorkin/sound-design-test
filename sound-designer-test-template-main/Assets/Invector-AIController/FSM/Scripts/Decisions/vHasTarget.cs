using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{

    public class vHasTarget : vStateDecision
    {
        public override string defaultName
        {
            get
            {
                return "Has Target?";
            }
        }

        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            if(fsmBehaviour.aiController==null)
                return false;
            return fsmBehaviour.aiController.currentTarget.transform != null;
        }
    }
}
