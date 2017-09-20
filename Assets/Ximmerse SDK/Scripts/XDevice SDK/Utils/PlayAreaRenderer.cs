//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using UnityEngine;

/// <summary>
/// 
/// </summary>
//[RequireComponent(typeof(MeshRenderer),typeof(MeshFilter))]
public class PlayAreaRenderer:MonoBehaviour {

	#region Nested Types

	public enum Style{
		A,
		B,
		C
	}

	#endregion Nested Types

	#region Static

	public static GameObject CreateRenderedObject(string name,Mesh mesh,Material material) {
		GameObject go=new GameObject(name,typeof(MeshFilter),typeof(MeshRenderer));

		var filter=go.GetComponent<MeshFilter>();
		filter.sharedMesh=mesh;

		var renderer=go.GetComponent<MeshRenderer>();
		renderer.sharedMaterial=material;

		renderer.reflectionProbeUsage=UnityEngine.Rendering.ReflectionProbeUsage.Off;
		renderer.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;
		renderer.receiveShadows=false;
#if !(UNITY_5_3||UNITY_5_2||UNITY_5_1||UNITY_5_0)
		renderer.lightProbeUsage=UnityEngine.Rendering.LightProbeUsage.Off;
#else
		renderer.useLightProbes = false;
#endif
		return go;
	}

	public static void SetParent(Transform t,Transform parent) {
		t.SetParent(parent);
		t.localPosition=Vector3.zero;
		t.localRotation=Quaternion.identity;
		t.localScale=Vector3.one;
	}

	/// <summary>
	/// Taken from SteamVR_PlayArea.BuildMesh() in SteamVR SDK.
	/// </summary>
	public static Mesh DrawGround(Mesh mesh,Vector3[] corners,float borderThickness,Color startColor,Color endColor) {
		if(mesh==null) {
			mesh=new Mesh();
		}
		//
		Vector3[] vertices = new Vector3[corners.Length*2];
		for(int i=0,imax=corners.Length;i<imax;++i) {
			var c = corners[i];
			vertices[i]=new Vector3(c.x,0.01f,c.z);
		}

		for(int i=0,imax=corners.Length;i<imax;++i) {
			int next = (i+1)%corners.Length;
			int prev = (i+corners.Length-1)%corners.Length;

			var nextSegment = (vertices[next]-vertices[i]).normalized;
			var prevSegment = (vertices[prev]-vertices[i]).normalized;

			var vert = vertices[i];
			//vert+=Vector3.Cross(nextSegment,Vector3.up)*borderThickness;
			//vert+=Vector3.Cross(prevSegment,Vector3.down)*borderThickness;
			float x=Mathf.Sqrt(1-Mathf.Pow((nextSegment+prevSegment).sqrMagnitude-2,2)/4);// Magic function????
			vert-=(nextSegment+prevSegment)*(borderThickness/x);

			vertices[corners.Length+i]=vert;
		}

		var triangles = new int[corners.Length*6];
		for(int i=0,imax=corners.Length,j=0;i<imax;++i) {
			int next=(i+1)%imax;
			/*
			0, 1, 4,
			1, 5, 4,
			*/
			//
			triangles[j++]=i;
			triangles[j++]=next;
			triangles[j++]=imax+i;

			triangles[j++]=next;
			triangles[j++]=imax+next;
			triangles[j++]=imax+i;
		}

		var uv = new Vector2[corners.Length*2];
		for(int i=0,imax=corners.Length,j=0;i<imax;++i) {
			uv[j++]=new Vector2(0.0f, 0.0f);
			uv[j++]=new Vector2(1.0f, 0.0f);
		}

		var colors = new Color[corners.Length*2];
		for(int i=0,imax=corners.Length;i<imax;++i) {
			colors[i]=startColor;
			colors[imax+i]=endColor;
		}

		mesh.vertices=vertices;
		mesh.uv=uv;
		mesh.colors=colors;
		mesh.triangles=triangles;

		//mesh.RecalculateNormals();
		//mesh.RecalculateBounds();
		return mesh;
	}
	
