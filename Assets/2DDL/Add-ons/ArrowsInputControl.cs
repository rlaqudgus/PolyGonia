namespace DynamicLight2D
{
	using UnityEngine;
	using System.Collections;
	using System;

#if UNITY_EDITOR
	using UnityEditor;
#endif

	[ExecuteInEditMode]
	public class ArrowsInputControl : AddOnBase {
		// Need inherit from AddOnBase if you need use 
		// 2DDL instance of current Light2D

		// Tags array is used for search results in inspector
		public static string []tags = {"input", "keys", "Axis",  "player", "move", "arrows"};

		// Brief description of behavior in this Add-on
		public static string description = "Move the 2DDL Object with input axis";


		[TitleAttribute("Velocity of rotation in degrees by frame")]
		[SerializeField] [Range(10f,100f)]float magnitude = 30f;


		[Space(20)]


		private Vector3 pos;

		

		public override void Start () {
			base.Start();
		}


		
		public override void Update () {

			base.Update();

			if(!Application.isPlaying)
				return;


				//pos = dynamicLightInstance.gameObject.transform.position;

				float h = Input.GetAxis("Horizontal") * magnitude;
				float v = Input.GetAxis("Vertical") * magnitude;

			transform.Translate(h*Time.deltaTime, v*Time.deltaTime, 0);
			//dynamicLightInstance.gameObject.transform

		}


	}
}