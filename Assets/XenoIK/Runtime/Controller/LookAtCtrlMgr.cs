using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XenoIK
{
    /// <summary>
    /// LookAt管理器，管理该物体上所有的LookAtCtrl
    /// 兼容多头单位
    /// </summary>
    public class LookAtCtrlMgr : MonoBehaviour
    {
        public Transform target;
        public List<LookAtCtrl> ctrls;

        private Transform lastTarget;

        public bool Enable => this.ctrls is { Count: > 0 };

        private void Awake()
        {
            if (!this.Enable) OnInit();
        }

        private void LateUpdate()
        {
            if (this.target != this.lastTarget) SetTarget(this.target);
        }

        public void OnInit()
        {
            this.ctrls = this.transform.GetComponentsInChildren<LookAtCtrl>().ToList();
        }
        
        public void EnableIK()
        {
            this.ctrls?.ForEach(ctrl=>ctrl.EnableIK());
        }
        
        public void DisableIK()
        {
            this.ctrls?.ForEach(ctrl=>ctrl.DisableIK());
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
            this.ctrls?.ForEach(ctrl=>ctrl.SetTarget(target));
            this.lastTarget = this.target;
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