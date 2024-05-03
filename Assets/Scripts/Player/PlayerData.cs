using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
	[Header("Gravity")]
	[HideInInspector] public float gravityStrength;
	[HideInInspector] public float gravityScale; // 프로젝트 중력값을 고려한 실제 게임 상의 중력값 = RigidBody의 중력값
	
	[Header("Gravity")]
	[Space(5)]
	public float fallGravityMult; // 낙하 시 중력 값에 대한 Multiflier값
	public float maxFallSpeed; // 낙하 시 최대 종단 속도
	
	[Space(5)]
	public float fastFallGravityMult; // fallGravityMult보다 더 큰 Multiflier : 점프 후 아래 키를 누를 시 더 빠른 낙하
	public float maxFastFallSpeed; // 빠른 낙하를 할 시 최대 종단 속도
	
	[Space(20)]
	[Header("Run")]
	public float runMaxSpeed; // 플레이어 최대 이동 속도
	public float runAccel; // 가속도 값 : 0일 시 현재 속도 유지, 1일 시 즉시 최대, 최소 속도로 바뀜
	public float runDeccel; // 0과 1 사이일 시 서서히 가속, 감속
	[HideInInspector] public float runAccelAmount; // 최대 이동 속도를 고려한 실제 가속도 값
	[HideInInspector] public float runDeccelAmount;
	
	[Space(5)]
	[Range(0f, 1f)] public float accelInAir; // 공중에 있을 시 가속도 값에 대한 Multiflier
	[Range(0f, 1f)] public float deccelInAir;

	[Space(20)]
	[Header("Jump")]
	public int jumpAmount; // Jump 가능 횟수
	public float jumpHeight; // 플레이어 최대 점프 높이
	public float jumpTimeToApex; // 최대 점프 높이에 도달하기까지의 시간 : 중력 값과 jump force값에 영향을 줌.
	[HideInInspector] public float jumpForce; // 점프 시 위쪽으로 작용하는 힘

	[Header("Both Jumps")]
	public float jumpCutGravityMult; // 점프 버튼을 누르던 도중에 일찍 release한 경우 gravity 값을 증가시키는 Multiplier
	[Range(0f, 1)] public float jumpHangGravityMult; // 최고 높이 근처에서 gravity 값을 줄이는 Multiplier
	public float jumpHangThreshold; // 최고 높이에 도달했을 때 얼마나 jumpHang을 수행하는 시간
	
	[Space(0.5f)]
	public float jumpHangAccelerationMult;
	public float jumpHangMaxSpeedMult;

	[Header("Wall Jump")]
	public Vector2 wallJumpForce; // 벽점프 시 플레이어에 가해지는 힘의 크기
	[Range(0f, 1f)] public float wallJumpRunLerp; // 벽점프 시 플레이어의 다른 이동 제한
	[Range(0f, 1.5f)] public float wallJumpTime; // 벽점프 후 플레이어의 움직임이 느려지는 시간
	public bool doTurnOnWallJump; // Player will rotate to face wall jumping direction

	[Space(20)]
	[Header("Slide")]
	public float slideSpeed;
	public float slideAccel;

    [Header("Assists")]
	[Range(0.01f, 0.5f)] public float coyoteTime; // 벽, 바닥에 있지 않아도 점프를 할 수 있는 유예 시간
	[Range(0.01f, 0.5f)] public float jumpInputBufferTime; // 점프 버튼을 누른 뒤 실제 점프가 수행되기 전까지의 유예 시간
                                                        // 점프 버튼을 누르고 BufferTime이 지난 후 점프를 수행할 환경
                                                        // (ex. IsGround = true)이 되는 경우 자동적으로 점프를 수행

	[Space(20)]
	[Header("Dash")]
	public int dashAmount;
	public float dashSpeed;
	public float dashSleepTime; //Duration for which the game freezes when we press dash but
                             // before we read directional input and apply a force
	[Space(5)]
	public float dashAttackTime;
	
	[Space(5)]
	public float dashEndTime; //Time after you finish the inital drag phase,
                           //smoothing the transition back to idle (or any standard state)
	public Vector2 dashEndSpeed; // 대쉬 종료 시 속도
	[Range(0f, 1f)] public float dashEndRunLerp; // 대쉬 종료 시 속도 변화
	
	[Space(5)]
	public float dashCoolTime; // 대쉬 쿨타임
	
	[Space(5)]
	[Range(0.01f, 0.5f)] public float dashInputBufferTime;
	

	// 인스펙터가 업데이트될 때 자동적으로 유니티에서 호출하는 콜백 함수
    private void OnValidate()
    {
		// 공식 : (0.5) * gravity * time^2 = jumpHeightToApex 
		gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
		
		// 프로젝트 상의 중력값을 고려하여 RigidBody 상의 중력값 계산 gravity scale (project settings/Physics2D)
		gravityScale = gravityStrength / Physics2D.gravity.y;

		// 플레이어 이동 가속도의 최댓값을 이동 속도로 제한
		runAccel = Mathf.Clamp(runAccel, 0.01f, runMaxSpeed);
		runDeccel = Mathf.Clamp(runDeccel, 0.01f, runMaxSpeed);
		
		// 최대 이동 속도가 클 수록 가속도 값 작아지게 함 : 너무 빨리 이동 속도가 최대로 되는 것을 방지
		// 50 : time.fixedDeltaTime(=0.02)의 역수값
		// runAcceleration / (runMaxSpeed * Time.fixedDeltaTime)과 동일함_
		runAccelAmount = (50 * runAccel) / runMaxSpeed;
		runDeccelAmount = (50 * runDeccel) / runMaxSpeed;

		//Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
		// jumpForce가 아니라 정확히는 initJumpVelocity지만 그 값을 힘으로 취급
		jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;
    }
}

