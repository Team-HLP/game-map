using UnityEngine;
using System.Collections;
using VSX.Utilities;

namespace VSX.Engines3D
{

	/// <summary>
    /// This class contains static methods for calculating PID-controller-based maneuvers for AI and autopilots.
    /// </summary>
	public class Maneuvring : MonoBehaviour 
	{
	
		/// <summary>
        /// Turn toward a world position in space.
        /// </summary>
        /// <param name="vehicleTransform">The transform of the vehicle.</param>
        /// <param name="targetPosition">The position the vehicle must turn toward.</param>
        /// <param name="PIDCoeffs">The PID coefficients for this vehicle.</param>
        /// <param name="maxRotationAngles">The maximum rotation angles about each axis that the vehicle can reach during maneuvers.</param>
        /// <param name="controlValues">Get the control values calculated for the maneuver.</param>
        /// <param name="integralVals">Get the updated integral values for the PID controller.</param>
		public static void TurnToward (Transform vehicleTransform, Vector3 targetPosition, Vector3 maxRotationAngles, PIDController3D steeringPIDController)
		{
            
			Vector3 tmpRelPos = GetRollAndPitchUnaffectedRelPos(vehicleTransform, targetPosition);
			float zxAngleGlobal = Vector3.Angle(Vector3.forward, new Vector3(tmpRelPos.x, 0f, tmpRelPos.z));

			if (tmpRelPos.y > 0)
			{
				zxAngleGlobal *= -1;
			}
	
			// Get the roll angle
			float xyAngleGlobal = Vector3.Angle(Vector3.up, new Vector3(tmpRelPos.x, tmpRelPos.y, 0));
			if (tmpRelPos.x > 0)
			{
				xyAngleGlobal *= -1;
			}
	
			// Get the ship's current roll
			float currentRollAngle = GetRollAngleFromHorizon(vehicleTransform);
            
			
			float desiredRollAngle;
			if (Mathf.Abs(xyAngleGlobal) > maxRotationAngles.z)
			{
				desiredRollAngle = maxRotationAngles.z * Mathf.Sign(xyAngleGlobal);
			}
			else
			{
				desiredRollAngle = xyAngleGlobal;
			}

            desiredRollAngle *= Mathf.Clamp(new Vector3(tmpRelPos.x, tmpRelPos.y, 0).magnitude, 0, 1); // Ignore roll when target is close to nose


            desiredRollAngle *= Mathf.Clamp((Mathf.Abs(zxAngleGlobal) / 45f) - 0.05f, 0, 1);
			float relativeRollAngle = desiredRollAngle - currentRollAngle;
	
			Vector3 toTargetDir = (targetPosition - vehicleTransform.position).normalized;
	
			Vector3 tmp = GetRollAndPitchUnaffectedRelPos(vehicleTransform, targetPosition);
	
			// This is the pitch from the totarget vector to the vertically flattened totarget vector
			float pitchAngleToTarget_Global = Mathf.Approximately(Mathf.Abs(Vector3.Dot(toTargetDir, Vector3.up)), 1) ? 90 : Vector3.Angle(toTargetDir, new Vector3(toTargetDir.x, 0, toTargetDir.z));
            
            pitchAngleToTarget_Global = tmp.y > 0 ? pitchAngleToTarget_Global * -1 : pitchAngleToTarget_Global;
			
			// this is the pitch from the ship's forward vector to the vertically flattened ships forward vector
			float currentPitchAngle_Global = Vector3.Angle(vehicleTransform.forward, new Vector3(vehicleTransform.forward.x, 0f, vehicleTransform.forward.z));
			currentPitchAngle_Global = vehicleTransform.forward.y > 0 ? currentPitchAngle_Global * -1 : currentPitchAngle_Global;
			float clampedTargetPitch = Mathf.Clamp(pitchAngleToTarget_Global, -maxRotationAngles.x, maxRotationAngles.x);
			
			float relativePitchAngle = clampedTargetPitch - currentPitchAngle_Global;
	
			// THis is is yaw angle from the vertically flattened ship forward, to the vertically flattened totarget
			float yawAngleToTarget_Global = Vector3.Angle(Vector3.forward, new Vector3(tmp.x, 0f, tmp.z));
            
            yawAngleToTarget_Global = tmp.x < 0 ? yawAngleToTarget_Global * -1 : yawAngleToTarget_Global;
	
			Vector3 pitchAndYaw = new Vector3(yawAngleToTarget_Global, relativePitchAngle, 0f);
			pitchAndYaw = Quaternion.Euler(0f, 0f, currentRollAngle) * pitchAndYaw;

            // Update the PID Controller
            
            steeringPIDController.SetError(PIDController3D.Axis.X, pitchAndYaw.y, -1f * pitchAndYaw.y);
            steeringPIDController.SetError(PIDController3D.Axis.Y, pitchAndYaw.x, -1f * pitchAndYaw.x);
            steeringPIDController.SetError(PIDController3D.Axis.Z, relativeRollAngle, -1f * relativeRollAngle);
            
			float angleToTarget = Vector3.Angle(vehicleTransform.forward, targetPosition - vehicleTransform.position);
			if (angleToTarget > 25)
			{
                steeringPIDController.SetIntegralInfluence(0);
			}
			else
			{
                steeringPIDController.SetIntegralInfluence(1);
            }
		}
        
