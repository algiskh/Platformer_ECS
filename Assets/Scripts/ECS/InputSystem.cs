using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.InputSystem;


namespace ECS
{
	public class InputSystem : IEcsInitSystem, IEcsRunSystem
	{
		public void Init(IEcsSystems systems)
		{
			var world = systems.GetWorld();
			ref var playerInput = ref world.GetAsSingleton<PlayerInputComponent>();
			playerInput.IsRight = true;
		}

		public void Run(IEcsSystems systems)
		{
			var world = systems.GetWorld();
			ref var input = ref world.GetAsSingleton<PlayerInputComponent>();
			var borderPool = world.GetPool<BorderComponent>();
			var collisionPool = world.GetPool<CollisionComponent>();
			var player = world.GetAsSingleton<PlayerComponent>();
			var playerPos = player.Value.transform.position;

			input.PreviousMove = input.Move;

			input.Move = new Vector2(Input.GetAxisRaw("Horizontal"), 0f);
			input.IsFiring = Input.GetButton("Fire1");


			var borderFilter = world.Filter<BorderComponent>().Inc<CollisionComponent>().End();
			foreach (var borderEntity in borderFilter)
			{
				ref var border = ref borderPool.Get(borderEntity);
				ref var borderCollision = ref collisionPool.Get(borderEntity);
				Vector2 borderPos = border.Transform.position;

				if (!border.IsPlayerNearBy)
					continue;
				if (playerPos.x < borderPos.x && input.Move.x > 0)
					input.Move.x = 0;

				if (playerPos.x > borderPos.x && input.Move.x < 0)
					input.Move.x = 0;
			}
		}
	}
}