	public static int DrawLine(
		Vector3[] vertices,int index,Vector3 right,float widthL,float widthR,
		Vector3 startPoint,Vector3 endPoint,
		float startPadding,float endPadding
	) {
		Vector3 direction=(endPoint-startPoint).normalized;
		startPoint+=startPadding*direction;
		endPoint-=endPadding*direction;
		//
		vertices[index++]=startPoint-(widthL)*right;
		vertices[index++]=startPoint+(widthR)*right;
		vertices[index++]=endPoint-(widthL)*right;
		vertices[index++]=endPoint+(widthR)*right;
		return index;
	}

	public static Mesh DrawWall(Mesh mesh,
		Color color,float thickness,float cellSize,float emptySize,
		Vector3 position,Quaternion rotation,float width,float height
	) {
		if(mesh==null) {
			mesh=new Mesh();
		}
		//
		int i,j,imax,jmax;
		int numCols=Mathf.CeilToInt(width/cellSize),
			numRows=Mathf.CeilToInt(height/cellSize);// of cells.
		int numVertices=4*((numCols+1)*numRows+numCols*(numRows+1));
		int numQuads=numVertices/4;
		int pVertices=0;
		float thicknessL,thicknessR;
		Vector3 right,up,point;
		//
		Vector3[] vertices=new Vector3[numVertices];
		right=(rotation*Vector3.right);
		up=(rotation*Vector3.up);
		// Draw vertical lines.
		for(j=0,jmax=numCols+1;j<jmax;++j) {
			point=position+right*Mathf.Clamp(j*cellSize,0,width);
			thicknessL=(j==0?0.0f:.5f)*thickness;
			thicknessR=(j==numCols?0.0f:.5f)*thickness;
			for(i=0,imax=numRows;i<imax;++i) {
				pVertices=DrawLine(vertices,pVertices,
					right,thicknessL,thicknessR,
					point+up*(i*cellSize),
					point+up*Mathf.Clamp((i+1)*cellSize,0,height),
					emptySize*.5f,emptySize*.5f
				);
			}
		}
		// Draw horizontal lines.
		for(i=0,imax=numRows+1;i<imax;++i) {
			point=position+up*Mathf.Clamp(i*cellSize,0,height);
			thicknessL=(i==0?0.0f:.5f)*thickness;
			thicknessR=(i==numRows?0.0f:.5f)*thickness;
			for(j=0,jmax=numCols;j<jmax;++j) {
				pVertices=DrawLine(vertices,pVertices,
					up,thicknessL,thicknessR,
					point+right*Mathf.Clamp((j+1)*cellSize,0,width),
					point+right*(j*cellSize),
					emptySize*.5f,emptySize*.5f
				);
			}
		}
		//
		if(mesh.vertexCount==numVertices) {// Faster mode
			mesh.vertices=vertices;

			//mesh.RecalculateNormals();
			//mesh.RecalculateBounds();
		}else {// Full mode
			mesh.Clear();
			//
			Vector2[] uvs=new Vector2[numVertices];
			for(i=0;i<numVertices;) {
				uvs[i++]=new Vector2(0.0f,0.0f);
				uvs[i++]=new Vector2(1.0f,0.0f);
				uvs[i++]=new Vector2(0.0f,1.0f);
				uvs[i++]=new Vector2(1.0f,1.0f);
			}
			//
			Color[] colors=new Color[numVertices];
			i=numVertices;
			while(i-->0) {
				colors[i]=color;
			}
			//
			int[] triangles=new int[numQuads*6];
			for(i=0,j=0;i<numQuads;++i) {
				triangles[j++]=4*i+0;
				triangles[j++]=4*i+2;
				triangles[j++]=4*i+1;

				triangles[j++]=4*i+1;
				triangles[j++]=4*i+2;
				triangles[j++]=4*i+3;
			}
			//
			mesh.vertices=vertices;
			mesh.uv=uvs;
			mesh.colors=colors;
			mesh.triangles=triangles;

			//mesh.RecalculateNormals();
			//mesh.RecalculateBounds();
		}
		//
		return mesh;
	}

