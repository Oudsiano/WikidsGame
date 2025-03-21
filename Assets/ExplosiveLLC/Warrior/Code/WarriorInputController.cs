﻿using UnityEngine;

namespace WarriorAnims
{
	public class WarriorInputController:MonoBehaviour
	{
		[HideInInspector] public bool inputAiming;
		[HideInInspector] public float inputAimHorizontal = 0;
		[HideInInspector] public float inputAimVertical = 0;
		[HideInInspector] public bool inputAttack;
		[HideInInspector] public bool inputAttackMove;
		[HideInInspector] public bool inputAttackRanged;
		[HideInInspector] public bool inputAttackSpecial;
		[HideInInspector] public float inputBlockTarget = 0;
		[HideInInspector] public bool inputDeath;
		[HideInInspector] public bool inputJump;
		[HideInInspector] public bool inputLightHit;
		[HideInInspector] public bool inputRoll;
		[HideInInspector] public bool inputTarget;
		[HideInInspector] public float inputHorizontal = 0;
		[HideInInspector] public float inputVertical = 0;

		public Vector3 moveInput { get { return CameraRelativeInput(inputHorizontal, inputVertical); } }
		public Vector2 aimInput { get { return CameraRelativeInput(inputAimHorizontal, inputAimVertical); } }

		private void Update()
		{
			Inputs();
			Toggles();
		}

		/// <summary>
		/// Input abstraction for easier asset updates using outside control schemes.
		/// </summary>
		private void Inputs()
		{
			inputAiming = Input.GetButton("Aiming");
			inputAimHorizontal = Input.GetAxisRaw("AimHorizontal");
			inputAimVertical = Input.GetAxisRaw("AimVertical");
			inputAttack = Input.GetButtonDown("Attack");
			inputAttackMove = Input.GetButtonDown("AttackMove");
			inputAttackRanged = Input.GetButtonDown("AttackRanged");
			inputAttackSpecial = Input.GetButtonDown("AttackSpecial");
			inputBlockTarget = Input.GetAxisRaw("BlockTarget");
			inputDeath = Input.GetButtonDown("Death");
			inputJump = Input.GetButtonDown("Jump");
			inputLightHit = Input.GetButtonDown("LightHit");
			inputTarget = Input.GetButton("Target");
			inputHorizontal = Input.GetAxisRaw("Horizontal");
			inputVertical = Input.GetAxisRaw("Vertical");
		}

		private void Toggles()
		{
			// Slow time toggle.
			if (Input.GetKeyDown(KeyCode.T)) {
				if (Time.timeScale != 1) { Time.timeScale = 1; }
				else { Time.timeScale = 0.125f; }
			}
			// Pause toggle.
			if (Input.GetKeyDown(KeyCode.P)) {
				if (Time.timeScale != 1) { Time.timeScale = 1; }
				else { Time.timeScale = 0f; }
			}
		}

		public bool hasBlockInput
		{
			get { return inputBlockTarget > 0.2; }
		}

		public bool hasTargetInput
		{
			get { return inputBlockTarget < -0.2 || inputTarget; }
		}

		/// <summary>
		/// Movement based off camera facing.
		/// </summary>
		private Vector3 CameraRelativeInput(float inputX, float inputZ)
		{
			// Forward vector relative to the camera along the x-z plane.
			Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
			forward.y = 0;
			forward = forward.normalized;

			// Right vector relative to the camera always orthogonal to the forward vector.
			Vector3 right = new Vector3(forward.z, 0, -forward.x);
			Vector3 relativeVelocity = inputHorizontal * right + inputVertical * forward;

			// Reduce input for diagonal movement.
			if (relativeVelocity.magnitude > 1) { relativeVelocity.Normalize(); }

			return relativeVelocity;
		}
	}
}
