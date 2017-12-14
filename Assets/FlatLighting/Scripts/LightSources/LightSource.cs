/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

using UnityEngine;
using System.Collections.Generic;

namespace FlatLighting {
	public abstract class LightSource<T> : MonoBehaviour where T : LightSource<T> {

		protected static int MAX_LIGHTS = 25; //Don't modify this as it is a limit also included in the shader, and if it's change, the tool will be unstable.
		protected static int lightCount = 0;
		protected static object my_lock = new object();
		protected static LightBag lights = new LightBag();
		protected int Id;

		protected class LightBag: List<T> 
		{
			public new void Remove(T entity)
			{
				base.Remove(entity);
				for (var i = 0; i < Count; i++)
				{
					T e = this[i];
					if(e.Id > entity.Id)
					{
						int oldId = e.Id;
						e.Id--;
						e.UpdatedId(e.Id, oldId);
					}
				}
			}
		}

		protected abstract void UpdatedId(int newId, int oldId);

		protected void InitLightSource(string lightCountProperty) {
			lock(my_lock) {
				if (lightCount >= MAX_LIGHTS) {
					Debug.LogError("Could not initialize a new light source because a limit has been reached");
					return;
				}
				Id = lightCount;
				lights.Add((T)this);
				lightCount++;
				Shader.SetGlobalInt(lightCountProperty, lightCount);
			}
		}

		protected void ReleaseLightSource(string lightCountProperty) {
			lock(my_lock) {
				lights.Remove((T) this);
				lightCount--;
				Shader.SetGlobalInt(lightCountProperty, lightCount);
			}
		}

		protected void DrawSelectedGizmo() {
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(transform.position, 0.25f);
		}
	}
}
