/**
* Modified Unity default Script
*/


// Require a character controller to be attached to the same game object
@script RequireComponent(CharacterController)

public var idleAnimation : AnimationClip;
public var walkAnimation : AnimationClip;
public var runAnimation : AnimationClip;
public var jumpPoseAnimation : AnimationClip;

public var walkMaxAnimationSpeed : float = 0.75;
public var trotMaxAnimationSpeed : float = 1.0;
public var runMaxAnimationSpeed : float = 1.0;
public var jumpAnimationSpeed : float = 1.15;
public var landAnimationSpeed : float = 1.0;

public var upSpeed : float = 2f;
public var downSpeed : float = -2f;
private var aim : float = 0f;
private var _animation : Animation;

enum CharacterState {
    Idle = 0,
	Walking = 1,
	Trotting = 2,
	Running = 3,
	Jumping = 4,
    }

private var _characterState : CharacterState;

// The speed when walking
var walkSpeed = 2.0;
// after trotAfterSeconds of walking we trot with trotSpeed
var trotSpeed = 4.0;
// when pressing "Fire3" button (cmd) we start running
var runSpeed = 6.0;

var inAirControlAcceleration = 3.0;

// How high do we jump when pressing jump and letting go immediately
var jumpHeight = 0.5;

// The gravity for the character
var gravity = 20.0;
// The gravity in controlled descent mode
var speedSmoothing = 10.0;
var rotateSpeed = 500.0;
var trotAfterSeconds = 3.0;

var canJump = true;

private var jumpRepeatTime = 0.05;
private var jumpTimeout = 0.15;
private var groundedTimeout = 0.25;

// The camera doesnt start following the target immediately but waits for a split second to avoid too much waving around.
private var lockCameraTimer = 0.0;

// The current move direction in x-z
private var moveDirection = Vector3.zero;
// The current vertical speed
private var verticalSpeed = 0.0;
// The current x-z move speed
private var moveSpeed = 0.0;

// The last collision flags returned from controller.Move
private var collisionFlags : CollisionFlags; 

// Are we jumping? (Initiated with jump button and not grounded yet)
private var jumping = false;
private var jumpingReachedApex = false;

// Are we moving backwards (This locks the camera to not do a 180 degree spin)
private var movingBack = false;
// Is the user pressing any keys?
private var isMoving = false;
// When did the user start walking (Used for going into trot after a while)
private var walkTimeStart = 0.0;
// Last time the jump button was clicked down
private var lastJumpButtonTime = -10.0;
// Last time we performed a jump
private var lastJumpTime = -1.0;


// the height we jumped from (Used to determine for how long to apply extra jump power after jumping.)
private var lastJumpStartHeight = 0.0;


private var inAirVelocity = Vector3.zero;

private var lastGroundedTime = 0.0;


private var isControllable = true;

function Awake ()
{
    moveDirection = transform.TransformDirection(Vector3.forward);
	
    _animation = GetComponent(Animation);
    if(!_animation)
        Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");
	
    /*
public var idleAnimation : AnimationClip;
public var walkAnimation : AnimationClip;
public var runAnimation : AnimationClip;
public var jumpPoseAnimation : AnimationClip;	
	*/
    if(!idleAnimation) {
        _animation = null;
        Debug.Log("No idle animation found. Turning off animations.");
    }
    if(!walkAnimation) {
        _animation = null;
        Debug.Log("No walk animation found. Turning off animations.");
    }
    if(!runAnimation) {
        _animation = null;
        Debug.Log("No run animation found. Turning off animations.");
    }
    if(!jumpPoseAnimation && canJump) {
        _animation = null;
        Debug.Log("No jump animation found and the character has canJump enabled. Turning off animations.");
    }
			
}


function UpdateSmoothedMovementDirection ()
{
    var forward = this.transform.TransformDirection(Vector3.forward);
    forward.y = 0;
    forward = forward.normalized;
    // Right vector relative to the camera
    // Always orthogonal to the forward vector
    var right = Vector3(forward.z, 0, -forward.x);
    Debug.DrawRay(transform.position, right, Color.green);
    var v = Input.GetAxisRaw("Vertical");
    var h = Input.GetAxisRaw("Horizontal");

    // Are we moving backwards or looking backwards
    if (v < -0.2)
    {
        movingBack = true;
    }
    else if( (v > 0 || h > 0 || h < 0) && !isMoving)
    {
        movingBack = false;
    }
    moveDirection = Vector3.zero;
    isMoving = Mathf.Abs (h) > 0.1 || Mathf.Abs (v) > 0.1;

    // Target direction relative to the camera
    var targetDirection = h * right + v * forward;

    // We store speed and direction seperately,
    // so that when the character stands still we still have a valid forward direction
    // moveDirection is always normalized, and we only update it if there is user input.
    if (targetDirection != Vector3.zero)
    {
        //moveDirection.y = transform.position.y;
        if(!movingBack)
        {
            moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
            moveDirection = moveDirection.normalized;
            //moveDirection.y = 0;
        }
        else
        {
            moveDirection = targetDirection;
            moveDirection = moveDirection.normalized;
            // moveDirection.y = 0;
        }
    }
        
    // Smooth the speed based on the current target direction
    var curSmooth = speedSmoothing * Time.deltaTime;
		
    // Choose target speed
    //* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
    var targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0);
	
    _characterState = CharacterState.Idle;
		
    // Pick speed modifier
    if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))
    {
        targetSpeed *= runSpeed;
        _characterState = CharacterState.Running;
    }
    else if (Time.time - trotAfterSeconds > walkTimeStart)
    {
        targetSpeed *= trotSpeed;
        _characterState = CharacterState.Trotting;
    }
    else
    {
        targetSpeed *= walkSpeed;
        _characterState = CharacterState.Walking;
    }
		
    moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);
		
    // Reset walk time start when we slow down
    if (moveSpeed < walkSpeed * 0.3)
        walkTimeStart = Time.time;
}

