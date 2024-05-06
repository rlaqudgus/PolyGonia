﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

//[HelpURL("sdfsdf.com")]

public class DynamicLightSetting : ScriptableObject
{

	//[Header("Settings")]

	[Header("Package Info")]
	// Package related
	public string version = "1.3.6r1";
	public string dateReleased = System.DateTime.Now.ToShortDateString();


	[Header("LayerMask of 2DLight Object by Default")]
	public LayerMask layerMask;

	[Header("Layer of Caster by Default")]
	public int layerCaster = 0;// = TagLayerClass.getLayerNumberFromLayerMask(layer);// LayerMask.GetMask("Default");

	[Header("Automatic Layer creation")]
	public bool LayerCreationHasBeenPerformed = false;


	void OnEnable()
		{
			if(layerMask == 0)
				layerMask = LayerMask.GetMask("Default");
		}

}
#endif