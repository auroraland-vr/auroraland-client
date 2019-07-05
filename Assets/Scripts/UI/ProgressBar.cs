using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Auroraland{
	public class ProgressBar : MonoBehaviour {

		public Text TaskCount;
		public Text Progress;
		public Image ProgressIndicator;
		private bool isProgressing;
		private string taskCount;

		// Use this for initialization
		void Start(){
			if(TaskCount) taskCount = TaskCount.text;
		}
		
		// Update is called once per frame
		void Update () {
		}
		public void SetTaskCount(int progressedTotal, int total){ // show progressed task count / total task count
            TaskCount.text = string.Format ("({0}/{1})", progressedTotal, total);
		}
		public float GetProgress(){
			return ProgressIndicator.fillAmount;
		}

		public void SetProgress(float value){
            ProgressIndicator.fillAmount = value;
            if(Progress!=null)Progress.text = value.ToString("0.#%");
            isProgressing = (value > 0 && value < 1f) ? true : false;
        }
	}
}
