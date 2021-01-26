using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("This is a vCheckTargetDistance decision", UnityEditor.MessageType.Info)]
#endif
    public class vCheckTargetDistance : vStateDecision
    {
        public override string defaultName
        {
            get
            {
                return "vCheckTargetDistance";
            }
        }

        protected enum CompareValueMethod
        {
            Greater,Less,Equal
        }
        [SerializeField]
        protected CompareValueMethod compareMethod;
        public float distance;
        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            if (!fsmBehaviour.aiController.currentTarget.transform) return false;
            var dist = fsmBehaviour.aiController.targetDistance;
            switch (compareMethod)
            {
                case CompareValueMethod.Equal:
                    return dist.Equals(distance);
                case CompareValueMethod.Greater:
                    return dist>distance;
                case CompareValueMethod.Less:
                    return dist< distance;
            }
            return false;
        }
    }
}
