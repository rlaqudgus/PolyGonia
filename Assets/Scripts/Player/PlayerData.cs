using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Wall Jump")]
    public Vector2 wallJumpForce; //The actual force (this time set by us) applied to the player when wall jumping.
    [Space(5)]
    [Range(0f, 1f)] public float wallJumpRunLerp; //Reduces the effect of player's movement while wall jumping.
    [Range(0f, 1.5f)] public float wallJumpTime; //Time after wall jumping the player's movement is slowed for.
    public bool doTurnOnWallJump; //Player will rotate to face wall jumping direction

    [Space(20)]

    [Header("Wall Slide")]
    public float wallSlideSpeed;
    public float wallSlideAccel;
    
    [Header("Assists")]
    [Range(0.01f, 0.5f)] public float coyoteTime; // 특정 Collider나 trigger에서 벗어나도 동작이 가능한 유예 시간
    [Range(0.01f, 0.5f)] public float jumpInputBufferTime; // Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.
}
