using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XenoIK
{
    public class LookAtCtrlMgr : MonoBehaviour
    {
        public List<LookAtCtrl> ctrls;

        private void Awake()
        {
            this.ctrls = this.transform.GetComponentsInChildren<LookAtCtrl>().ToList();
        }
        
        public void OpenIK()
        {
            this.ctrls?.ForEach(ctrl=>ctrl.OpenIK());
        }
        
        public void CloseIK()
        {
            this.ctrls?.ForEach(ctrl=>ctrl.CloseIK());
        }

        public void SetTarget(Transform target)
        {
            this.ctrls?.ForEach(ctrl=>ctrl.SetTarget(target));
        }

        public void SetPoint(Vector3 point)
        {
            this.ctrls?.ForEach(ctrl=>ctrl.SetLookAtPoint(point));
        }

        public void ResetTarget()
        {
            this.ctrls?.ForEach(ctrl=>ctrl.ResetTarget());
        }
    }
}