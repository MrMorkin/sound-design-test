using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("This is a GoToTarget Action", UnityEditor.MessageType.Info)]
#endif
    public class GoToTarget : vStateAction
    {       
        public override string defaultName
        {
            get
            {
                return "GoToTarget";
            }
        }
        public bool goInStrafe = false;
        public vAIMovementSpeed speed = vAIMovementSpeed.Walking; 
        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            if (fsmBehaviour.aiController == null) return;
            if (executionType == vFSMComponentExecutionType.OnStateEnter) fsmBehaviour.aiController.ForceUpdatePath(2f);
            fsmBehaviour.aiController.SetSpeed(speed);
            if(goInStrafe)
                fsmBehaviour.aiController.StrafeMoveTo(fsmBehaviour.aiController.lastTargetPosition, fsmBehaviour.aiController.lastTargetPosition - fsmBehaviour.transform.position);
            else fsmBehaviour.aiController.MoveTo(fsmBehaviour.aiController.lastTargetPosition);
        }
    }
}