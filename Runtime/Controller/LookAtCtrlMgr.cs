using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XenoIK
{
    public class LookAtCtrlMgr : MonoBehaviour
    {
        public Transform target;
        public List<LookAtCtrl> ctrls;

        private Transform lastTarget;

        public bool Enable => this.ctrls != null && this.ctrls.Count > 0;

        private void Awake()
        {
            if (!this.Enable) this.OnInit();
        }

        private void LateUpdate()
        {
            if (this.target != this.lastTarget && this.target != null)
            {
                this.SetTarget(this.target);
                this.lastTarget = this.target;
            }
        }

        public void OnInit()
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
            this.target = target;
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