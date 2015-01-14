/**
* Modified Unity default Script
*/

var cameraTransform : Transform;
private var _target : Transform;

// The distance in the x-z plane to the target
var speed = 5.0f;
var distance = 7.0;

// the height we want the camera to be above the target
var height = 3.0;

var angularSmoothLag = 0.3;
var angularMaxSpeed = 15.0;

var heightSmoothLag = 0.3;

var snapSmoothLag = 0.2;
var snapMaxSpeed = 720.0;

var clampHeadPositionScreenSpace = 0.75;

var lockCameraTimeout = 0.2;
var overallSmooth = 8f;
var distanceUp = 2.0f;

private var headOffset = Vector3.zero;
private var centerOffset = Vector3.zero;
private var offX : Vector3;
private var heightVelocity = 0.0;
private var angleVelocity = 0.0;
private var snap = false;
private var controller : ThirdPersonController;
private var targetHeight = 100000.0; 

// CAMERA MOUSE LOOK
public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 };
public var axes : RotationAxes = RotationAxes.MouseXAndY;
public var sensitivityX = 15F;
public var sensitivityY = 15F;

public var minimumX = -360F;
public var maximumX = 360F;

public var minimumY = -60F;
public var maximumY = 60F;

var rotationX = 0F;
var rotationY = 0F;
private var originalRotation : Quaternion;

function Awake ()
{
	offX = new Vector3(0, height, distance);
	if(!cameraTransform && Camera.main)
		cameraTransform = Camera.main.transform;
	if(!cameraTransform) {
		Debug.Log("Please assign a camera to the ThirdPersonCamera script.");
		enabled = false;	
	}
			
		
	_target = transform;
	originalRotation = cameraTransform.localRotation;
	if (_target)
	{
		controller = _target.GetComponent(ThirdPersonController);
	}
	
	if (controller)
	{
		var characterController : CharacterController = _target.GetComponent.<Collider>();
		centerOffset = characterController.bounds.center - _target.position;
		headOffset = centerOffset;
		headOffset.y = characterController.bounds.max.y - _target.position.y;
	}
	else
		Debug.Log("Please assign a target to the camera that has a ThirdPersonController script attached.");

	SnapToCharacter();
	Cut(_target, centerOffset);
}

function SnapToCharacter()
{
	cameraTransform.LookAt(_target);
}

function DebugDrawStuff ()
{
	Debug.DrawLine(_target.position, _target.position + headOffset);

}

function AngleDistance (a : float, b : float)
{
	a = Mathf.Repeat(a, 360);
	b = Mathf.Repeat(b, 360);
	
	return Mathf.Abs(b - a);
}

