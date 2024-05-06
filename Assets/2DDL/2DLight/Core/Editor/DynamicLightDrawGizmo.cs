namespace DynamicLight2D
{
	using UnityEngine;
	using UnityEditor;
	using System.Collections;
	
	public class DynamicLightDrawGizmo{

		[DrawGizmo(GizmoType.NonSelected | GizmoType.NotInSelectionHierarchy)]
		private static void drawGizmoNow(DynamicLight dl, GizmoType gizmoType)
		{

			PostProcessMethods.checkIconTexture();

			PostProcessMethods.SyncWithPhysics2D (); // check auto sync field from Physics2D property

			Gizmos.DrawIcon(dl.transform.position, "logo2DDL_gizmos.png", false);

		}

	}
		
}
