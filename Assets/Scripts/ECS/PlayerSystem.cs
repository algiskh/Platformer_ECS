using Leopotam.EcsLite;
using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
	public class PlayerSystem : IEcsRunSystem
	{
		public void Run(IEcsSystems systems)
		{
			var world = systems.GetWorld();
			var playerPool = world.GetPool<PlayerComponent>();
			var movePool = world.GetPool<MoveComponent>();
			ref var playerInput = ref world.GetAsSingleton<PlayerInputComponent>();
			var input = playerInput.Move;
			ref var muzzle = ref world.GetAsSingleton<MuzzleComponent>();
			var previousInput = playerInput.Move;


			var borderPool = world.GetPool<BorderComponent>();
			var collisionPool = world.GetPool<CollisionComponent>();

			var filter = world.Filter<PlayerComponent>().Inc<MoveComponent>().End();
			foreach (var entity in filter)
			{
				ref var player = ref playerPool.Get(entity);
				ref var moveComponent = ref movePool.Get(entity);

				var isIdle = input.x == 0;

				if (input.x != 0)
				{
					bool newIsRight = input.x > 0f;
					if (playerInput.IsRight != newIsRight)
					{
						playerInput.IsRight = newIsRight;
						player.Value.FlipMuzzle(!playerInput.IsRight);
						player.Value.Animator.SetFlipX(!playerInput.IsRight);
					}
				}

				if (muzzle.IsFiring)
				{
					if (player.State != PlayerState.Fire || previousInput != input)
					{
						moveComponent.Direction = Vector2.zero;
						SetFire(ref player, ref playerInput);
					}
				}
				else
				{
					moveComponent.Direction = input;

					if ((previousInput != input || player.State != PlayerState.Run) && !isIdle)
					{
						SetRun(ref player, ref playerInput);
					}
					else if (isIdle)
					{
						SetIdle(ref player, playerInput.IsRight);
					}
				}

				//player fire input
				var fireRequestPool = world.GetPool<RequestFireComponent>();
				if (playerInput.IsFiring)
				{
					var fireEntity = world.NewEntity();
					ref var requestFireComponent = ref fireRequestPool.Add(fireEntity);
					requestFireComponent.IsRight = playerInput.IsRight;
				}
			}
		}

		private void SetRun(ref PlayerComponent player, ref PlayerInputComponent playerInput)
		{
			if (player.State != PlayerState.Run)
			{
				player.Value.Animator.SetAnimation("Run");
				player.State = PlayerState.Run;
			}
		}

		private void SetFire(ref PlayerComponent player, ref PlayerInputComponent playerInput)
		{
			if (player.State != PlayerState.Fire)
			{
				player.Value.Animator.SetAnimation("Fire");
				player.State = PlayerState.Fire;
			}
		}

		private void SetIdle(ref PlayerComponent player, bool isRight)
		{
			if (player.State != PlayerState.Idle)
			{
				player.Value.Animator.SetAnimation("Idle");
				player.State = PlayerState.Idle;
			}
		}
	}
}
