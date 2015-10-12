﻿using UnityEngine;
using System.Collections;

public class Monk : Character
{
	//public members
	public float health = 200;

	//private members

	// Use this for initialization
	void Start() 
	{
		Initialize(health);
		Debug.Log (this.ToString ());
	}
	
	// Update is called once per frame
	void Update() 
	{

	}

	override public void SpecialMoveAlpha()
	{
		FlyingKick();
	}

	public void FlyingKick()
	{
		float speed = -40;
		float duration = 0.30f;

		if (!base.moveHandler.IsBusy()) 
		{
			base.moveHandler.OnSpecialAlphaStart();
			float direction = IsFacingLeft () ? 1 : -1;
			Vector2 velocity = new Vector2(speed, 0);
			StartCoroutine(MoveOverTime(direction * velocity, duration));
		}
	}

	//Coroutines
	IEnumerator MoveOverTime(Vector2 speed, float duration)
	{
		yield return new WaitForEndOfFrame();

		float currentTime = 0;
		while(currentTime < duration)
		{
			currentTime = currentTime + Time.deltaTime;
			float xDisplacement = speed.x * Time.deltaTime;
			float yDisplacement = speed.y * Time.deltaTime;
			this.transform.position = new Vector3(xDisplacement + this.transform.position.x, 
			                                      yDisplacement + this.transform.position.y, 
			                                      this.transform.position.z);
			yield return new WaitForEndOfFrame();
		}
	}
}