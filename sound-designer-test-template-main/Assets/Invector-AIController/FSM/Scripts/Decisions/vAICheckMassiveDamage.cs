using System.Collections.Generic;
using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
    public class vAICheckMassiveDamage : vStateDecision
    {
        [vToggleOption("Compare Value","Less","GreaterEqual")]
        public bool greater = false;
        [vToggleOption("Compare Type", "Massive Count", "Massive Value")]
        public bool massiveValue = true;

        public int valueToCompare;
        public override string defaultName
        {
            get
            {
                return "Check Massive Damage";
            }
        }
        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            return (HasDamage(fsmBehaviour));
        }

        protected virtual bool HasDamage(vIFSMBehaviourController fsmBehaviour)
        {
            if (fsmBehaviour.aiController.receivedDamage == null) return false;
            var value = massiveValue? fsmBehaviour.aiController.receivedDamage.massiveValue: fsmBehaviour.aiController.receivedDamage.massiveCount; 
            return (greater?(value>=valueToCompare):(value<valueToCompare));
        }
    }

}
