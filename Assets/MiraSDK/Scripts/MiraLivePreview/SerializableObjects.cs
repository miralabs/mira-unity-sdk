// Copyright (c) Mira Labs, Inc., 2017. All rights reserved.
//
// Licensed under the MIT License.
// See LICENSE.md at the root of this project for more details.

using System;
using UnityEngine;
using UnityEngine.XR.iOS;
using System.Text;

namespace Utils
{
	

	/// <summary>
	/// Since unity doesn't flag the Vector3 as serializable, we
	/// need to create our own version. This one will automatically convert
	/// between Vector3 and SerializableVector3
	/// </summary>
	[System.Serializable]
	public struct SerializableVector3
	{
		/// <summary>
		/// x component
		/// </summary>
		public float x;
		
		/// <summary>
		/// y component
		/// </summary>
		public float y;
		
		/// <summary>
		/// z component
		/// </summary>
		public float z;
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="rX"></param>
		/// <param name="rY"></param>
		/// <param name="rZ"></param>
		public SerializableVector3(float rX, float rY, float rZ)
		{
			x = rX;
			y = rY;
			z = rZ;
		}
		
		/// <summary>
		/// Returns a string representation of the object
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("[{0}, {1}, {2}]", x, y, z);
		}
		
		/// <summary>
		/// Automatic conversion from SerializableVector3 to Vector3
		/// </summary>
		/// <param name="rValue"></param>
		/// <returns></returns>
		public static implicit operator Vector3(SerializableVector3 rValue)
		{
			return new Vector3(rValue.x, rValue.y, rValue.z);
		}
		