	public static Mesh DrawWall_StyleB(Mesh mesh,
		Color color,float thickness,float cellSize,float emptySize,
		Vector3 position,Quaternion rotation,float width,float height
	) {
		if(mesh==null) {
			mesh=new Mesh();
		}
		//
		int i,j,imax,jmax;
		int numCols=Mathf.CeilToInt(width/cellSize),
			numRows=Mathf.CeilToInt(height/cellSize);// of cells.
		int numVertices=4*4*((numCols+1)*numRows+numCols*(numRows+1));
		int numQuads=numVertices/4;
		int pVertices=0;
		float thicknessL,thicknessR;
		Vector3 right,up,point;
		//
		Vector3[] vertices=new Vector3[numVertices];
		right=(rotation*Vector3.right);
		up=(rotation*Vector3.up);
		// Draw vertical lines.
		for(j=0,jmax=numCols+1;j<jmax;++j) {
			point=position+right*Mathf.Clamp(j*cellSize,0,width);
			thicknessL=
			thicknessR=.5f*thickness;
			for(i=0,imax=numRows;i<imax;++i) {
				if(j!=0) {
					pVertices=DrawLine(vertices,pVertices,
						right,thicknessL,thicknessR,
						point+up*(i*cellSize+emptySize*0.5f)-right*(emptySize*0.5f),
						point+up*(i*cellSize+emptySize*1.0f)-right*(emptySize*0.5f),
						thickness*0.5f,0.0f
					);
					pVertices=DrawLine(vertices,pVertices,
						right,thicknessL,thicknessR,
						point+up*((i+1)*cellSize-emptySize*1.0f)-right*(emptySize*0.5f),
						point+up*((i+1)*cellSize-emptySize*0.5f)-right*(emptySize*0.5f),
						0.0f,thickness*0.5f
					);
				}
				if(j!=numCols) {
					pVertices=DrawLine(vertices,pVertices,
						right,thicknessL,thicknessR,
						point+up*(i*cellSize+emptySize*0.5f)+right*(emptySize*0.5f),
						point+up*(i*cellSize+emptySize*1.0f)+right*(emptySize*0.5f),
						thickness*0.5f,0.0f
					);
					pVertices=DrawLine(vertices,pVertices,
						right,thicknessL,thicknessR,
						point+up*((i+1)*cellSize-emptySize*1.0f)+right*(emptySize*0.5f),
						point+up*((i+1)*cellSize-emptySize*0.5f)+right*(emptySize*0.5f),
						0.0f,thickness*0.5f
					);
				}
			}
		}
		// Draw horizontal lines.
		for(i=0,imax=numRows+1;i<imax;++i) {
			point=position+up*Mathf.Clamp(i*cellSize,0,height);
			thicknessL=
			thicknessR=.5f*thickness;
			for(j=0,jmax=numCols;j<jmax;++j) {
				if(i!=0) {
					pVertices=DrawLine(vertices,pVertices,
						up,thicknessL,thicknessR,
						point+right*(j*cellSize+emptySize*1.0f)-up*(emptySize*0.5f),
						point+right*(j*cellSize+emptySize*0.5f)-up*(emptySize*0.5f),
						0.0f,-thickness*0.5f
					);
					pVertices=DrawLine(vertices,pVertices,
						up,thicknessL,thicknessR,
						point+right*(Mathf.Clamp((j+1)*cellSize,0,width)-emptySize*0.5f)-up*(emptySize*0.5f),
						point+right*(Mathf.Clamp((j+1)*cellSize,0,width)-emptySize*1.0f)-up*(emptySize*0.5f),
						-thickness*0.5f,0.0f
					);
				}
				if(i!=numRows) {
					pVertices=DrawLine(vertices,pVertices,
						up,thicknessL,thicknessR,
						point+right*(j*cellSize+emptySize*1.0f)+up*(emptySize*0.5f),
						point+right*(j*cellSize+emptySize*0.5f)+up*(emptySize*0.5f),
						0.0f,-thickness*0.5f
					);
					pVertices=DrawLine(vertices,pVertices,
						up,thicknessL,thicknessR,
						point+right*(Mathf.Clamp((j+1)*cellSize,0,width)-emptySize*0.5f)+up*(emptySize*0.5f),
						point+right*(Mathf.Clamp((j+1)*cellSize,0,width)-emptySize*1.0f)+up*(emptySize*0.5f),
						-thickness*0.5f,0.0f
					);
				}
			}
		}
		//
		if(mesh.vertexCount==numVertices) {// Faster mode
			mesh.vertices=vertices;

			//mesh.RecalculateNormals();
			//mesh.RecalculateBounds();
		}else {// Full mode
			mesh.Clear();
			//
			Vector2[] uvs=new Vector2[numVertices];
			for(i=0;i<numVertices;) {
				uvs[i++]=new Vector2(0.0f,0.0f);
				uvs[i++]=new Vector2(1.0f,0.0f);
				uvs[i++]=new Vector2(0.0f,1.0f);
				uvs[i++]=new Vector2(1.0f,1.0f);
			}
			//
			Color[] colors=new Color[numVertices];
			i=numVertices;
			while(i-->0) {
				colors[i]=color;
			}
			//
			int[] triangles=new int[numQuads*6];
			for(i=0,j=0;i<numQuads;++i) {
				triangles[j++]=4*i+0;
				triangles[j++]=4*i+2;
				triangles[j++]=4*i+1;

				triangles[j++]=4*i+1;
				triangles[j++]=4*i+2;
				triangles[j++]=4*i+3;
			}
			//
			mesh.vertices=vertices;
			mesh.uv=uvs;
			mesh.colors=colors;
			mesh.triangles=triangles;

			//mesh.RecalculateNormals();
			//mesh.RecalculateBounds();
		}
		//
		return mesh;
	}

