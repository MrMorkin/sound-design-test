using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("This is a AIIsShotting decision", UnityEditor.MessageType.Info)]
#endif
    public class AIIsShotting : vStateDecision
    {
        public override string defaultName
        {
            get
            {
                return "Is Shotting?";
            }
        }

        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            if(fsmBehaviour.aiController is vIControlAIShooter)
            {
                vIControlAIShooter shooter = (fsmBehaviour.aiController as vIControlAIShooter);
                return shooter.shooterManager ? shooter.shooterManager.isShooting || shooter.isAttacking:false;
            }
            //TODO
            return true;
        }
    }
}
