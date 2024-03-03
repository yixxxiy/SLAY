using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// ReSharper disable once CheckNamespace
namespace UiParticles
{
	/// <summary>
	/// Ui Parcticles, requiere ParticleSystem component
	/// </summary>
	
	[RequireComponent(typeof(ParticleSystem))]
	public class UiParticles : MaskableGraphic
	{

		#region InspectorFields

		/// <summary>
		/// ParticleSystem used for generate particles
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("m_ParticleSystem")]
		private ParticleSystem m_ParticleSystem;

		/// <summary>
		/// If true, particles renders in streched mode
		/// </summary>
		[FormerlySerializedAs("m_RenderMode")]
		[SerializeField]
		[Tooltip("Render mode of particles")]
		private UiParticleRenderMode m_RenderMode = UiParticleRenderMode.Billboard;

		/// <summary>
		/// Scale particle size, depends on particle velocity
		/// </summary>
		[FormerlySerializedAs("m_StretchedSpeedScale")]
		[SerializeField]
		[Tooltip("Speed Scale for streched billboards")]
		private float m_StretchedSpeedScale = 1f;

		/// <summary>
		/// Sclae particle length in streched mode
		/// </summary>
		[FormerlySerializedAs("m_StretchedLenghScale")]
		[SerializeField]
		[Tooltip("Speed Scale for streched billboards")]
		private float m_StretchedLenghScale = 1f;

		[SerializeField]
		[Tooltip("Which component affect the stretched scale")]
		private Vector3 m_StretchedAffectComonent = Vector3.up;

		[SerializeField]
		[Tooltip("You may want stretch tex with different direction")]
		private float m_StretchedAngleOffset;

		[FormerlySerializedAs("m_IgnoreTimescale")]
		[SerializeField]
		[Tooltip("If true, particles ignore timescale")]
		private bool m_IgnoreTimescale = false;

		[SerializeField]
		private Mesh m_RenderedMesh;

		[SerializeField]
		private bool m_isMatUseCustomDataMode = false;

		#endregion


		#region Public properties
		/// <summary>
		/// ParticleSystem used for generate particles
		/// </summary>
		/// <value>The particle system.</value>
		public ParticleSystem ParticleSystem {
			get { return m_ParticleSystem; }
			set {
				if (SetPropertyUtility.SetClass(ref m_ParticleSystem, value))
					SetAllDirty();
			}
		}

		/// <summary>
		/// Texture used by the particles
		/// </summary>
		public override Texture mainTexture {
			get {
				if (material != null)
				{
					if (m_MainTexProperty == -1)
					{
						if (material.HasProperty(MainTexPropertyName))
							m_MainTexProperty = Shader.PropertyToID(MainTexPropertyName);
						else if (material.HasProperty(BaseMapPropertyName))
							m_MainTexProperty = Shader.PropertyToID(BaseMapPropertyName);
					}

					return material.GetTexture(m_MainTexProperty);
				}

				return s_WhiteTexture;
			}
		}

		/// <summary>
		/// Particle system render mode (billboard, strechedBillobard)
		/// </summary>
		public UiParticleRenderMode RenderMode {
			get { return m_RenderMode; }
			set {
				if (SetPropertyUtility.SetStruct(ref m_RenderMode, value))
					SetAllDirty();
			}
		}

		public Mesh RenderedMesh {
			get { return m_RenderedMesh; }
			set {
				if (SetPropertyUtility.SetClass(ref m_RenderedMesh, value))
				{
					InitMeshData();
					SetAllDirty();
				}
			}
		}

		public float StretchedSpeedScale {
			get => m_StretchedSpeedScale;
			set {
				m_StretchedSpeedScale = value;
			}
		}
		public float StretchedLenghScale {
			get => m_StretchedLenghScale;
			set {
				m_StretchedLenghScale = value;
			}
		}
		public Vector3 StretchedAffectComonent {
			get => m_StretchedAffectComonent;
			set {
				m_StretchedAffectComonent = value;
			}
		}

