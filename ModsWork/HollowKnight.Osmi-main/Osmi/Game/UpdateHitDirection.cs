using Osmi.ChildRefs;

namespace Osmi.Game;

[PublicAPI]
[RequireComponent(typeof(DamageEnemies))]
public sealed class UpdateHitDirection : MonoBehaviour {
	[ChildRef] private DamageEnemies de = null!;

	public float offset = 0f;

	private void Start() => this.InitChildRefs();

	private void FixedUpdate() =>
		de.direction = gameObject.transform.rotation.z + offset;
}
