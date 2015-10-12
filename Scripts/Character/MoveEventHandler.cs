﻿using UnityEngine;
using System.Collections;

public class MoveEventHandler : MonoBehaviour 
{


	//Private members
	private bool onNormalAlpha = false;
	private bool onSpecialAlpha = false;
	private bool onLightHit = false;
	private Animator anim;

	void Start () 
	{
		anim = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
	{	

	}
	
	//Events
	public void OnNormalAlphaStart()
	{
		onNormalAlpha = true;
		anim.SetBool("OnNormalAlpha", onNormalAlpha);
	}

	public void OnNormalAlphaEnd()
	{
		onNormalAlpha = false;
		anim.SetBool("OnNormalAlpha", onNormalAlpha);
	}

	public void OnSpecialAlphaStart()
	{
		onSpecialAlpha = true;
		anim.SetBool("OnSpecialAlpha", onSpecialAlpha);
		Vector2 speed = new Vector2(-30, 0);
	}

	public void OnSpecialAlphaEnd()
	{
		onSpecialAlpha = false;
		anim.SetBool("OnSpecialAlpha", onSpecialAlpha);
	}

	public void OnLightHitStart()
	{
		onLightHit = true;
		anim.SetBool ("OnLightHit", onLightHit);
	}

	public void OnLightHitEnd()
	{
		onLightHit = false;
		Debug.Log ("OnLightHitEnd");
		anim.SetBool ("OnLightHit", onLightHit);
	}

	public bool IsBusy()
	{
		return onNormalAlpha || onSpecialAlpha || onLightHit;
	}
}