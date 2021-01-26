using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Invector.vCharacterController.AI.FSMBehaviour
{
    public class vAIHearNoise : vStateDecision
    {
        public override string defaultName
        {
            get
            {
                return "Is listening noise";
            }
        }
        [vToggleOption("Noise Type","Any Noise","Specific Noise")]
        public bool specific;
        [vHideInInspector("specific")]
        public List<string> noiseTypes;
        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            if(fsmBehaviour.aiController!=null)
            {
                if(fsmBehaviour.aiController.HasComponent<vAINoiseListener>())
                {
                    var noiseListener = fsmBehaviour.aiController.GetAIComponent<vAINoiseListener>();
                    if (specific) return noiseListener.IsListeningSpecificNoises(noiseTypes);
                    else return noiseListener.IsListeningNoise();
                }
            }
            return false;
        }
    }
}
