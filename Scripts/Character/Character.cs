﻿using UnityEngine;
using System.Collections;

public abstract class Character : MonoBehaviour 
{
	public Transform spawnPoint;
	public Rigidbody2D rigidbodyTwoD;

	private bool isFacingLeft = true;

	protected float hitPoints = 100;
	protected MoveEventHandler moveHandler;

	protected void Initialize(float hitPoints)
	{
		this.hitPoints = hitPoints;
		moveHandler = this.gameObject.GetComponent<MoveEventHandler>();

		//This is backwards since our prefabis facing left by default
		if(this.transform.right.x > 0)
		{
			isFacingLeft = true;
		}
		else
		{
			isFacingLeft = false;
		}

		this.rigidbodyTwoD = this.gameObject.GetComponent<Rigidbody2D>();
	}

	virtual public void NormalMoveAlpha()
	{
		moveHandler.OnNormalAlphaStart();
	}
	
	virtual public void NormalMoveBeta()
	{
		moveHandler.OnNormalBetaStart();
	}

	virtual public void SpecialMoveAlpha()
	{
		moveHandler.OnSpecialAlphaStart();
	}

	virtual public void LightHitStun()
	{
		moveHandler.OnLightHitStart();
	}
	
	virtual public void HeavyHitStun()
	{
		moveHandler.OnHeavyHitStart();
	}

	public void Jump()
	{
		if(!moveHandler.IsBusy())
		{
			//jump
		}
	}

	public void FaceLeft()
	{
		if(!isFacingLeft && !moveHandler.IsBusy())
		{
			this.transform.Rotate(new Vector3(0,180,0));
			isFacingLeft = true;
		}
	}

	public void FaceRight()
	{
		if(isFacingLeft && !moveHandler.IsBusy())
		{
			this.transform.Rotate(new Vector3(0,-180,0));
			isFacingLeft = false;
		}
	}

	public bool IsFacingLeft()
	{
		return isFacingLeft;
	}

	public float GetHitPoints()
	{
		return hitPoints;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.name == "DeathArea") {
			this.gameObject.SetActive(false);
			transform.position = spawnPoint.position;
			this.gameObject.SetActive(true);
		}
	}
}