		public bool IsMatUseCustomDataMode {
			get => m_isMatUseCustomDataMode;
			set {
				m_isMatUseCustomDataMode = value;
			}
		}

		#endregion

		private const string MainTexPropertyName = "_MainTex";
		private const string BaseMapPropertyName = "_BaseMap";
		private int m_MainTexProperty = -1;

		private ParticleSystemRenderer m_ParticleSystemRenderer;
		private ParticleSystem.Particle[] m_Particles;

		private Mesh _cachedMesh;
		private Vector3[] m_MeshVerts;
		private int[] m_MeshTriangles;
		private Vector2[] m_MeshUvs;
		private Color32[] m_MeshCols;

		private Vector4 v4Zero = Vector4.zero;


		protected override void Awake()
		{
			var particleSystem = GetComponent<ParticleSystem>();
			var particleSystemRenderer = GetComponent<ParticleSystemRenderer>();
			if (m_Material == null) {
				m_Material = particleSystemRenderer.sharedMaterial;
			}
			if(m_Material != null && m_Material.shader != null) {
				m_Material.shader = Shader.Find(m_Material.shader.name);
			}
			// if (m_Material) { 
			// 	isMatUseCustomDataMode = 0 == m_Material.GetFloat(pModeId);
			// }
			if (particleSystemRenderer.renderMode == ParticleSystemRenderMode.Stretch)
				RenderMode = UiParticleRenderMode.StreachedBillboard;

			base.Awake();
			ParticleSystem = particleSystem;
			m_ParticleSystemRenderer = particleSystemRenderer;
			InitMeshData();
		}

		private void InitMeshData()
		{
			if (RenderedMesh != null && RenderedMesh != _cachedMesh)
			{
				m_MeshVerts = RenderedMesh.vertices;
				m_MeshTriangles = RenderedMesh.triangles;
				m_MeshUvs = RenderedMesh.uv;
				m_MeshCols = RenderedMesh.colors32;
				_cachedMesh = RenderedMesh;
			}
		}


