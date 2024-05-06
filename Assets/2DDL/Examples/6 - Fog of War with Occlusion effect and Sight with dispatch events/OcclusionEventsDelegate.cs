namespace DynamicLight2D
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections;
	
	public class OcclusionEventsDelegate : MonoBehaviour {

		[SerializeField] DynamicLight SightOfWar;
		[SerializeField] Text status;

		string text;

		void Start(){
			//subscribe on start
			SightOfWar.InsideFieldOfViewEvent += OcclusionEventsDelegate_OnInside;
			text = "Boxes Revealed: ";
		}

		public void OcclusionEventsDelegate_OnInside(GameObject []go, DynamicLight light){
			text = "Boxes Revealed: ";
			for (int i = 0; i < go.Length; i++) {
				text = System.String.Join(" ", new string[]{text, go[i].name});
				//text += go[i].name + " ";
			}

			status.text = text;
		}

	}
}

