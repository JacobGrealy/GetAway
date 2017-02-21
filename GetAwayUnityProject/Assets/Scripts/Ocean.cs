using UnityEngine;
using System.Collections;

public class Ocean : MonoBehaviour 
{
	public Camera farCamera; //set the far camera to determine how large ocean should be

	public Material m_oceanMat;
	public Material m_wireframeMat;
	public int m_ansio = 2; //Ansiotrophic filtering on wave textures
	public float m_lodFadeDist = 2000.0f; //The distance that mipmap level on wave textures fades to highest mipmap. A neg number will disable this
	public int m_resolution = 128; //The resolution of the grid used for the ocean
	public bool m_useMaxResolution = false; //If enable this will over ride the resolution setting and will use the largest mesh possible in Unity
	public float m_bias = 2.0f; //A higher number will push more of the mesh verts closer to center of grid were player is. Must be >= 1
	public bool m_staged = false; //If enabled will spread the fourier transform over a few frame. This gives better frame rate but there maybe a jittery look to waves
	public int m_fourierGridSize = 64; //Fourier grid size. This version struggles with a number greater than 64. Must be pow2 number
	
	//These setting can be used to control the look of the waves from rough seas to calm lakes.
	//WARNING - not all combinations of numbers makes sense and the waves will not always look correct.
	//This will be amplified by the fact that the heights are stored in a 8 bit texture and heghts out side of -1 and 1 will be clamped
	public float m_windSpeed = 8.0f; //A higher wind speed gies greater swell to the waves
	public float m_waveAmp = 1.0f; //Scales the height of the waves
	public float m_inverseWaveAge = 0.84f; //A lower number means the waves last longer and will build up larger waves
	public Vector4 m_gridSizes = new Vector4(5488, 392, 28, 2);
	public GameObject m_sun;
	
	GameObject m_grid;
	GameObject m_gridWireframe;
	Texture2D m_fresnelLookUp;
	bool m_lastStaged;
	
	WaveSpectrumCPU m_waves;
	
	public float SampleHeight(Vector3 worldPos) { return m_waves.SampleHeight(worldPos, m_staged); }
	
	Mesh CreateRadialGrid(int segementsX, int segementsY)
	{
	
		Vector3[] vertices = new Vector3[segementsX*segementsY];
		Vector3[] normals = new Vector3[segementsX*segementsY];
		Vector2[] texcoords = new Vector2[segementsX*segementsY]; // not used atm
		
		float TAU = Mathf.PI*2.0f;
		float r;
		for(int x = 0; x < segementsX; x++)
		{
			for(int y = 0; y < segementsY; y++)
			{
				r = (float)x / (float)(segementsX-1);
				r = Mathf.Pow(r, m_bias);
				
				normals[x + y*segementsX] = new Vector3(0,1,0);

				vertices[x + y*segementsX].x = r * Mathf.Cos( TAU*(float)y / (float)(segementsY-1) ) ;
				vertices[x + y*segementsX].y = 0.0f;
				vertices[x + y*segementsX].z = r * Mathf.Sin( TAU*(float)y / (float)(segementsY-1) ) ;
			}
		}
	
		int[] indices = new int[segementsX*segementsY*6];
	
		int num = 0;
		for(int x = 0; x < segementsX-1; x++)
		{
			for(int y = 0; y < segementsY-1; y++)
			{
				indices[num++] = x + y * segementsX;
				indices[num++] = x + (y+1) * segementsX;
				indices[num++] = (x+1) + y * segementsX;
	
				indices[num++] = x + (y+1) * segementsX;
				indices[num++] = (x+1) + (y+1) * segementsX;
				indices[num++] = (x+1) + y * segementsX;
	
			}
		}
		
		Mesh mesh = new Mesh();
	
		mesh.vertices = vertices;
		mesh.uv = texcoords;
		mesh.normals = normals;
		mesh.triangles = indices;
		
		return mesh;
		
	}
	
	void CreateFresnelLookUp()
	{
		float nSnell = 1.34f; //Refractive index of water
	
		m_fresnelLookUp = new Texture2D(512, 1, TextureFormat.Alpha8, false);
		m_fresnelLookUp.filterMode = FilterMode.Bilinear;
		m_fresnelLookUp.wrapMode = TextureWrapMode.Clamp;
		m_fresnelLookUp.anisoLevel = 0;
		
		for(int x = 0; x < 512; x++)
		{
			float fresnel = 0.0f;
			float costhetai = (float)x/511.0f;
			float thetai = Mathf.Acos(costhetai);
			float sinthetat = Mathf.Sin(thetai)/nSnell;
			float thetat = Mathf.Asin(sinthetat);
			
			if(thetai == 0.0f)
			{
				fresnel = (nSnell - 1.0f)/(nSnell + 1.0f);
				fresnel = fresnel * fresnel;
			}
			else
			{
				float fs = Mathf.Sin(thetat - thetai) / Mathf.Sin(thetat + thetai);
				float ts = Mathf.Tan(thetat - thetai) / Mathf.Tan(thetat + thetai);
				fresnel = 0.5f * ( fs*fs + ts*ts );
			}
			
			m_fresnelLookUp.SetPixel(x, 0, new Color(fresnel,fresnel,fresnel,fresnel));
		}
		
		m_fresnelLookUp.Apply();
		
	}

