using UnityEngine;

namespace SeinoIK
{
    public class IKManager : MonoBehaviour
    {
        public LookAtCtrlMgr LookAtCtrlMgr;
        public FootIKCtrl FootIKCtrl;

        private void Awake()
        {
            LookAtCtrlMgr = this.transform.GetComponentInChildren<LookAtCtrlMgr>();
            FootIKCtrl = this.transform.GetComponentInChildren<FootIKCtrl>();
        }
    }
}