function Apply ()
{
	// Early out if we don't have a target
	if (!controller)
		return;
	
	var targetCenter = _target.position + centerOffset;
	var targetHead = _target.position + headOffset;

//	DebugDrawStuff();

	// Calculate the current & target rotation angles
	var originalTargetAngle = _target.eulerAngles.y;
	var currentAngle = cameraTransform.eulerAngles.y;

	// Adjust real target angle when camera is locked
	var targetAngle = originalTargetAngle; 
	
	// When pressing Fire2 (alt) the camera will snap to the target direction real quick.
	// It will stop snapping when it reaches the target
	if (Input.GetButton("Fire2"))
		snap = true;
	
	if (snap)
	{
		// We are close to the target, so we can stop snapping now!
		if (AngleDistance (currentAngle, originalTargetAngle) < 3.0)
			snap = false;
		
		//currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, angleVelocity, snapSmoothLag, snapMaxSpeed);
	}
	// Normal camera motion
	else
	{
		if (controller.GetLockCameraTimer () < lockCameraTimeout)
		{
			targetAngle = currentAngle;
		}

		// Lock the camera when moving backwards!
		// * It is really confusing to do 180 degree spins when turning around.
		if (AngleDistance (currentAngle, targetAngle) > 160 && controller.IsMovingBackwards ())
			targetAngle += 180;

		currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, angleVelocity, angularSmoothLag, angularMaxSpeed);
	}


	// When jumping don't move camera upwards but only down!
	if (controller.IsJumping ())
	{
		// We'd be moving the camera upwards, do that only if it's really high
		var newTargetHeight = targetCenter.y + height;
		if (newTargetHeight < targetHeight || newTargetHeight - targetHeight > 5)
			targetHeight = targetCenter.y + height;
	}
	// When walking always update the target height
	else
	{
		targetHeight = targetCenter.y + height;
	}

	// Damp the height
	var currentHeight = cameraTransform.position.y;
	currentHeight = Mathf.SmoothDamp (currentHeight, targetHeight, heightVelocity, heightSmoothLag);

	// Convert the angle into a rotation, by which we then reposition the camera
	var currentRotation = Quaternion.Euler (0, currentAngle, 0);
	
	// Set the position of the camera on the x-z plane to:
	// distance meters behind the target
	//var targetPosition = targetCenter;
	//targetPosition += currentRotation * Vector3.back * distance;
	//cameraTransform.position = targetPosition;
	
	var movement = false;
	var targetPosition = _target.position + _target.up * distanceUp - _target.forward * distance;
	cameraTransform.position = targetPosition;
	var newMaxX = ClampAngle(_target.localEulerAngles.y + maximumX, -360, 360);
	var newMinX = ClampAngle(_target.localEulerAngles.y - maximumX, -360, 360);
	Debug.Log(newMaxX + "+" + newMinX);
	if(newMinX > newMaxX)
	{
		newMaxX = newMinX;
		newMinX = newMaxX;
	}
	//cameraTransform.Rotate(new Vector3(0, _target.rotation.y, 0));
	//cameraTransform.LookAt(transform.position);
	
	if (axes == RotationAxes.MouseXAndY) {
		rotationX += Input.GetAxis("Mouse X") * sensitivityX;
		rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

		rotationX = ClampAngle (rotationX, newMinX, newMaxX);
		rotationY = ClampAngle (rotationY, minimumY, maximumY);
		
		var xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
		var yQuaternion = Quaternion.AngleAxis (rotationY, Vector3.left);
		
		cameraTransform.localRotation = originalRotation * xQuaternion * yQuaternion;
	}
	else if (axes == RotationAxes.MouseX) 
	{
		rotationX += Input.GetAxis("Mouse X") * sensitivityX;
		rotationX = ClampAngle (rotationX, newMinX, newMaxX);

		xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
		cameraTransform.localRotation = originalRotation * xQuaternion;
	}
	else 
	{
		rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
		rotationY = ClampAngle (rotationY, minimumY, maximumY);

		yQuaternion = Quaternion.AngleAxis (rotationY, Vector3.left);
		cameraTransform.localRotation = originalRotation * yQuaternion;
	}
	
	
	
	/*
	var floor : RaycastHit;
	if(Input.GetKey(KeyCode.RightArrow))
	{
		offX = Quaternion.AngleAxis(speed, Vector3.up) * offX;
	}
	if(Input.GetKey(KeyCode.LeftArrow))
	{
		offX = Quaternion.AngleAxis(speed, Vector3.down) * offX;
	}
	if(Input.GetKey(KeyCode.DownArrow))
	{
	
	     if (Physics.Raycast(cameraTransform.position, Vector3.down, floor, 0.8f))
	     {
	     	offX = Quaternion.AngleAxis(speed * 2, Vector3.left) * offX;
	     }
	     else
	     {
	     	offX = Quaternion.AngleAxis(speed, Vector3.right) * offX;
	     }
	     
	     offX = Quaternion.AngleAxis(speed, Vector3.left) * offX;
	}
	if(Input.GetKey(KeyCode.UpArrow))
	{
	    /*if (Physics.Raycast(cameraTransform.position, Vector3.down, floor, 0.8f))
	    {
	    	offX = Quaternion.AngleAxis(speed * 2, Vector3.right) * offX;
	    }
	    else
	    {
	    	offX = Quaternion.AngleAxis(speed, Vector3.left) * offX;
	    }
	    offX = Quaternion.AngleAxis(speed, Vector3.left) * offX;
	}
	 var hit : RaycastHit;
     if (Physics.Raycast(cameraTransform.position, _target.position - cameraTransform.position, hit, distance*0.90))
     {
     	Debug.Log("infront of camera");
     }
     Debug.DrawRay (cameraTransform.position, (_target.position - cameraTransform.position) * (distance/4), Color.green);
	
	cameraTransform.position = _target.position + offX;*/
	
	
	//cameraTransform.Translate(directionVec * speed * Time.deltaTime);
	//cameraTransform.position = new Vector3(cameraTransform.position.x,cameraTransform.position.y, _target.position.z - distance);
	// Set the height of the camera
	//cameraTransform.position.y = currentHeight;
	// Always look at the target	
	//SetUpRotation(targetCenter, targetHead);
}