function CalculateJumpVerticalSpeed (targetJumpHeight : float)
{
    // From the jump height and gravity we deduce the upwards speed 
    // for the character to reach at the apex.
    return Mathf.Sqrt(2 * targetJumpHeight * gravity);
}

function FixedUpdate() {
	
    if (!isControllable)
    {
        // kill all inputs if not controllable.
        Input.ResetInputAxes();
    }
    if(Input.GetButton("Jump"))
    {
        inAirVelocity = new Vector3(0,upSpeed,0);
    }
    else if (Input.GetButton("Crouch"))
    {
        inAirVelocity = new Vector3(0,downSpeed,0);
    }
    else
    {
        inAirVelocity = Vector3.zero;
    }

    UpdateSmoothedMovementDirection();
	
    // Calculate actual motion
    var movement = moveDirection * moveSpeed + inAirVelocity;
    movement *= Time.deltaTime;
    // Move the controller
    var controller : CharacterController = GetComponent(CharacterController);
    controller.Move(movement);
    collisionFlags = CollisionFlags.Below;
	
    /*// ANIMATION sector
    if(_animation) {
        if(_characterState == CharacterState.Jumping) 
        {
            if(!jumpingReachedApex) {
                _animation[jumpPoseAnimation.name].speed = jumpAnimationSpeed;
                _animation[jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
                _animation.CrossFade(jumpPoseAnimation.name);
            } else {
                _animation[jumpPoseAnimation.name].speed = -landAnimationSpeed;
                _animation[jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
                _animation.CrossFade(jumpPoseAnimation.name);				
            }
        } 
        else 
        {
            if(controller.velocity.sqrMagnitude < 0.1) {
                _animation.CrossFade(idleAnimation.name);
            }
            else 
            {
                if(_characterState == CharacterState.Running) {
                    _animation[runAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0, runMaxAnimationSpeed);
                    _animation.CrossFade(runAnimation.name);	
                }
                else if(_characterState == CharacterState.Trotting) {
                    _animation[walkAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0, trotMaxAnimationSpeed);
                    _animation.CrossFade(walkAnimation.name);	
                }
                else if(_characterState == CharacterState.Walking) {
                    _animation[walkAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0, walkMaxAnimationSpeed);
                    _animation.CrossFade(walkAnimation.name);	
                }
				
            }
        }
    }
    // Set rotation to the move direction
    if (IsGrounded())
    {
        if(!movingBack)
        {	
            if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))
            {
            }
            else
            {
                var lookDirection = Vector3.RotateTowards(transform.forward, moveDirection, 5 * Time.deltaTime, 0.0);
                Debug.DrawRay(transform.position, lookDirection, Color.red);
                transform.rotation = Quaternion.LookRotation(lookDirection); 
            }
        }
    }	
    else
    {
        var xzMove = movement;
        xzMove.y = 0;
        if (xzMove.sqrMagnitude > 0.001)
        {
            transform.rotation = Quaternion.LookRotation(xzMove);
        }
    }	*/
}

function OnControllerColliderHit (hit : ControllerColliderHit )
{
    //	Debug.DrawRay(hit.point, hit.normal);
    if (hit.moveDirection.y > 0.01) 
        return;
}

function GetSpeed () {
    return moveSpeed;
}

function IsJumping () {
    return jumping;
}

function IsGrounded () {
    return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
}

function GetDirection () {
    return moveDirection;
}

function IsMovingBackwards () {
    return movingBack;
}

function GetLockCameraTimer () 
{
    return lockCameraTimer;
}

function IsMoving ()  : boolean
{
    return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5;
}

function HasJumpReachedApex ()
{
    return jumpingReachedApex;
}

function IsGroundedWithTimeout ()
{
    return lastGroundedTime + groundedTimeout > Time.time;
}

function Reset ()
{
    gameObject.tag = "Player";
}

