namespace Osmi.Game;

[RequireComponent(typeof(Collider2D))]
public class EnemyDetector : MonoBehaviour {
	private readonly HashSet<Collider2D> colliders = new();

	public IEnumerable<GameObject> Enemies {
		get {
			_ = colliders.RemoveWhere(c => !IsValidEnemy(c));
			return colliders.Map(c => c.gameObject).Distinct();
		}
	}

	public HealthManager? GetTarget() {
		_ = colliders.RemoveWhere(c => !IsValidEnemy(c));

		foreach (Collider2D collider in colliders.OrderBy(c => (transform.position - c.transform.position).sqrMagnitude)) {
			HealthManager hm = collider.GetComponent<HealthManager>();
			if (!hm.IsInvincible && !Physics2D.Linecast(transform.position, hm.transform.position, Layer.Terrain.GetMask())) {
				return hm;
			}
		}

		return null;
	}

	private void OnTriggerEnter2D(Collider2D otherCollider) {
		if (IsValidEnemy(otherCollider)) {
			_ = colliders.Add(otherCollider);
		}
	}

	private void OnTriggerExit2D(Collider2D otherCollider) =>
		colliders.Remove(otherCollider);

	public static bool IsValidEnemy(GameObject go) => go.layer == (int) Layer.Enemies
		&& go.GetComponent<HealthManager>() is HealthManager hm
		&& IsValidEnemy(hm);

	public static bool IsValidEnemy(Collider2D collider) => !collider.isTrigger && IsValidEnemy(collider.gameObject);

	public static bool IsValidEnemy(HealthManager hm) => hm is { isActiveAndEnabled: true, hp: > 0, isDead: false };
}
