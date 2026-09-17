namespace Osmi.Game;

// Simulates SendRandomEventV3
[PublicAPI]
public abstract class AbstractRandomSelector<T> {
	public abstract void Add(SelectionItem item);

	public abstract bool Remove(SelectionItem item);

	public abstract void Clear();

	protected abstract IEnumerable<SelectionItem> GetItems();

	public T Select() {
		SelectionItem[] items = GetItems().ToArray();

		if (items.Length == 0) {
			throw new InvalidOperationException("Selection item list is empty");
		}

		float totalWeight = items.Aggregate(0f, (w, i) => w + i.Weight);

		for (int i = 0; i < 100; i++) {
			float rng = UnityEngine.Random.Range(0f, totalWeight);
			SelectionItem? selected = items.FirstOrDefault(i => {
				if (rng < i.Weight) {
					return true;
				}

				rng -= i.Weight;
				return false;
			});

			if (selected == null) {
				continue;
			}

			if (items.LastOrDefault(
				 item => item.Missed.Value >= item.MissedMax && item.MissedMax > 0
			) is SelectionItem missed) {
				items.ForEach(item => {
					item.Combo.Value = 0;
					item.Missed.Value++;
				});

				missed.Combo.Value = 1;
				missed.Missed.Value = 0;
				return missed.Value;
			} else {
				if (selected.Combo.Value >= selected.ComboMax && selected.ComboMax > 0) {
					continue;
				}

				int combo = selected.Combo.Value + 1;
				items.ForEach(item => {
					item.Combo.Value = 0;
					item.Missed.Value++;
				});

				selected.Combo.Value = combo;
				selected.Missed.Value = 0;
				return selected.Value;
			}
		}

		return items[0].Value;
	}

	public sealed record SelectionItem(T Value, float Weight, int ComboMax, int MissedMax) {
		internal Box<int> Combo { get; init; } = 0;
		internal Box<int> Missed { get; init; } = 0;

		public SelectionItem(T value)
			: this(value, 1f) {
		}

		public SelectionItem(T value, float weight)
			: this(value, weight, 0) {
		}

		public SelectionItem(T value, float weight, int comboMax)
			: this(value, weight, comboMax, 0) {
		}
	}
}

[PublicAPI]
public class RandomSelector<T> : AbstractRandomSelector<T>, IEnumerable<AbstractRandomSelector<T>.SelectionItem> {
	private readonly List<SelectionItem> items = new();

	public override void Add(SelectionItem item) => items.Add(item);

	public override bool Remove(SelectionItem item) => items.Remove(item);

	public override void Clear() => items.Clear();

	protected override IEnumerable<SelectionItem> GetItems() => items;

	public IEnumerator<SelectionItem> GetEnumerator() => items.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
}

[PublicAPI]
public class DynamicRandomSelector<T> : AbstractRandomSelector<T>, IEnumerable<DynamicRandomSelector<T>.DynamicSelectionItem> {
	private readonly List<DynamicSelectionItem> items = new();

	public override void Add(SelectionItem item) => items.Add(new(item));

	public override bool Remove(SelectionItem item) => false;

	public void Add(DynamicSelectionItem item) => items.Add(item);

	public bool Remove(DynamicSelectionItem item) => items.Remove(item);

	public override void Clear() => items.Clear();

	protected override IEnumerable<SelectionItem> GetItems() => items.Map(i => i.Pin());

	public sealed record DynamicSelectionItem(T Value, Func<float> Weight, Func<int> ComboMax, Func<int> MissedMax) {
		internal Box<int> Combo { get; init; } = 0;
		internal Box<int> Missed { get; init; } = 0;

		public DynamicSelectionItem(T value)
			: this(value, Funcs.Constant(1f)) {
		}

		public DynamicSelectionItem(T value, Func<float> weight)
			: this(value, weight, Funcs.Constant(0)) {
		}

		public DynamicSelectionItem(T value, Func<float> weight, Func<int> comboMax)
			: this(value, weight, comboMax, Funcs.Constant(0)) {
		}

		public DynamicSelectionItem(SelectionItem item)
			: this(item.Value, Funcs.Constant(item.Weight), Funcs.Constant(item.ComboMax), Funcs.Constant(item.MissedMax)) {
			Combo = item.Combo;
			Missed = item.Missed;
		}

		public SelectionItem Pin() => new(Value, Weight(), ComboMax(), MissedMax()) {
			Combo = Combo,
			Missed = Missed
		};
	}

	public IEnumerator<DynamicSelectionItem> GetEnumerator() => items.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
}