        public static void TranslateToward(Rigidbody rBody, Vector3 targetPosition, PIDController3D movementPIDController)
        {

            // Get the relative target position
            Vector3 targetRelPos = rBody.transform.InverseTransformPoint(targetPosition);

            // Get the to-target vector
            Vector3 toTargetVector = targetPosition - rBody.transform.position;
            
            // Calculate the closing velocity
            Vector3 closingVelocity = Vector3.Dot(rBody.velocity.normalized, toTargetVector.normalized) * rBody.velocity;
            Vector3 distanceChangeRate = -1 * new Vector3(Mathf.Abs(closingVelocity.x), Mathf.Abs(closingVelocity.y), Mathf.Abs(closingVelocity.z));

            // Update the PID Controller

            movementPIDController.SetError(PIDController3D.Axis.X, targetRelPos.x, distanceChangeRate.x);
            movementPIDController.SetError(PIDController3D.Axis.Y, targetRelPos.y, distanceChangeRate.y);
            movementPIDController.SetError(PIDController3D.Axis.Z, targetRelPos.z, distanceChangeRate.z);

            float integralPhaseInDistance = 100;
            float dist = targetRelPos.magnitude;
            float amount = Mathf.Clamp(1 - (dist / integralPhaseInDistance), 0, 1);

            movementPIDController.SetIntegralInfluence(amount);

        }



        // Get the relative position of a point from a transform, regardless of the roll (local z axis rotation) of the transform
        private static Vector3 GetRollUnaffectedRelPos(Transform t, Vector3 point){
	
			Quaternion rot = Quaternion.LookRotation(t.forward, Vector3.up);
			Vector3 result = Vector3.Scale(new Vector3(1/t.lossyScale.x, 1/t.lossyScale.y, 1/t.lossyScale.z), Quaternion.Inverse(rot) * (point - t.position));
			return result; 
	
		}
	

		// Get the relative position of a point from a transform, regardless of the roll (local z axis rotation) or pitch (local x axis rotation)of the transform
		private static Vector3 GetRollAndPitchUnaffectedRelPos(Transform t, Vector3 point)
        {
            Vector3 viewingVector = new Vector3(t.forward.x, 0f, t.forward.z).normalized;

            Quaternion rot = (viewingVector.magnitude < 0.0001f) ? Quaternion.identity : Quaternion.LookRotation(viewingVector, Vector3.up);
            
			return Vector3.Scale(new Vector3(1/t.lossyScale.x, 1/t.lossyScale.y, 1/t.lossyScale.z), Quaternion.Inverse(rot) * (point - t.position));
		}

		// Get the roll angle from horizon (how much the vehicle is rotated on its z axis wrt the horizon, regardless of pitch)
		public static float GetRollAngleFromHorizon(Transform vehicleTransform){
			
			Vector3 rollRefVector;
	
			// Project the forward vector onto the world horizontal plane
			rollRefVector = new Vector3(vehicleTransform.forward.x, 0f, vehicleTransform.forward.z);
	
			// Rotate it 90 degrees
			rollRefVector = Quaternion.AngleAxis(90, Vector3.up) * rollRefVector;
	
			
			float rollAngle = Vector3.Angle(vehicleTransform.right, rollRefVector);
	
			if ((vehicleTransform.right - rollRefVector).y < 0f) rollAngle *= -1;
			
			// Now get the angle
			return (rollAngle);
	
		}	
	}
}
