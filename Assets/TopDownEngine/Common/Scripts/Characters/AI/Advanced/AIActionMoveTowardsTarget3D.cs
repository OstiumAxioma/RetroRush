﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.AI;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Requires a CharacterMovement ability. Makes the character move up to the specified MinimumDistance in the direction of the target. 
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionMoveTowardsTarget3D")]
	//[RequireComponent(typeof(CharacterMovement))]
	public class AIActionMoveTowardsTarget3D : AIAction
	{
		/// the minimum distance from the target this Character can reach.
		[Tooltip("the minimum distance from the target this Character can reach.")]
		public float MinimumDistance = 1f;

		protected Vector3 _directionToTarget;
		protected CharacterMovement _characterMovement;
		protected int _numberOfJumps = 0;
		protected Vector2 _movementVector;

		private NavMeshAgent _agent;

		/// <summary>
		/// On init we grab our CharacterMovement ability
		/// </summary>
		public override void Initialization()
		{
			if(!ShouldInitialize) return;
			base.Initialization();
            _agent = GetComponent<NavMeshAgent>();
            _characterMovement = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterMovement>();
		}

		/// <summary>
		/// On PerformAction we move
		/// </summary>
		public override void PerformAction()
		{
            Move();
            AI_Move();
        }

		void AI_Move() {
            if (_brain.Target == null)
            {
                return;
            }
            //_agent.speed = 2.5f;
            //_agent.SetDestination(_brain.Target.position);
		}

		/// <summary>
		/// Moves the character towards the target if needed
		/// </summary>
		protected virtual void Move()
		{
			if (_brain.Target == null)
			{
				return;
			}
            
			_directionToTarget = _brain.Target.position - this.transform.position;
			_movementVector.x = _directionToTarget.x / 100;
			_movementVector.y = _directionToTarget.z / 100;
			_characterMovement.SetMovement(_movementVector);


			if (Mathf.Abs(this.transform.position.x - _brain.Target.position.x) < MinimumDistance)
			{
				_characterMovement.SetHorizontalMovement(0f);
			}

			if (Mathf.Abs(this.transform.position.z - _brain.Target.position.z) < MinimumDistance)
			{
				_characterMovement.SetVerticalMovement(0f);
			}
		}

		/// <summary>
		/// On exit state we stop our movement
		/// </summary>
		public override void OnExitState()
		{
			base.OnExitState();

			_characterMovement?.SetHorizontalMovement(0f);
			_characterMovement?.SetVerticalMovement(0f);
		}
	}
}