		/// <summary>
		/// Automatic conversion from Vector3 to SerializableVector3
		/// </summary>
		/// <param name="rValue"></param>
		/// <returns></returns>
		public static implicit operator SerializableVector3(Vector3 rValue)
		{
			return new SerializableVector3(rValue.x, rValue.y, rValue.z);
		}
	}
	/// <summary>
	/// Since unity doesn't flag the Vector2 as serializable, we
	/// need to create our own version. This one will automatically convert
	/// between Vector3 and SerializableVector3
	/// </summary>
	[System.Serializable]
	public struct SerializableVector2
	{
		/// <summary>
		/// x component
		/// </summary>
		public float x;

		/// <summary>
		/// y component
		/// </summary>
		public float y;



		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="rX"></param>
		/// <param name="rY"></param>
		public SerializableVector2(float rX, float rY)
		{
			x = rX;
			y = rY;
			
		}

		/// <summary>
		/// Returns a string representation of the object
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("[{0}, {1}]", x, y);
		}

		/// <summary>
		/// Automatic conversion from SerializableVector3 to Vector3
		/// </summary>
		/// <param name="rValue"></param>
		/// <returns></returns>
		public static implicit operator Vector2(SerializableVector2 rValue)
		{
			return new Vector2(rValue.x, rValue.y);
		}

		/// <summary>
		/// Automatic conversion from Vector3 to SerializableVector3
		/// </summary>
		/// <param name="rValue"></param>
		/// <returns></returns>
		public static implicit operator SerializableVector2(Vector2 rValue)
		{
			return new SerializableVector2(rValue.x, rValue.y);
		}
	}


	/// <summary>
	/// Since unity doesn't flag the Vector4 as serializable, we
	/// need to create our own version. This one will automatically convert
	/// between Vector4 and SerializableVector4
	/// </summary>
	[Serializable]
	public class SerializableVector4
	{
		/// <summary>
		/// x component
		/// </summary>
		public float x;

		/// <summary>
		/// y component
		/// </summary>
		public float y;

		/// <summary>
		/// z component
		/// </summary>
		public float z;

		/// <summary>
		/// w component
		/// </summary>
		public float w;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="rX"></param>
		/// <param name="rY"></param>
		/// <param name="rZ"></param>
		/// <param name="rW"></param>
		public SerializableVector4(float rX, float rY, float rZ, float rW)
		{
			x = rX;
			y = rY;
			z = rZ;
			w = rW;
		}

		/// <summary>
		/// Returns a string representation of the object
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("[{0}, {1}, {2}, {3}]", x, y, z, w);
		}

		/// <summary>
		/// Automatic conversion from SerializableVector4 to Vector4
		/// </summary>
		/// <param name="rValue"></param>
		/// <returns></returns>
		public static implicit operator Vector4(SerializableVector4 rValue)
		{
			return new Vector4(rValue.x, rValue.y, rValue.z, rValue.w);
		}

		/// <summary>
		/// Automatic conversion from Vector4 to SerializableVector4
		/// </summary>
		/// <param name="rValue"></param>
		/// <returns></returns>
		public static implicit operator SerializableVector4(Vector4 rValue)
		{
			return new SerializableVector4(rValue.x, rValue.y, rValue.z, rValue.w);
		}
	}

	/// <summary>
	/// Since unity doesn't flag the Quaternion as serializable, we
	/// need to create our own version. This one will automatically convert
	/// between Quaternion and SerializableQuaternion
	/// </summary>
	[System.Serializable]
	public struct SerializableQuaternion
	{
		/// <summary>
		/// x component
		/// </summary>
		public float x;
		
		/// <summary>
		/// y component
		/// </summary>
		public float y;
		
		/// <summary>
		/// z component
		/// </summary>
		public float z;
		
		/// <summary>
		/// w component
		/// </summary>
		public float w;
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="rX"></param>
		/// <param name="rY"></param>
		/// <param name="rZ"></param>
		/// <param name="rW"></param>
		public SerializableQuaternion(float rX, float rY, float rZ, float rW)
		{
			x = rX;
			y = rY;
			z = rZ;
			w = rW;
		}
		
		/// <summary>
		/// Returns a string representation of the object
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("[{0}, {1}, {2}, {3}]", x, y, z, w);
		}
		
		/// <summary>
		/// Automatic conversion from SerializableQuaternion to Quaternion
		/// </summary>
		/// <param name="rValue"></param>
		/// <returns></returns>
		public static implicit operator Quaternion(SerializableQuaternion rValue)
		{
			return new Quaternion(rValue.x, rValue.y, rValue.z, rValue.w);
		}
		
		/// <summary>
		/// Automatic conversion from Quaternion to SerializableQuaternion
		/// </summary>
		/// <param name="rValue"></param>
		/// <returns></returns>
		public static implicit operator SerializableQuaternion(Quaternion rValue)
		{
			return new SerializableQuaternion(rValue.x, rValue.y, rValue.z, rValue.w);
		}
	}
	[System.Serializable]
	/// <summary>
	/// To keep things in a consistent format, here is a serializeable float!
	/// </summary>
	public class serializableFloat
	{
		public float x;
		public serializableFloat(float rX)
		{
			x = rX;
		}
		public override string ToString()
		{
			return String.Format("[{0}", x);
		}

		public static implicit operator float(serializableFloat rValue)
		{
			return float.Parse(rValue.x.ToString());
		}
		
		
		public static implicit operator serializableFloat(float rValue)
		{
			return new serializableFloat(rValue);
		}
	}
	/// <summary>
	/// Serialized attitude and acceleration of a device's gyroscope
	/// </summary>
	[Serializable]  
	public class serializableGyroscope {
		public SerializableQuaternion attitude;
		public SerializableVector3 userAcceleration;

		public serializableGyroscope(SerializableQuaternion a, SerializableVector3 uA) {
			attitude = a;
			userAcceleration = uA;
		}

		public static implicit operator serializableGyroscope(Gyroscope rValue) {
			return new serializableGyroscope(rValue.attitude, rValue.userAcceleration);
		}
	}

	[Serializable]
	/// <summary>
	/// This serializes an object's position and rotation
	/// </summary>
	public class serializableTransform
	{
		public SerializableVector3 position;
		public SerializableQuaternion rotation;

		public serializableTransform(SerializableVector3 p, SerializableQuaternion r) {
			position = p;
			rotation = r;
		}
	
		public static implicit operator serializableTransform(Transform _object) {
			return new serializableTransform(_object.position, _object.rotation);
		}


	}

	/// <summary>
	/// Serializes the orientation, rotationRate, and acceleration of a Mira Prism Remote
	/// </summary>
	[Serializable]
	public class serializableBTRemote {
		
		public SerializableVector3 orientation;
		public SerializableVector3 rotationRate;
		public SerializableVector3 acceleration;

		public serializableBTRemote(SerializableVector3 _orientation, SerializableVector3 _rotationRate, SerializableVector3 _acceleration) {
			orientation = _orientation;
			rotationRate = _rotationRate;
			acceleration = _acceleration;
		}

		public static implicit operator serializableBTRemote(Remote _btValue) {
			return new serializableBTRemote(_btValue.motion.orientation.getOrientationVector(), _btValue.motion.rotationRate.getMotionSensorVector(), _btValue.motion.acceleration.getMotionSensorVector());
		}
	}


	[Serializable]
	/// <summary>
	/// Serializes the touchPad axes and buttons on the Mira Prism remote
	/// </summary>
	public class serializableBTRemoteTouchPad {
		
		public bool touchActive;
		public bool touchButton;
		public SerializableVector2 touchPos;
		
		public bool upButton;
		public bool downButton;
		public bool leftButton;
		public bool rightButton;

		public serializableBTRemoteTouchPad(bool _TouchActive, bool _TouchpadButton,
			SerializableVector2 _TouchPos, bool _UpButton, 
			bool _DownButton, bool _LeftButton, 
			bool _RightButton) 
		{
			touchActive = _TouchActive;

			touchPos = _TouchPos;

			touchButton = _TouchpadButton;

			upButton = _UpButton;

			downButton = _DownButton;

			leftButton = _LeftButton;

			rightButton = _RightButton;
		}


		public static implicit operator serializableBTRemoteTouchPad(Remote _tpValue) {
			return new serializableBTRemoteTouchPad(
				_tpValue.touchPad.isActive,
				_tpValue.touchPad.button.isPressed,
				new SerializableVector2(_tpValue.touchPad.xAxis.value, _tpValue.touchPad.yAxis.value),
				_tpValue.touchPad.up.isActive, 
				_tpValue.touchPad.down.isActive,  
				_tpValue.touchPad.left.isActive,
				_tpValue.touchPad.right.isActive
			);
		}
	}

	[Serializable]
	/// <summary>
	/// Serializes the buttons (start, back, trigger) on the Mira Prism remote
	/// </summary>
	public class serializableBTRemoteButtons {
		public bool startButton;
		
		public bool backButton;
		
		public bool triggerButton;
		

		public serializableBTRemoteButtons(bool _startButton, bool _backButton, bool _triggerButton) {

			startButton = _startButton;

			backButton = _backButton;

			triggerButton = _triggerButton;
			
		}

		public static implicit operator serializableBTRemoteButtons(Remote _bValue)
		{
			return new serializableBTRemoteButtons(_bValue.homeButton.isPressed, _bValue.menuButton.isPressed, _bValue.trigger.isPressed);
		}
	}


	[Serializable]
	public class serializablePointCloud
	{
		public byte [] pointCloudData;

		public serializablePointCloud(byte [] inputPoints)
		{
			pointCloudData = inputPoints;
		}

		public static implicit operator serializablePointCloud(Vector3 [] vecPointCloud)
		{
			if (vecPointCloud != null)
			{
				byte [] createBuf = new byte[vecPointCloud.Length * sizeof(float) * 3];
				for(int i = 0; i < vecPointCloud.Length; i++)
				{
					int bufferStart = i * 3;
					Buffer.BlockCopy( BitConverter.GetBytes( vecPointCloud[i].x ), 0, createBuf, (bufferStart)*sizeof(float), sizeof(float) );
					Buffer.BlockCopy( BitConverter.GetBytes( vecPointCloud[i].y ), 0, createBuf, (bufferStart+1)*sizeof(float), sizeof(float) );
					Buffer.BlockCopy( BitConverter.GetBytes( vecPointCloud[i].z ), 0, createBuf, (bufferStart+2)*sizeof(float), sizeof(float) );

				}
				return new serializablePointCloud (createBuf);
			}
			else 
			{
				return new serializablePointCloud(null);
			}
		}

		public static implicit operator Vector3 [] (serializablePointCloud spc)
		{
			if (spc.pointCloudData != null) 
			{
				int numVectors = spc.pointCloudData.Length / (3 * sizeof(float));
				Vector3 [] pointCloudVec = new Vector3[numVectors];
				for (int i = 0; i < numVectors; i++) 
				{
					int bufferStart = i * 3;
					pointCloudVec [i].x = BitConverter.ToSingle (spc.pointCloudData, (bufferStart) * sizeof(float));
					pointCloudVec [i].y = BitConverter.ToSingle (spc.pointCloudData, (bufferStart+1) * sizeof(float));
					pointCloudVec [i].z = BitConverter.ToSingle (spc.pointCloudData, (bufferStart+2) * sizeof(float));
					
				}
				return pointCloudVec;
			} 
			else 
			{
				return null;
			}
		}
	};



	[Serializable]
	public class serializableFromEditorMessage {
		public Guid subMessageId;
		public byte[] bytes;
	};
}