		public override void SetMaterialDirty()
		{
			base.SetMaterialDirty();
			if (m_ParticleSystemRenderer != null)
				m_ParticleSystemRenderer.sharedMaterial = m_Material;
		}

		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			if (ParticleSystem == null) {
				base.OnPopulateMesh(toFill);
				return;
			}
			GenerateParticlesBillboards(toFill);
		}

		protected virtual void Update()
		{
			if (!m_IgnoreTimescale)
			{
				if (ParticleSystem != null && ParticleSystem.isPlaying)
				{
					SetVerticesDirty();
				}
			}
			else
			{
				if (ParticleSystem != null)
				{
					ParticleSystem.Simulate(Time.unscaledDeltaTime, true, false);
					SetVerticesDirty();
				}
			}

			// disable default particle renderer, we using our custom
			if (m_ParticleSystemRenderer != null && m_ParticleSystemRenderer.enabled)
				m_ParticleSystemRenderer.enabled = false;
		}


		private void InitParticlesBuffer(ParticleSystem.MainModule mainModule)
		{
			if (m_Particles == null || m_Particles.Length < mainModule.maxParticles)
			{
				m_Particles = new ParticleSystem.Particle[mainModule.maxParticles];
			}

		}

		private void GenerateParticlesBillboards(VertexHelper vh)
		{
			//read modules ones, cause they produce allocations when read.
			var mainModule = ParticleSystem.main;

			var textureSheetAnimationModule = ParticleSystem.textureSheetAnimation;

			InitParticlesBuffer(mainModule);
			int numParticlesAlive = ParticleSystem.GetParticles(m_Particles);

			vh.Clear();

			//!NOTE sample curves before render particles, because it produces allocations
			var frameOverTime = ParticleSystem.textureSheetAnimation.frameOverTime;
			var velocityOverLifeTime = ParticleSystem.velocityOverLifetime;
			var velocityOverTimeX = velocityOverLifeTime.x;
			var velocityOverTimeY = velocityOverLifeTime.y;
			var velocityOverTimeZ = velocityOverLifeTime.z;
			var isWorldSimulationSpace = mainModule.simulationSpace == ParticleSystemSimulationSpace.World;

			if (RenderMode == UiParticleRenderMode.Mesh)
			{
				if (RenderedMesh != null)
				{
					List<UIVertex> verts = new List<UIVertex>();
					List<int> indexes = new List<int>();
					InitMeshData();
					for (int i = 0; i < numParticlesAlive; i++)
					{
						// DrawParticleMesh(m_Particles[i], vh, frameOverTime, isWorldSimulationSpace,
						// 	textureSheetAnimationModule, m_MeshVerts, m_MeshTriangles, m_MeshUvs);
						DrawParticleMesh(ref verts, ref indexes, m_Particles[i], frameOverTime, isWorldSimulationSpace,
							textureSheetAnimationModule, m_MeshVerts, m_MeshTriangles, m_MeshUvs, m_MeshCols);
					}
					vh.AddUIVertexStream(verts, indexes);
				}
			}
			else
			{
				List<UIVertex> verts = new List<UIVertex>();
				List<int> indexes = new List<int>();
				for (int i = 0; i < numParticlesAlive; i++)
				{
					// DrawParticleBillboard(ref verts, ref indexes, m_Particles[i], vh, frameOverTime,
					// 	velocityOverTimeX, velocityOverTimeY, velocityOverTimeZ, isWorldSimulationSpace,
					// 	textureSheetAnimationModule);
					DrawParticleBillboard(ref verts, ref indexes, m_Particles[i], frameOverTime,
						velocityOverTimeX, velocityOverTimeY, velocityOverTimeZ, isWorldSimulationSpace,
						textureSheetAnimationModule);
				}
				vh.AddUIVertexStream(verts, indexes);
			}
		}

		private void DrawParticleMesh(
			ref List<UIVertex> vertList,
			ref List<int> indexList,
			ParticleSystem.Particle particle,
			// VertexHelper vh,
			ParticleSystem.MinMaxCurve frameOverTime,
			bool isWorldSimulationSpace,
			ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule, Vector3[] verts, int[] triangles,
			Vector2[] uvs, Color32[] colors)
		{
			var center = particle.position;
			var rotation = Quaternion.Euler(particle.rotation3D);

			if (isWorldSimulationSpace)
			{
				center = rectTransform.InverseTransformPoint(center);
			}

			float timeAlive = particle.startLifetime - particle.remainingLifetime;

			Vector3 size3D = particle.GetCurrentSize3D(ParticleSystem);
			Color32 color32 = particle.GetCurrentColor(ParticleSystem);
			color32 = MultiplyColor32(color32, color);

			Vector2 uv0;
			Vector2 uv1;
			Vector2 uv2;
			Vector2 uv3;

			CalculateUvs(
				particle, frameOverTime, textureSheetAnimationModule, timeAlive, 
				out uv0, out uv1, out uv2, out uv3, m_ParticleSystemRenderer.flip,
				m_StretchedAngleOffset
			);
			Vector4 customData = GetCustomData(timeAlive);

			//   This "pivot" value is not [-1,1], is actually [-meshSize, meshSize].
			//   We should calculate the mesh size by ourself, and set the "pivot * meshSize" 
			// to "pivot" on particle system renderer.
			//   For sure, we can calculate the mesh size here, but the calculation need to loop 
			// every vertics, I'm afraid it could make low performance, so I choice the simple solution.
			var pivot = new Vector3(
					m_ParticleSystemRenderer.pivot.x * size3D.x,
					m_ParticleSystemRenderer.pivot.y * size3D.y,
					m_ParticleSystemRenderer.pivot.z * size3D.z
				);
			// var currentVertCount = vh.currentVertCount;
			var currentVertCount = vertList.Count;
			var hasCols = colors.Length > 0;
			for (int j = 0; j < verts.Length; j++)
			{
				Vector3 pos = verts[j];
				pos.x = pos.x * size3D.x + pivot.x;
				pos.y = pos.y * size3D.y + pivot.y;
				pos.z = pos.z * size3D.z + pivot.z;
				pos = rotation * pos + center;

				var uvXpercent = uvs[j].x;
				var uvYpercent = uvs[j].y;

				var newUvx = Mathf.Lerp(uv0.x, uv2.x, uvXpercent);
				var newUvy = Mathf.Lerp(uv0.y, uv2.y, uvYpercent);

				// vh.AddVert(pos, color32, new Vector2(newUvx, newUvy));
				var color = color32;
				if (hasCols) {
					color = MultiplyColor32(color32, colors[j]);
				}
				vertList.Add(CreateNewUIVert(pos, color, new Vector2(newUvx, newUvy), customData, m_isMatUseCustomDataMode));
			}

			for (int i = 0; i < triangles.Length; i += 3) {
				indexList.Add(currentVertCount + triangles[i]);
				indexList.Add(currentVertCount + triangles[i + 1]);
				indexList.Add(currentVertCount + triangles[i + 2]);
			}
		}

		private void DrawParticleBillboard(
			ref List<UIVertex> vertList,
			ref List<int> indexList,
			ParticleSystem.Particle particle,
			// VertexHelper vh,
			ParticleSystem.MinMaxCurve frameOverTime,
			ParticleSystem.MinMaxCurve velocityOverTimeX,
			ParticleSystem.MinMaxCurve velocityOverTimeY,
			ParticleSystem.MinMaxCurve velocityOverTimeZ,
			bool isWorldSimulationSpace,
			ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule)
		{
			var center = particle.position;
			var rotation = Quaternion.Euler(particle.rotation3D);

			if (isWorldSimulationSpace)
			{
				center = rectTransform.InverseTransformPoint(center);
			}

			float timeAlive = particle.startLifetime - particle.remainingLifetime;
			float globalTimeAlive = timeAlive / particle.startLifetime;

			Vector3 size3D = particle.GetCurrentSize3D(ParticleSystem);

			if (m_RenderMode == UiParticleRenderMode.StreachedBillboard)
			{
				GetStrechedBillboardsSizeAndRotation(particle, globalTimeAlive, ref size3D, out rotation,
					velocityOverTimeX, velocityOverTimeY, velocityOverTimeZ);
			}

			var pivot = new Vector3(
				m_ParticleSystemRenderer.pivot.x * size3D.x,
				m_ParticleSystemRenderer.pivot.y * size3D.y,
				m_ParticleSystemRenderer.pivot.z * size3D.z
				);
			var leftTop = new Vector3(-size3D.x * 0.5f, size3D.y * 0.5f) + pivot;
			var rightTop = new Vector3(size3D.x * 0.5f, size3D.y * 0.5f) + pivot;
			var rightBottom = new Vector3(size3D.x * 0.5f, -size3D.y * 0.5f) + pivot;
			var leftBottom = new Vector3(-size3D.x * 0.5f, -size3D.y * 0.5f) + pivot;


			leftTop = rotation * leftTop + center;
			rightTop = rotation * rightTop + center;
			rightBottom = rotation * rightBottom + center;
			leftBottom = rotation * leftBottom + center;

			Color32 color32 = particle.GetCurrentColor(ParticleSystem);
			// var i = vh.currentVertCount;
			var i = vertList.Count; 

			Vector2 uv0;
			Vector2 uv1;
			Vector2 uv2;
			Vector2 uv3;

			CalculateUvs(
				particle, frameOverTime, textureSheetAnimationModule, timeAlive, 
				out uv0, out uv1, out uv2, out uv3, m_ParticleSystemRenderer.flip,
				m_StretchedAngleOffset
			);
			Vector4 customData = GetCustomData(globalTimeAlive);

			vertList.Add(CreateNewUIVert(leftBottom,	color32, uv0, customData, m_isMatUseCustomDataMode));
			vertList.Add(CreateNewUIVert(leftTop,		color32, uv1, customData, m_isMatUseCustomDataMode));
			vertList.Add(CreateNewUIVert(rightTop,		color32, uv2, customData, m_isMatUseCustomDataMode));
			vertList.Add(CreateNewUIVert(rightBottom,	color32, uv3, customData, m_isMatUseCustomDataMode));

			indexList.Add(i);
			indexList.Add(i + 1);
			indexList.Add(i + 2);
			indexList.Add(i + 2);
			indexList.Add(i + 3);
			indexList.Add(i);
		}

		private UIVertex CreateNewUIVert(Vector3 pos_, Color32 col_, Vector2 uv0_, Vector4 customData_, bool hasCustomData_) {
			var vert = UIVertex.simpleVert;
			vert.position = pos_;
			vert.color = col_;
			vert.uv0 = uv0_;
			if (hasCustomData_) {
				vert.uv1 = new Vector2(customData_.x, customData_.y);
				vert.uv2 = new Vector2(customData_.z, customData_.w);
			}
			return vert;
		}
		private Vector4 GetCustomData(float globalTimeAlive_) {
			var customData = v4Zero;
			if (ParticleSystem.customData.enabled) {
				customData.Set(
					GetCustomDataComponent(ParticleSystem.customData, 0, globalTimeAlive_),
					GetCustomDataComponent(ParticleSystem.customData, 1, globalTimeAlive_),
					GetCustomDataComponent(ParticleSystem.customData, 2, globalTimeAlive_),
					GetCustomDataComponent(ParticleSystem.customData, 3, globalTimeAlive_)
				);
			}
			return customData;
		}
		private float GetCustomDataComponent(ParticleSystem.CustomDataModule customData_, int componentIdx_, float time_) {
			var cdCurve = customData_.GetVector(ParticleSystemCustomData.Custom1, componentIdx_);
			float crvValue = cdCurve.Evaluate(time_);
			return crvValue;
		}
		private Color32 MultiplyColor32(Color32 color0_, Color32 color1_) {
			var color = new Color32(
				(byte)(color0_.r / 255f * color1_.r / 255f * 255f),
				(byte)(color0_.g / 255f * color1_.g / 255f * 255f),
				(byte)(color0_.b / 255f * color1_.b / 255f * 255f),
				(byte)(color0_.a / 255f * color1_.a / 255f * 255f)
				);
			return color;
		}

		private static void CalculateUvs(ParticleSystem.Particle particle, ParticleSystem.MinMaxCurve frameOverTime,
			ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule, 
			float timeAlive, out Vector2 uv0, out Vector2 uv1, out Vector2 uv2,
			out Vector2 uv3, Vector3 flip, float uvRotAngle_)
		{
			if (!textureSheetAnimationModule.enabled)
			{
				uv0 = new Vector2(0f, 0f);
				uv1 = new Vector2(0f, 1f);
				uv2 = new Vector2(1f, 1f);
				uv3 = new Vector2(1f, 0f);
			}
			else
			{
				float lifeTimePerCycle = particle.startLifetime / textureSheetAnimationModule.cycleCount;
				float timePerCycle = timeAlive % lifeTimePerCycle;
				float timeAliveAnim01 = timePerCycle / lifeTimePerCycle; // in percents


				var totalFramesCount = textureSheetAnimationModule.numTilesY * textureSheetAnimationModule.numTilesX;
				var frame01 = frameOverTime.Evaluate(timeAliveAnim01);

				var frame = 0f;
				switch (textureSheetAnimationModule.animation)
				{
					case ParticleSystemAnimationType.WholeSheet:
					{
						frame = Mathf.Clamp(Mathf.Floor(frame01 * totalFramesCount), 0, totalFramesCount - 1);
						break;
					}
					case ParticleSystemAnimationType.SingleRow:
					{
						frame = Mathf.Clamp(Mathf.Floor(frame01 * textureSheetAnimationModule.numTilesX), 0,
							textureSheetAnimationModule.numTilesX - 1);
						int row = textureSheetAnimationModule.rowIndex;
						if (textureSheetAnimationModule.useRandomRow)
						{
							//Random.InitState((int) particle.randomSeed);
							row = Random.Range(0, textureSheetAnimationModule.numTilesY);
						}
						frame += row * textureSheetAnimationModule.numTilesX;
						break;
					}
				}

				int x = (int) frame % textureSheetAnimationModule.numTilesX;
				int y = (int) frame / textureSheetAnimationModule.numTilesX;

				var xDelta = 1f / textureSheetAnimationModule.numTilesX;
				var yDelta = 1f / textureSheetAnimationModule.numTilesY;
				y = textureSheetAnimationModule.numTilesY - 1 - y;
				var sX = x * xDelta;
				var sY = y * yDelta;
				var eX = sX + xDelta;
				var eY = sY + yDelta;

				uv0 = new Vector2(sX, sY);
				uv1 = new Vector2(sX, eY);
				uv2 = new Vector2(eX, eY);
				uv3 = new Vector2(eX, sY);
			}

			if (flip.x > 0) {
				var tmpUv = uv0; uv0 = uv3; uv3 = tmpUv;
				tmpUv = uv1; uv1 = uv2; uv2 = tmpUv;
			}
			if (flip.y > 0) {
				var tmpUv = uv0; uv0 = uv1; uv1 = tmpUv;
				tmpUv = uv2; uv2 = uv3; uv3 = tmpUv;
			}
			if (0 != uvRotAngle_) {
				float angle = uvRotAngle_ * Mathf.Deg2Rad;
				float cos = Mathf.Cos(angle);
				float sin = Mathf.Sin(angle);
				uv1.Set(-sin, cos);
				uv2.Set(cos - sin, sin + cos);
				uv3.Set(cos, sin);
			}
		}


		/// <summary>
		/// Evaluate size and roatation of particle in streched billboard mode
		/// </summary>
		/// <param name="particle">particle</param>
		/// <param name="timeAlive01">current life time percent [0,1] range</param>
		/// <param name="size3D">particle size</param>
		/// <param name="rotation">particle rotation</param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		private void GetStrechedBillboardsSizeAndRotation(ParticleSystem.Particle particle, float timeAlive01,
			ref Vector3 size3D, out Quaternion rotation, 
			ParticleSystem.MinMaxCurve x, ParticleSystem.MinMaxCurve y, ParticleSystem.MinMaxCurve z)
		{
			var velocityOverLifeTime = Vector3.zero;

			if (ParticleSystem.velocityOverLifetime.enabled)
			{
				velocityOverLifeTime.x = x.Evaluate(timeAlive01);
				velocityOverLifeTime.y = y.Evaluate(timeAlive01);
				velocityOverLifeTime.z = z.Evaluate(timeAlive01);
			}
		    
			var finalVelocity = particle.velocity + velocityOverLifeTime;
			var ang = Vector3.Angle(finalVelocity,  Vector3.up);
			var horizontalDirection = finalVelocity.x < 0 ? 1 : -1;
			rotation = Quaternion.Euler(new Vector3(0,0, ang*horizontalDirection));
			size3D.Set(
				size3D.x * Mathf.Lerp(1, m_StretchedLenghScale, m_StretchedAffectComonent.x),
				size3D.y * Mathf.Lerp(1, m_StretchedLenghScale, m_StretchedAffectComonent.y),
				size3D.z * Mathf.Lerp(1, m_StretchedLenghScale, m_StretchedAffectComonent.z)
				);
			size3D+= new Vector3(0, m_StretchedSpeedScale*finalVelocity.magnitude);
		}
	}


	/// <summary>
	/// Particles Render Modes
	/// </summary>
    public enum UiParticleRenderMode
    {
        Billboard,
        StreachedBillboard,
	    Mesh
    }
}
