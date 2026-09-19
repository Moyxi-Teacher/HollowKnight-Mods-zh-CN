namespace ExtraNotchesForExtraCharms
{
    public class GlobalSettings
    {
        /// <summary>
        /// 每解锁一个「额外护符」，额外增加的护符槽位数量。
        /// 修改(2026-09-20)：原为「每 N 个额外护符换 1 个护符槽位」（除法），
        /// 现改为「每个已解锁的额外护符增加 N 个槽位」（乘法），默认 5。
        /// </summary>
        public int notchesPerCharm { get; set; } = 5;
    }
}