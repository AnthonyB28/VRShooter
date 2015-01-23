/**
* Modified Unity default Script
*/

var cameraTransform : Transform;
var characterHead: Transform;
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

public var maximumX = 30F;

public var minimumY = -60F;
public var maximumY = 60F;

var rotationX = 0F;
var rotationY = 0F;
private var originalRotation : Quaternion;
private var originalRotationHead : Quaternion;

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
    if(characterHead)
    {
        originalRotationHead = characterHead.localRotation;
    }
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
	
    var movement = false;
    var targetPosition = _target.position + _target.up * distanceUp - _target.forward * distance;
    cameraTransform.position = targetPosition;
    var eulerTargetY = _target.localEulerAngles.y;
    var newMaxX = 0;
    var newMinX = 0; 

    if(eulerTargetY > 180)
    {
        var adjustedEuler = eulerTargetY - 360;
        newMaxX = adjustedEuler + maximumX;
        newMinX = adjustedEuler - maximumX;
    }
    else
    {
        newMaxX = eulerTargetY + maximumX;
        newMinX = eulerTargetY - maximumX;
    }

    var xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
    var yQuaternion = Quaternion.AngleAxis (rotationY, Vector3.left);
    var xQuaternionHead = Quaternion.AngleAxis (rotationY, Vector3.up);
    var yQuaternionHead = Quaternion.AngleAxis (rotationX, Vector3.left);
    rotationX += Input.GetAxis("Mouse X") * sensitivityX;
    rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
    rotationY = ClampAngle (rotationY, minimumY, maximumY);
    rotationX = ClampAngle (rotationX, newMinX, newMaxX);

    if (axes == RotationAxes.MouseXAndY) {
        cameraTransform.localRotation = originalRotation * xQuaternion * yQuaternion;
        if(characterHead)
        {
            characterHead.localRotation = originalRotationHead * xQuaternionHead * yQuaternionHead;
        }
    }
    else if (axes == RotationAxes.MouseX) 
    {
        cameraTransform.localRotation = originalRotation * xQuaternion;
        if(characterHead)
        {
            characterHead.localRotation = originalRotationHead * xQuaternionHead;
        }
    }
    else 
    {
        cameraTransform.localRotation = originalRotation * yQuaternion;
        if(characterHead)
        {
            characterHead.localRotation = originalRotationHead * yQuaternionHead;
        }
    }
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
