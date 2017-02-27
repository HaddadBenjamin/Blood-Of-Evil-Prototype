using NGTools.NGFav;
using System;
using UnityEngine;

namespace NGTools.Tests
{
	public class SimpleLog : MonoBehaviour, ICustomFavorite
	{
		[Serializable]
		public class AuxiliaryClass
		{
			public int		doubleNestedFieldInteger;
			public int[]	doubleNestedArrayFieldInteger;
		}

		[Serializable]
		public class RefClass
		{
			public int				nestedFieldInteger;
			public int[]			nestedArrayFieldInteger;
			public AuxiliaryClass	nestedComplex;
		}

		//public AudioSource		audioSource1;
		//public AudioSource audioSource2;
		public UnityEngine.Object		anyObject;
		//public UnityEngine.Object		all2;
		public AudioSource	source1;
		public AudioSource	source2;
		public AudioSource	source3;
		public SimpleLog	ref1;
		public Component	component;
		//public Component	componentRef2;
		//public MonoBehaviour	MonoBomponentRef1;
		//public MonoBehaviour	MonoBomponentRef2;
		public ICustomFavorite	interface1;
		public ICustomFavorite	interface2;
		//public GameObject	gameObject;
		public GameObject   unityObject;
		//public Texture2D	texture;
		//public Sprite		sprite;
		//public Camera		camcam;
		//public AudioListener AudioListener;
		//public Projector	pro;
		//public TrailRenderer	tr;
		//public Shader		shaderr;
		//public Material		mat1;
		//public TextAsset	textAsset;
		//public Material		mat2;
		//public SimpleLog	log;
		//public SimpleLog[]	arrayLog;

		//public UnityEngine.Object[]	arrayObjects;

		//public Vector2		vector2;
		//public Vector3		vector3;
		//public Vector4		vector4;
		//public Quaternion	quat;
		//public Color		color;
		//public Rect			rect;

		//public Material		material { get; set; }
		//public Material		sharedMaterial { get; set; }

		//public string		stringer;
		//public int			fieldInteger;
		//public int[]		arrayFieldInteger;
		//public RefClass		refClass;
		//public RefClass[]	arrayRefClass;

		public void	Test()
		{
			Debug.Log("test");
		}

		float	OnHierarchyGUI(Rect r)
		{
			r.xMin = r.xMax - 20F;
			GUI.Button(r, "b");
			return r.xMin;
		}

		public void GetFavorite(out int identifier, out Func<int, GameObject> resolver)
		{
			identifier = 20;
			//resolver = a;
			resolver = (int id ) => { return Camera.main != null ? Camera.main.gameObject : null; };
		}

		private GameObject	a(int id)
		{
			return Camera.main.gameObject;
		}
	}
}