using System.Collections.Generic;
using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
    public class vAICheckDamage : vStateDecision
    {
        public bool lookToDamageSender;

        public override string defaultName
        {
            get
            {
                return "Check Damage";
            }
        }

        public List<string> damageTypeToCheck;


        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            return (HasDamage(fsmBehaviour));
        }

        protected virtual bool HasDamage(vIFSMBehaviourController fsmBehaviour)
        {
            if (fsmBehaviour.aiController == null) return false;
            var hasDamage = (fsmBehaviour.aiController.receivedDamage.isValid) && (damageTypeToCheck.Count == 0 || damageTypeToCheck.Contains(fsmBehaviour.aiController.receivedDamage.lasType));

            if (hasDamage && lookToDamageSender && fsmBehaviour.aiController.receivedDamage.lastSender)
            {
                fsmBehaviour.aiController.LookToTarget(fsmBehaviour.aiController.receivedDamage.lastSender, 5f);                
            }
            if (fsmBehaviour.debugMode)
            {
                fsmBehaviour.SendDebug(Name + " " + (fsmBehaviour.aiController.receivedDamage.isValid)+" "+ fsmBehaviour.aiController.receivedDamage.lastSender, this);
            }         
            
            return hasDamage;
        }
    }

}
