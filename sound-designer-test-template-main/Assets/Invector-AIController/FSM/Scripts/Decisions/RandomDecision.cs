using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("This is a RandomDecision decision", UnityEditor.MessageType.Info)]
#endif
    public class RandomDecision : vStateDecision
    {
        public override string defaultName
        {
            get
            {
                return "RandomDecision";
            }
        }
        
        [Range(0,100)][Tooltip("Percentage Change between true and false")]
        public float randomTrueFalse;
        public float frequency;
        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            if(frequency>0)
            {
                if (InTimer(fsmBehaviour, frequency))
                    return Random.Range(0, 100) < randomTrueFalse;
                else return false;
            }
            //TODO
            return Random.Range(0,100)<randomTrueFalse;
        }
    }
}
