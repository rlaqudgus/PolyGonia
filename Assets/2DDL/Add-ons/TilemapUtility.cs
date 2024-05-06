namespace DynamicLight2D
{
	using UnityEngine;
	using System.Collections;
	using UnityEngine.Tilemaps;

#if UNITY_EDITOR
	using UnityEditor;
	[ExecuteInEditMode][SelectionBase]
#endif
	//

	public class TilemapUtility : AddOnBase {

		// Tags array is used for search results in inspector
		public static string []tags = {"tilemap", "block", "pixel", "art"};
		
		// Brief description of behavior in this Add-on
		public static string description = "Add support to tilemap collider";

		Tilemap[] _allTilemaps;


		#if UNITY_EDITOR
		// Use this for initialization
		public override void Start () {
			if (Application.isPlaying)
				return;
			
			print ("here");
			createCollisions (); //
		}
		#endif

		void createCollisions(){
			_allTilemaps = FindObjectsOfType (typeof(Tilemap)) as Tilemap[];

			for (int i = 0; i < _allTilemaps.Length; i++) {
				if (null == _allTilemaps [i].gameObject.GetComponent<TilemapCollisionGenerator> ()) {
					_allTilemaps [i].gameObject.AddComponent<TilemapCollisionGenerator> ();
				} else {
					_allTilemaps [i].gameObject.GetComponent<TilemapCollisionGenerator> ().GenerateTilemapCollision ();
				}
			}
		}

		void OnDestroy () {
			if (_allTilemaps != null) {
				for (int i = 0; i < _allTilemaps.Length; i++) {
					if (null != _allTilemaps [i]) {
						if (null != _allTilemaps [i].gameObject.GetComponent<TilemapCollisionGenerator> ()) {
							GameObject.DestroyImmediate (_allTilemaps [i].gameObject.GetComponent<TilemapCollisionGenerator> ());
						}
					}
				}
			}
		}
	}
	
}