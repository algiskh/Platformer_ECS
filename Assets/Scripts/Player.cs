using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField] private SimpleAnimator _animator;
	[SerializeField] private Transform _muzzle;
	[SerializeField] private AudioSource _audioSource;
	private Vector2 _muzzleOriginal;
	public SimpleAnimator Animator => _animator;
	public Transform Muzzle => _muzzle;
	public AudioSource AudioSource => _audioSource;

	private void Awake()
	{
		_muzzleOriginal = _muzzle.localPosition;
	}

	public void FlipMuzzle(bool flip)
	{
		if (_muzzle != null)
		{
			var flipX = flip ? -1 : 1;
			_muzzle.localPosition = new Vector3(_muzzleOriginal.x * flipX, _muzzleOriginal.y);
		}
	}
}
