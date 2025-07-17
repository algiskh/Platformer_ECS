﻿using Leopotam.EcsLite;
using UnityEngine;

namespace ECS
{
	public class BulletSystem : IEcsRunSystem
	{
		public void Run(IEcsSystems systems)
		{
			var world = systems.GetWorld();

			#region GettingPoolsAndSingletons
			var bulletPool = world.GetPool<BulletComponent>();
			var requestFirePool = world.GetPool<RequestFireComponent>();
			var movePool = world.GetPool<MoveComponent>();
			var collisionPool = world.GetPool<CollisionComponent>();
			ref var muzzle = ref world.GetAsSingleton<MuzzleComponent>();
			ref var bulletPoolPool = ref world.GetAsSingleton<BulletPoolComponent>();
			var soundHolder = world.GetAsSingleton<SoundHolderComponent>();
			#endregion

			// Check disposed bullets and return them to the pool
			var disposedFilter = world.Filter<BulletComponent>().Inc<MoveComponent>().End();
			foreach (var bulletEntity in disposedFilter)
			{
				ref var bullet = ref bulletPool.Get(bulletEntity);
				if (bullet.IsDisposed || bullet.LifeTime <= 0)
				{
					bullet.Bullet.gameObject.SetActive(false);
					bulletPoolPool.Value.Push(bullet.Bullet);
					world.DelEntity(bulletEntity); // delete entity
				}
				else
				{
					bullet.LifeTime -= Time.deltaTime;
				}
			}

			var isCoolDownPassed = Time.time >= muzzle.PrevFireTime + muzzle.GunConfig.FireCoolDown;

			// Handle fire requests
			var fireFilter = world.Filter<RequestFireComponent>().End();

			if (fireFilter.GetEntitiesCount() == 0 && isCoolDownPassed || muzzle.Count <= 0)
			{
				muzzle.IsFiring = false;
			}
			else
			{
				foreach (var entity in fireFilter)
				{
					if (isCoolDownPassed)
					{
						ref var request = ref requestFirePool.Get(entity);
						Bullet bullet;
						if (bulletPoolPool.Value != null &&
							bulletPoolPool.Value.Count > 0)
						{
							bullet = bulletPoolPool.Value.Pop();
						}
						else
						{
							bullet = Object.Instantiate(
								muzzle.GunConfig.BulletPrefab,
								muzzle.Transform);
						}

						bullet.transform.position = muzzle.Transform.position;
						bullet.gameObject.SetActive(true);
						var bulletEntity = world.NewEntity();
						ref var bulletComponent = ref bulletPool.Add(bulletEntity);
						bulletComponent.Bullet = bullet;
						bulletComponent.Damage = muzzle.GunConfig.BulletDamage;
						bulletComponent.IsDisposed = false;
						bulletComponent.LifeTime = muzzle.GunConfig.BulletLifeTime;
						ref var moveComponent = ref movePool.Add(bulletEntity);
						moveComponent.Direction = new Vector2(request.IsRight ? 1 : -1, 0).normalized;
						moveComponent.Speed = muzzle.GunConfig.BulletSpeed;
						moveComponent.Transform = bullet.transform;
						ref var collisionComponent = ref collisionPool.Add(bulletEntity);
						collisionComponent.CollisionType = CollisionType.Bullet;
						collisionComponent.Radius = muzzle.GunConfig.BulletRadius;
						muzzle.IsFiring = true;
						muzzle.PrevFireTime = Time.time;
						muzzle.Count--;
						var sound = soundHolder.Value.GetClip(muzzle.GunConfig.FireSoundId);
						muzzle.AudioSource.PlayOneShot(sound);
					}
					world.DelEntity(entity);
				}
			}
		}
	}
}