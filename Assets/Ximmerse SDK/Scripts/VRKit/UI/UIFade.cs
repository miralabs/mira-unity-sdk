//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ximmerse.UI {

	public class UIFade:MonoBehaviour {

		#region Fields

		public bool autoPlay=false;
		public float delay=0.0f,durationIn=1.0f,durationKeep=0.0f,durationOut=1.0f;
		
		public UnityEngine.Events.UnityEvent onBecameVisible=new UnityEngine.Events.UnityEvent();
		public UnityEngine.Events.UnityEvent onBecameInvisible=new UnityEngine.Events.UnityEvent();

		[System.NonSerialized]protected GameObject m_GameObject;

		[System.NonSerialized]protected bool m_IsPlaying,m_IsFadeOut;
		[System.NonSerialized]protected float m_Time,m_Duration,m_Alpha=/*-*/1.0f;

		[System.NonSerialized]protected List<Graphic> m_Graphics=null;
		[System.NonSerialized]protected float[] m_Alphas=null;
			
		#endregion Fields

		#region Unity Messages

		protected virtual void Awake() {
			m_GameObject=gameObject;
			//
			if(autoPlay) {
				alpha=0.0f;
				if(delay<0) {
					// Do nothing
				}else if(delay==0){
					FadeIn();
				}else{
					StartCoroutine(FadeInDelayed(delay));
				}
				// TODO
				if(durationKeep>0.0f) {// It will fade out automatically.
					onBecameVisible.AddListener(()=>{
						StartCoroutine(FadeOutDelayed(durationKeep));
					});
				}
			}
		}

		protected virtual void Update() {
			if(m_IsPlaying) {
				m_Time+=Time.deltaTime;
				//
				if(m_IsFadeOut) {
					alpha=1.0f-m_Time/m_Duration;
					//
					m_IsPlaying=alpha>0.0f;
				}else {
					alpha=m_Time/m_Duration;
					//
					m_IsPlaying=alpha<1.0f;
				}
				//
				if(!m_IsPlaying) {
					Stop();
					//
					if(m_IsFadeOut) {
						m_GameObject.SetActive(false);
						//
						onBecameInvisible.Invoke();
					}else {
						//
						onBecameVisible.Invoke();
					}
				}
			}
		}

		protected virtual void OnDestroy() {
		}

		#endregion Unity Messages

		#region Methods

		public virtual void Play(bool isVisible) {
			if(isVisible) {
				FadeIn();
			}else {
				FadeOut();
			}
		}
		
		public virtual void FadeIn(float delay,float duration) {
			float t=durationIn;
			durationIn=duration>=0?duration:durationIn;
				FadeIn();
				m_Time-=delay;
			durationIn=t;
		}
		
		public virtual void FadeOut(float delay,float duration) {
			float t=durationOut;
			durationOut=duration>=0?duration:durationOut;
				FadeOut();
				m_Time-=delay;
			durationOut=t;
		}

		/// <summary>
		/// An effect for UI Elements,when touchpad is down.
		/// </summary>
		public virtual void FadeInWhenTouch() {
			if(!m_IsPlaying) {
				if(alpha==1.0f){
					onBecameVisible.Invoke();
				}else {
					Play(true);
				}
			}else if(m_IsFadeOut) {
				Play(true);
			}
		}

		// TODO: Why STOP it before playing?
		// e.g.:Call FadeIn() when alpha equals 0.0f,then Call FadeOut() before
		// Update() at next frame,it will still do FadeIn().

		public virtual void FadeIn() {
			// Stop firstly.
			Stop();
			//
			if(alpha==1.0f){return;}
			//
			m_IsPlaying=true;
			m_IsFadeOut=false;
			//
			m_Time=alpha*durationIn;
			m_Duration=durationIn;
			//alpha=0.0f;
			if(m_GameObject==null) {
				m_GameObject=gameObject;
			}
			m_GameObject.SetActive(true);
		}

		public virtual void FadeOut() {
			// Stop firstly.
			Stop();
			//
			if(alpha==0.0f){return;}
			//
			m_IsPlaying=true;
			m_IsFadeOut=true;
			//
			m_Time=(1.0f-alpha)*durationOut;
			m_Duration=durationOut;
			//alpha=1.0f;
			if(m_GameObject==null) {
				m_GameObject=gameObject;
			}
			m_GameObject.SetActive(true);
		}

		public virtual void Stop() {
			//StopAllCoroutines();
			++m_DelayCount;
			//
			m_IsPlaying=false;
		}

		protected int m_DelayCount;

		protected virtual System.Collections.IEnumerator FadeInDelayed(float duration) {
			int dc=++m_DelayCount;
			yield return new WaitForSeconds(duration);
			if(dc==m_DelayCount) {
				FadeIn();
			}
		}

		protected virtual System.Collections.IEnumerator FadeOutDelayed(float duration) {
			int dc=++m_DelayCount;
			yield return new WaitForSeconds(duration);
			if(dc==m_DelayCount) {
				FadeOut();
			}
		}
		
		#endregion Methods

		#region Properties

		public virtual float alpha {
			get {
				return Mathf.Clamp01(m_Alpha);
			}
			set {
				//
				value=Mathf.Clamp01(value);
				//
				if(m_Alpha!=value) {
					m_Alpha=value;
					//
					if(m_Graphics==null) {
						m_Graphics=new List<Graphic>(GetComponentsInChildren<Graphic>());
						//m_Graphics.RemoveAll((x)=>!x.isActiveAndEnabled);
						int i=0,imax=m_Graphics.Count;
						m_Alphas=new float[imax];
						for(;i<imax;++i) {
							m_Alphas[i]=m_Graphics[i].color.a;
						}
					}
					//
					Graphic g;
					Color color;
					for(int i=0,imax=m_Graphics.Count;i<imax;++i) {
						g=m_Graphics[i];
						if(g!=null) {
							color=g.color;
							color.a=m_Alpha*m_Alphas[i];
							g.color=color;
						}
					}
				}
			}
		}

		public virtual bool isPlaying {
			get {
				return m_IsPlaying;
			}
		}

		public virtual bool isFadingIn {
			get {
				return m_IsPlaying&&!m_IsFadeOut;
			}
		}

		public virtual bool isFadingOut {
			get {
				return m_IsPlaying&&m_IsFadeOut;
			}
		}

		#endregion Properties

	}

}