function LateUpdate () {
	Apply();
}

function Cut (dummyTarget : Transform, dummyCenter : Vector3)
{
	var oldHeightSmooth = heightSmoothLag;
	var oldSnapMaxSpeed = snapMaxSpeed;
	var oldSnapSmooth = snapSmoothLag;
	
	snapMaxSpeed = 10000;
	snapSmoothLag = 0.001;
	heightSmoothLag = 0.001;
	
	snap = true;
	Apply();
	
	heightSmoothLag = oldHeightSmooth;
	snapMaxSpeed = oldSnapMaxSpeed;
	snapSmoothLag = oldSnapSmooth;
}

function SetUpRotation (centerPos : Vector3, headPos : Vector3)
{
	// Now it's getting hairy. The devil is in the details here, the big issue is jumping of course.
	// * When jumping up and down we don't want to center the guy in screen space.
	//  This is important to give a feel for how high you jump and avoiding large camera movements.
	//   
	// * At the same time we dont want him to ever go out of screen and we want all rotations to be totally smooth.
	//
	// So here is what we will do:
	//
	// 1. We first find the rotation around the y axis. Thus he is always centered on the y-axis
	// 2. When grounded we make him be centered
	// 3. When jumping we keep the camera rotation but rotate the camera to get him back into view if his head is above some threshold
	// 4. When landing we smoothly interpolate towards centering him on screen
	var cameraPos = cameraTransform.position;
	var offsetToCenter = centerPos - cameraPos;
	
	// Generate base rotation only around y-axis
	var yRotation = Quaternion.LookRotation(Vector3(offsetToCenter.x, 0, offsetToCenter.z));

	var relativeOffset = Vector3.forward * distance + Vector3.down * height;
	//cameraTransform.rotation = yRotation * Quaternion.LookRotation(relativeOffset);

	// Calculate the projected center position and top position in world space
	var centerRay = cameraTransform.GetComponent.<Camera>().ViewportPointToRay(Vector3(.5, 0.5, 1));
	var topRay = cameraTransform.GetComponent.<Camera>().ViewportPointToRay(Vector3(.5, clampHeadPositionScreenSpace, 1));

	var centerRayPos = centerRay.GetPoint(distance);
	var topRayPos = topRay.GetPoint(distance);
	
	var centerToTopAngle = Vector3.Angle(centerRay.direction, topRay.direction);
	
	var heightToAngle = centerToTopAngle / (centerRayPos.y - topRayPos.y);

	var extraLookAngle = heightToAngle * (centerRayPos.y - centerPos.y);
	if (extraLookAngle < centerToTopAngle)
	{
		extraLookAngle = 0;
	}
	else
	{
		extraLookAngle = extraLookAngle - centerToTopAngle;
		//cameraTransform.rotation *= Quaternion.Euler(-extraLookAngle, 0, 0);
	}
}

static function ClampAngle (angle : float, min : float, max : float) : float {
	if (angle < -360.0)
		angle += 360.0;
	if (angle > 360.0)
		angle -= 360.0;
	return Mathf.Clamp (angle, min, max);
}

function GetCenterOffset ()
{
	return centerOffset;
}
