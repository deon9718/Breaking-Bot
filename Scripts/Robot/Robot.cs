﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Robot : MonoBehaviour {
    public enum CharacterState { Idle, Blocking, BlockStun, LightFlinch, HeavyFlinch, LeftPunch, RightPunch, LeftKick, RightKick };

    //Constants
    const int PartCount = 4;
    const int LeftArm = 0;
    const int RightArm = 1;
    const int LeftLeg = 2;
    const int RightLeg = 3;

    //Public members
    public string mytag = "null";
    public Part[] robotParts;
    public Rigidbody2D rigidbodyTwoD;
    public float maxHealth = 100f;
    public float currentHealth = 100f;
	public Text healthNum;
	public Slider healthBar;
	public Image Fill;


    //Private members
    private GameManager gm;
    private CharacterState currentState;
    private Animator anim;
    private bool isFacingLeft = false;
    private bool isGrounded = true;
    private bool triggered = false;
	private bool comboState = false;
	private RobotHurtBox hurtBox;
    private IEnumerator moveTimeRoutine;
    private IEnumerator delayedJump;

    void Start ()
    {
        currentState = CharacterState.Idle;
        anim = gameObject.GetComponent<Animator>();
        rigidbodyTwoD = this.gameObject.GetComponent<Rigidbody2D>();
		hurtBox = this.transform.FindChild ("HurtBox").GetComponent<RobotHurtBox>();
        gm = (GameManager)GameObject.Find("GameManager").GetComponent<GameManager>();
        InitializeParts();

        if (this.transform.right.x < 0)
        {
            isFacingLeft = true;


        }
        else
        {
            isFacingLeft = false;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		healthBar.value = currentHealth / maxHealth * 100;
		healthNum.text = healthBar.value.ToString ();

		if (healthBar.value <= 40) {
			Fill.color = Color.red;
		} else 
			Fill.color = Color.green;

	    if(currentHealth <= 0)
        {
            gm.thisPlayerDied(mytag);
        }
	}

    private void InitializeParts()
    {
        Debug.Log("InitializeParts" + this);
        robotParts = new Part[PartCount];
        Transform partsObj = this.transform.FindChild("Parts");
        robotParts[LeftArm] = partsObj.GetChild(LeftArm).GetComponent<Part>();
        robotParts[LeftLeg] = partsObj.GetChild(LeftLeg).GetComponent<Part>();
        robotParts[RightLeg] = partsObj.GetChild(RightLeg).GetComponent<Part>();
    }

    //Attack Moves
    public void LeftPunch()
    {
		CharacterState thisMove = CharacterState.LeftPunch;

		if (!IsBusy() || CanComboMove(thisMove))
        {
            robotParts[LeftArm].Attack();
			comboState = false;
			anim.SetTrigger(robotParts[LeftArm].GetTrigger());

            if(isGrounded)
            {
                //rigidbodyTwoD.velocity = new Vector2(0, rigidbodyTwoD.velocity.y);
            }
           

			currentState = thisMove;
        }
    }

    public void LeftKick()
    {
		CharacterState thisMove = CharacterState.LeftKick;

		if(!IsBusy() || CanComboMove(thisMove))
        {
            robotParts[LeftLeg].Attack();
			comboState = false;
			anim.SetTrigger(robotParts[LeftLeg].GetTrigger());
            rigidbodyTwoD.velocity = new Vector2(0, rigidbodyTwoD.velocity.y);

			currentState = thisMove;
        }
    }

    public void RightKick()
    {
		CharacterState thisMove = CharacterState.RightKick;

		if (!IsBusy() || CanComboMove(thisMove))
        {
            robotParts[RightLeg].Attack();
			comboState = false;
			anim.SetTrigger(robotParts[RightLeg].GetTrigger());
            rigidbodyTwoD.velocity = new Vector2(0, rigidbodyTwoD.velocity.y);

			currentState = thisMove;
        }
    }

    public void Block()
    {
        if(( currentState == CharacterState.Blocking || !IsBusy() ) && isGrounded)
        {
            if(!IsBusy() && currentState != CharacterState.Blocking)
            {
                anim.SetTrigger("Block");

                currentState = CharacterState.Blocking;
            }

            rigidbodyTwoD.velocity = new Vector2(0, rigidbodyTwoD.velocity.y);
        }
    }

    public void UnBlock()
    {
        if(currentState == CharacterState.Blocking)
        {
            anim.SetTrigger("UnBlock");

            currentState = CharacterState.Idle;
        }
    }

    //Rocket Moves
    public void RocketLeftArm()
    {
        anim.SetTrigger("RocketLeftArm");
    }

    private void ShootPart(int index)
    {
        if(robotParts[index].active)
        {
            GameObject rocketPrefab = Instantiate(Resources.Load("RocketParts/TigerLeftHandRocket")) as GameObject;
            RobotRocket rocket = rocketPrefab.GetComponent<RobotRocket>();
            rocket.SetOwner(this);
            Physics2D.IgnoreCollision(rocket.GetComponent<Collider2D>(), this.GetComponent<Collider2D>());
            rocket.transform.position = this.transform.FindChild("Pelvis").FindChild("Chest").FindChild("LeftShoulder").transform.position;
            Rigidbody2D rocketBody = rocket.GetComponent<Rigidbody2D>();
            float direction = IsFacingLeft() ? -1 : 1;

            if (IsFacingLeft())
            {
                Debug.Log("rotate");
                rocket.transform.Rotate(new Vector3(0, 180, 0));
            }

            rocketBody.velocity = new Vector2(direction * 200, 0);
        }
    }

    public void ShootLeftArm()
    {
        Debug.Log("ShootLeftArm");
        ShootPart(0);
    }

	public void OnHitConnected()
	{
		comboState = true;
	}

    //Movement
    public void Jump()
    {
        if (!IsBusy() && isGrounded)
        {
            //rigidbodyTwoD.velocity = new Vector2(rigidbodyTwoD.velocity.x, 100);

            if(delayedJump != null)
            {
                StopCoroutine(delayedJump);
            }

            anim.SetTrigger("JumpStart");
            delayedJump = DelayedJump(0.05f);
            StartCoroutine(delayedJump);
        }
    }

    public void FaceLeft()
    {
        if (!isFacingLeft && !IsBusy())
        {
            this.transform.Rotate(new Vector3(0, 180, 0));
            isFacingLeft = true;
        }
    }

    public void FaceRight()
    {
        if (isFacingLeft && !IsBusy())
        {
            this.transform.Rotate(new Vector3(0, -180, 0));
            isFacingLeft = false;
        }
    }

    public void MoveLeft()
    {
        if (!IsBusy())
        {
            FaceLeft();
            rigidbodyTwoD.velocity = new Vector2(-50, rigidbodyTwoD.velocity.y);
            DustEffect();
        }
    }

    public void MoveRight()
    {
        if (!IsBusy())
        {
            FaceRight();
            rigidbodyTwoD.velocity = new Vector2(50, rigidbodyTwoD.velocity.y);
            DustEffect();
        }
    }

    public void StayStill()
    {
        if (!IsBusy())
        {
            rigidbodyTwoD.velocity = new Vector2(0, rigidbodyTwoD.velocity.y);
        }
    }
    public bool IsFacingLeft()
    {
        return isFacingLeft;
    }

    private void DustEffect()
    {
        if (isGrounded)
        {
            /*
            GameObject dust = (GameObject)Resources.Load("Particles/Dust");
            var dustCloneLeft = Instantiate(dust, this.transform.Find("torso").Find("upperLeftLeg").Find("lowerLeftLeg").Find("leftFoot").position, Quaternion.identity);
            var dustCloneRight = Instantiate(dust, this.transform.Find("torso").Find("upperRightLeg").Find("lowerRightLeg").Find("rightFoot").position, Quaternion.identity);

            Destroy(dustCloneLeft, dust.GetComponent<ParticleSystem>().startLifetime);
            Destroy(dustCloneRight, dust.GetComponent<ParticleSystem>().startLifetime); 
            */
        }
    }

    public void HeavyHitStun(float damage, Vector2 pushVelocity, float duration)
    {
		comboState = false;
        currentHealth = currentHealth - damage;
        //healthBar.value = currentHealth / maxHealth * 100;
        anim.SetTrigger("HeavyHit");
        CancelAttacks();

        currentState = CharacterState.HeavyFlinch;

        if (moveTimeRoutine != null)
        {
            StopCoroutine(moveTimeRoutine);
        }

        moveTimeRoutine = MoveOverTime(pushVelocity, duration);
        StartCoroutine(moveTimeRoutine);
    }

    public void BlockStun(Vector2 pushVelocity, float duration)
    {
        anim.SetTrigger("BlockStun");

        currentState = CharacterState.BlockStun;

        Debug.Log("BlockStun");
        if (moveTimeRoutine != null)
        {
            StopCoroutine(moveTimeRoutine);
        }

        moveTimeRoutine = MoveOverTime(pushVelocity, duration);
        StartCoroutine(moveTimeRoutine);
    }

    public void EnableHitBox(int limbIndex)
    {
        robotParts[limbIndex].EnableHitBox();
    }

    public void DisableHitBox(int limbIndex)
    {
        robotParts[limbIndex].DisableHitBox();
    }

	public void EnableHurtBox()
	{
		hurtBox.EnableHurtBox();
	}

	public void DisableHurtBox()
	{
		hurtBox.DisableHurtBox();
	}

    public void OnMoveOrFlinchEnd()
    {
		comboState = false;
        currentState = CharacterState.Idle;
        //CancelAttacks();

        Debug.Log("finished " + this + " "+ currentState);

        if(delayedJump != null)
        {
            StopCoroutine(delayedJump);
        }
    }

    public void OnBlockStunEnd()
    {
        currentState = CharacterState.Blocking;
        CancelAttacks();
    }

    public bool IsBusy()
    {
        return currentState != CharacterState.Idle;
    }

	public bool CanComboMove(CharacterState nextMove)
	{
		bool canComboMove = false;

		if(   (currentState == CharacterState.LeftPunch)
		   || (currentState == CharacterState.LeftKick  && nextMove != CharacterState.LeftKick)
		   || (currentState == CharacterState.RightKick && nextMove == CharacterState.LeftKick))
		{
			canComboMove = true;
		}

		return canComboMove && comboState;
	}

    public void CancelAttacks()
    {
        robotParts[LeftArm].CancelAttack();
        robotParts[LeftLeg].CancelAttack();
        robotParts[RightLeg].CancelAttack();
    }

    public CharacterState GetCurrentState()
    {
        return currentState;
    }

    //Collisions
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground" || col.gameObject.tag == "Platform")
        {
            Collider2D collider = col.collider;

            Vector2 normal = col.contacts[0].normal;

            if (normal.y == 1)
            {
                isGrounded = true;
                DustEffect();
            }
        }

    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground" || col.gameObject.tag == "Platform")
        {
            Collider2D collider = col.collider;

            Vector2 normal = col.contacts[0].normal;
            if (normal.y == 1)
            {
                isGrounded = false;
            }
        }
    }

    public string getTag()
    {
        return mytag;
    }

    public void setTag(string tag)
    {
        mytag = tag;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggered && other.gameObject.name == "DeathArea")
        {
            triggered = true;
            gm.thisPlayerDied(mytag);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!triggered)
        {
            return;
        }
        triggered = false;
    }

    //Coroutines
    IEnumerator MoveOverTime(Vector2 velocity, float duration)
    {
        yield return new WaitForFixedUpdate();

        float currentTime = 0;
        while (currentTime < duration)
        {
            float xDisplacement = velocity.x * Time.fixedDeltaTime;
			float yDisplacement = velocity.y * Time.fixedDeltaTime;
            float zPosition = this.transform.position.z;

            Vector3 displacement = new Vector3(xDisplacement, yDisplacement, zPosition);
            rigidbodyTwoD.MovePosition(this.transform.position + displacement);

			currentTime = currentTime + Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator DelayedJump(float duration)
    {
        yield return new WaitForSeconds(duration);

        rigidbodyTwoD.AddForce(new Vector2(0, 100), ForceMode2D.Impulse);
    }
}