	public static Mesh DrawPlane(Mesh mesh,Vector3[] corners,Color color) {
		if(mesh==null) {
			mesh=new Mesh();
		}
		//
		var triangles = new int[(corners.Length-2)*3];
		for(int i=1,imax=corners.Length-1,j=0;i<imax;++i) {
			triangles[j++]=0;
			triangles[j++]=i;
			triangles[j++]=i+1;
		}

		var uv = new Vector2[corners.Length];
		for(int i=0,imax=corners.Length;i<imax;++i) {
			uv[i]=corners[i];
		}

		var colors = new Color[corners.Length];
		for(int i=0,imax=corners.Length;i<imax;++i) {
			colors[i]=color;
		}

		mesh.vertices=corners;
		mesh.uv=uv;
		mesh.colors=colors;
		mesh.triangles=triangles;

		//mesh.RecalculateNormals();
		//mesh.RecalculateBounds();
		return mesh;
	}

	#endregion Static

	#region Fields

	[Header("Ground")]
	public Style groundStyle=Style.B;
	public float borderThickness=.5f;
	public Material groundMaterial;
	public Color groundColor=Color.white;

	[Header("Wall")]
	public Style wallStyle=Style.B;
	public float thickness=.1f;
	public float cellSize=1.0f;
	public float emptySize=.15f;

	public Material wallMaterial;
	public Color wallColor=Color.white;

	[Header("Plane")]
	public Style planeStyle=Style.B;

	public Material planeMaterial;
	public Color planeColor=Color.white;

	public float height=10.0f;

	[Header("Data")]
	[Range(-1,1)]public int handedness=1;
	public Vector3[] corners;
	public Vector3 cameraPos;
	public Vector2 cameraFovY;

	[System.NonSerialized]protected GameObject m_GroundRoot;
	[System.NonSerialized]protected GameObject m_WallRoot;
	[System.NonSerialized]protected GameObject[] m_Walls;
	[System.NonSerialized]protected GameObject m_PlaneRoot;
	[System.NonSerialized]protected GameObject[] m_Planes;
	[System.NonSerialized]protected Vector3[][] m_PlaneCorners=new Vector3[2][];

	#endregion Fields

	#region Unity Messages

	protected virtual void Start() {
		Transform p=transform;
		Transform t;
		// Create ground.
		if(groundMaterial==null) {
			groundMaterial=new Material(Shader.Find("Sprites/Default"));
		}
		m_GroundRoot=CreateRenderedObject("_Ground",new Mesh(),groundMaterial);
		t=m_GroundRoot.transform;
		SetParent(t,p);
		// Create walls.
		if(wallMaterial==null) {
			wallMaterial=new Material(Shader.Find("Sprites/Default"));
		}
		m_WallRoot=new GameObject("_Walls");
		t=m_WallRoot.transform;
		SetParent(t,p);
		p=t;
		  //
		int i,imax=corners.Length;
		m_Walls=new GameObject[imax];
		for(i=0;i<imax;++i) {
			m_Walls[i]=CreateRenderedObject(string.Format("Wall ({0})",i),new Mesh(),wallMaterial);
			t=m_Walls[i].transform;
			SetParent(t,p);
		}
		// Create planes.
		p=transform;
		if(planeMaterial==null) {
			planeMaterial=new Material(Shader.Find("Sprites/Default"));
		}
		m_PlaneRoot=new GameObject("_Planes");
		t=m_PlaneRoot.transform;
		SetParent(t,p);
		p=t;
		  //
		imax=2;
		m_Planes=new GameObject[imax];
		for(i=0;i<imax;++i) {
			m_Planes[i]=CreateRenderedObject(string.Format("Plane ({0})",i),new Mesh(),planeMaterial);
			t=m_Planes[i].transform;
			SetParent(t,p);

			m_PlaneCorners[i]=(Vector3[])corners.Clone();
		}
		//
		BuildMesh();
	}

