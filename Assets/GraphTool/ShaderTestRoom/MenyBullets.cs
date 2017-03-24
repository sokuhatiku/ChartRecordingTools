using UnityEngine;
using System.Runtime.InteropServices;

namespace GraphTool
{
	/// <summary>
	/// 弾の構造体
	/// </summary>
	struct Bullet
	{
		/// <summary>
		/// 座標
		/// </summary>
		public Vector3 pos;

		/// <summary>
		/// 速度
		/// </summary>
		public Vector3 accel;

		/// <summary>
		/// 色
		/// </summary>
		public Color color;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Bullet(Vector3 pos, Vector3 accel, Color color)
		{
			this.pos = pos;
			this.accel = accel;
			this.color = color;
		}
	}

	/// <summary>
	/// 沢山の弾を管理するクラス
	/// </summary>
	public class MenyBullets : GraphPartsBase
	{


		public Shader bulletsShader;
		public Texture bulletsTexture;
		public ComputeShader bulletsComputeShader;

		Material bulletsMaterial;
		ComputeBuffer bulletsBuffer;

		void InitializeComputeBuffer()
		{
			bulletsBuffer = new ComputeBuffer(10000, Marshal.SizeOf(typeof(Bullet)));

			Bullet[] bullets = new Bullet[bulletsBuffer.count];
			for (int i = 0; i < bulletsBuffer.count; i++)
			{
				bullets[i] =
					new Bullet(
						new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f)),
						new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * 0.5f,
						new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)));
			}

			bulletsBuffer.SetData(bullets);
		}

		protected override void OnEnable()
		{
			bulletsMaterial = new Material(bulletsShader);
			InitializeComputeBuffer();
		}

		protected override void OnDisable()
		{
			bulletsBuffer.Release();
		}

		void Update()
		{
			bulletsComputeShader.SetBuffer(0, "Bullets", bulletsBuffer);
			bulletsComputeShader.SetFloat("DeltaTime", Time.deltaTime);
			bulletsComputeShader.Dispatch(0, bulletsBuffer.count / 8 + 1, 1, 1);
		}

		void OnRenderObject()
		{
			
			bulletsMaterial.SetTexture("_MainTex", bulletsTexture);
			bulletsMaterial.SetBuffer("Bullets", bulletsBuffer);

			bulletsMaterial.SetPass(0);

			
			Graphics.DrawProcedural(MeshTopology.Points, bulletsBuffer.count);
		}

	}
}