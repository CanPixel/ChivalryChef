using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoad : MonoBehaviour {
	public Image progressBar;

	public Text text;

	[HideInInspector]
	public float progress = 0;

	public List<LoadingTask> loadingTasks = new List<LoadingTask>();
	private int taskIndex = 0;

	[System.Serializable]
	public class LoadingTask {
		public string name;

		public LoadingTask(string name) {
			this.name = name;
		}
	}

	public void SetProgress(string txt, bool initTasks = false) {
		if(initTasks) {
			InitTask(txt.ToLower());
			return;
		} else {
			if(loadingTasks.Count <= 0) return;
			progress = ((float)taskIndex / (float)loadingTasks.Count) * 100f;
			progressBar.fillAmount = progress / 100f;
			if(txt != "") text.text = loadingTasks[taskIndex].name.ToLower();
			taskIndex++;
			if(taskIndex >= loadingTasks.Count) taskIndex = loadingTasks.Count - 1;
		}
	}

	public void InitTask(string txt) {
		loadingTasks.Add(new LoadingTask(txt));
	}


	public void Reset() {
		taskIndex = 0;
		loadingTasks.Clear();
	}
}