	#endregion Unity Messages

	#region Methods

	public virtual void BuildMesh() {
		MeshFilter filter;
		if(m_GroundRoot!=null) {
			filter=m_GroundRoot.GetComponent<MeshFilter>();
			switch(groundStyle){
				case Style.A:
					DrawGround(filter.sharedMesh,corners,borderThickness,groundColor,new Color(groundColor.r,groundColor.g,groundColor.b,0.0f));
				break;
				case Style.B:
					DrawGround(filter.sharedMesh,corners,thickness,groundColor,groundColor);
				break;
			}
		}
		if(m_WallRoot!=null) {
			int current=0,next;
			for(int i=0,imax=corners.Length;i<imax;++i,current+=handedness) {
				current=(current+imax)%imax;
				next=(current+handedness+imax)%imax;
				//
				Vector3 fwd=corners[next]-corners[current];
				filter=m_Walls[i].GetComponent<MeshFilter>();
				switch(wallStyle){
					case Style.A:
						DrawWall(
							filter.sharedMesh,wallColor,thickness,cellSize,emptySize,
							corners[current],Quaternion.LookRotation(fwd)*Quaternion.Euler(0,-90,0),fwd.magnitude,height
						);
					break;
					case Style.B:
						DrawWall_StyleB(
							filter.sharedMesh,wallColor,thickness,cellSize,emptySize,
							corners[current],Quaternion.LookRotation(fwd)*Quaternion.Euler(0,-90,0),fwd.magnitude,height
						);
					break;
				}
			}
		}
		if(m_PlaneRoot!=null) {
			for(int i=0,imax=m_Planes.Length,j;i<imax;++i) {
				float tan=Mathf.Tan(cameraFovY[i]*Mathf.Deg2Rad);
				j=corners.Length;while(j-->0) {
					m_PlaneCorners[i][j].Set(
						corners[j].x,
						cameraPos.y+(corners[j]-cameraPos).z*tan,
						corners[j].z
					);
				}
				//
				filter=m_Planes[i].GetComponent<MeshFilter>();
				switch(planeStyle){
					default:
						DrawPlane(
							filter.sharedMesh,m_PlaneCorners[i],planeColor
						);
					break;
				}
			}
		}
	}

	#endregion Methods

	#region Properties

	public virtual float groundAlpha {
		get {
			if(groundMaterial!=null) {
				return groundMaterial.color.a;
			}
			return 0.0f;
		}
		set {
			value=Mathf.Clamp01(value);
			if(groundMaterial!=null) {
				Color color=groundMaterial.color;
				color.a=value;
				groundMaterial.color=color;
			}
			if(m_GroundRoot!=null) {
				m_GroundRoot.SetActive(value>0.0f);
			}
		}
	}

	public virtual float wallAlpha {
		get {
			if(wallMaterial!=null) {
				return wallMaterial.color.a;
			}
			return 0.0f;
		}
		set {
			value=Mathf.Clamp01(value);
			if(wallMaterial!=null) {
				Color color=wallMaterial.color;
				color.a=value;
				wallMaterial.color=color;
			}
			if(m_WallRoot!=null) {
				m_WallRoot.SetActive(value>0.0f);
			}
		}
	}

	public virtual float planeAlpha {
		get {
			if(planeMaterial!=null) {
				return planeMaterial.color.a;
			}
			return 0.0f;
		}
		set {
			value=Mathf.Clamp01(value);
			if(planeMaterial!=null) {
				Color color=planeMaterial.color;
				color.a=value;
				planeMaterial.color=color;
			}
			if(m_PlaneRoot!=null) {
				m_PlaneRoot.SetActive(value>0.0f);
			}
		}
	}

	#endregion Properties

}