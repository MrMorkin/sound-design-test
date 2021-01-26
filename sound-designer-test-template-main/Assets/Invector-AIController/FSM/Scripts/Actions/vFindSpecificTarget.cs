using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("This is a vFindSpecificTarget Action", UnityEditor.MessageType.Info)]
#endif
    public class vFindSpecificTarget : vStateAction
    {       
        public override string defaultName
        {
            get
            {
                return "vFindSpecificTarget";
            }
        }
        public LayerMask _detectLayer;
        public vTagMask _detectTags;
        public float _maxDistanceToDetect;
        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            FindTarget(fsmBehaviour.aiController);
        }
        public virtual void FindTarget(vIControlAI vIControl)
        {
            var transform = vIControl.transform;
            var currentTarget = vIControl.currentTarget;

            var targets = Physics.OverlapSphere(transform.position + transform.up, _maxDistanceToDetect, _detectLayer);
            Transform target = null;
            var _targetDistance = Mathf.Infinity;

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != null && targets[i].transform != transform && _detectTags.Contains(targets[i].gameObject.tag))
                {
                    //Debug.Log(targets[i].name);
                   // if (findTargetByDistance)
                    {
                        var newTargetDistance = Vector3.Distance(targets[i].transform.position, transform.position);
                        var character = targets[i].GetComponent<vIHealthController>();
                        if (character != null && !character.isDead && newTargetDistance < _targetDistance)
                        {
                            target = targets[i].transform;
                            _targetDistance = newTargetDistance;
                        }
                    }
                  
                }
            }

            if (target != null)
            {
                vIControl.SetCurrentTarget(target);
            }
        }
    }
}