	void Start () 
	{
		m_lastStaged = m_staged;
		
		m_waves = new WaveSpectrumCPU(m_fourierGridSize, m_windSpeed, m_waveAmp, m_inverseWaveAge, m_ansio, m_gridSizes);
		m_waves.SimulateWaves(Time.realtimeSinceStartup);
		
		CreateFresnelLookUp();
		
		if(m_resolution*m_resolution >= 65000 || m_useMaxResolution)
		{
			m_resolution = (int)Mathf.Sqrt(65000);
			
			if(!m_useMaxResolution) 
				Debug.Log("Ocean::Start - Grid resolution set to high. Setting resolution to the maxium allowed(" + m_resolution.ToString() + ")" );
		}
		
		if(m_bias < 1.0f)
		{
			m_bias = 1.0f;
			Debug.Log("Ocean::Start - bias must not be less than 1, changing to 1");
		}
		
		Mesh mesh = CreateRadialGrid(m_resolution, m_resolution);
		
		float far = farCamera.farClipPlane;
		
		m_grid = new GameObject("Ocean Grid");
		m_grid.AddComponent<MeshFilter>();
		m_grid.AddComponent<MeshRenderer>();
		m_grid.renderer.material = m_oceanMat;
		m_grid.GetComponent<MeshFilter>().mesh = mesh;
		m_grid.transform.localScale = new Vector3(far,1,far); //Make radial grid have a radius equal to far plane
	
		m_gridWireframe = new GameObject("Ocean Grid Wireframe");
		m_gridWireframe.AddComponent<MeshFilter>();
		m_gridWireframe.AddComponent<MeshRenderer>();
		m_gridWireframe.renderer.material = m_wireframeMat;
		m_gridWireframe.GetComponent<MeshFilter>().mesh = mesh;
		m_gridWireframe.transform.localScale = new Vector3(far,1,far);
		m_gridWireframe.layer = 8;
		
		m_oceanMat.SetTexture("_FresnelLookUp", m_fresnelLookUp);
		m_oceanMat.SetVector("_GridSizes", m_waves.GetGridSizes());
		m_oceanMat.SetFloat("_MaxLod", m_waves.GetMipMapLevels());
		
		m_wireframeMat.SetVector("_GridSizes", m_waves.GetGridSizes());
		m_wireframeMat.SetFloat("_MaxLod", m_waves.GetMipMapLevels());
	}
	
	void Update () 
	{
		//If the option to uses staging is enable when playing the current stage in the WavesSpectrumCPU object needs to be reset
		if(m_lastStaged != m_staged) m_waves.ResetStage();
		
		if(m_staged)
			m_waves.SimulateWavesStaged(Time.realtimeSinceStartup);
		else
			m_waves.SimulateWaves(Time.realtimeSinceStartup);

		//Update shader values that may change every frame
		m_oceanMat.SetTexture("_Map0", m_waves.GetMap0());
		m_oceanMat.SetTexture("_Map1", m_waves.GetMap1());
		m_oceanMat.SetTexture("_Map2", m_waves.GetMap2());
		m_oceanMat.SetVector("_SunDir", m_sun.transform.forward*-1.0f);
		m_oceanMat.SetVector("_SunColor", m_sun.GetComponent<Light>().light.color);
		m_oceanMat.SetFloat("_LodFadeDist", m_lodFadeDist);
		
		m_wireframeMat.SetTexture("_Map0", m_waves.GetMap0());
		m_wireframeMat.SetFloat("_LodFadeDist", m_lodFadeDist);
		
		//This makes sure the grid is always centered were the player is
		Vector3 pos = Camera.main.transform.position;
		pos.y = 0.0f;
		
		m_grid.transform.localPosition = pos;
		m_gridWireframe.transform.localPosition = pos;
		m_grid.layer = 4; //set layer to water
		m_grid.renderer.receiveShadows = false;
		m_grid.renderer.castShadows = false;
	}
	
}
