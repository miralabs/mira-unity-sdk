using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(Text))]
public class FPSCounter:MonoBehaviour {

	public float fpsMeasurePeriod = 0.5f;
	public string display = "FPS {0}";
	public System.Func<int> getFrameCount=null;

	private int m_FpsAccumulator = 0;
	private int m_PrevFpsAccumulator = 0;
	private float m_FpsNextPeriod = 0;

	private float m_CurrentFps;
	private Text m_Text;

	private void Start() {
		m_FpsNextPeriod=Time.realtimeSinceStartup+fpsMeasurePeriod;
		m_Text=GetComponent<Text>();
	}


	private void Update() {
		// measure average frames per second
		if(getFrameCount==null) {
			m_FpsAccumulator++;
		}else {
			m_FpsAccumulator=getFrameCount();
		}
		if(Time.realtimeSinceStartup>m_FpsNextPeriod) {
			m_CurrentFps=((m_FpsAccumulator-m_PrevFpsAccumulator)/fpsMeasurePeriod);
			m_PrevFpsAccumulator=m_FpsAccumulator;
			m_FpsNextPeriod+=fpsMeasurePeriod;
			m_Text.text=string.Format(display,m_CurrentFps);
		}
	}
}
