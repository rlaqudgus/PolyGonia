namespace DynamicLight2D
{
	using UnityEditor;
	using UnityEngine;

	[CustomEditor(typeof(TilemapCollisionGenerator))]//Custom inspector of TileMapCollisionGenerator script
	public class TileMapCollisionGenerator_Editor : Editor
	{
	    public override void OnInspectorGUI()
	    {
	        base.OnInspectorGUI();//Base GUI
	        #region Generate collisions button
	        GUILayout.BeginHorizontal();
	        if (GUILayout.Button("Generate collisions"))//Generate collisions button
	        {
				TilemapCollisionGenerator Script = target as TilemapCollisionGenerator;
				Script.DestroyTilemapCollision(Script.Output);
	            Script.GenerateTilemapCollision(Script.Tilemap, Script.Output);
	        }
	        if (GUILayout.Button("Destroy collisions"))//Generate collisions button
	        {
				TilemapCollisionGenerator Script = target as TilemapCollisionGenerator;
	            Script.DestroyTilemapCollision(Script.Output);
	        }

			if (GUILayout.Button("Reset"))//Generate collisions button
			{
				TilemapCollisionGenerator Script = target as TilemapCollisionGenerator;
				Script.AdditionalSize *= 0f;
				Script.ColliderOffset *= 0f;
				Script.DestroyTilemapCollision(Script.Output);
				Script.GenerateTilemapCollision(Script.Tilemap, Script.Output);

			}
	        GUILayout.EndHorizontal();
	        #endregion
	    }
	}
}

