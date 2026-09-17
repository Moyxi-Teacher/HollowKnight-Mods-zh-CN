using Modding;

namespace ConnectionMetadataInjector
{
    /// <summary>
    /// Mod providing the ability to request and read supplemental metadata from custom randomizer items
    /// </summary>
    public class ConnectionMetadataInjector : Mod
    {
        /// <inheritdoc/>
        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        /// <summary>
        /// Parameterless constructor. Set in-game name so it's shorter
        /// </summary>
        public ConnectionMetadataInjector() : base("ConnectionMetadataInjector（连接元数据注入器）") { }
    }
}