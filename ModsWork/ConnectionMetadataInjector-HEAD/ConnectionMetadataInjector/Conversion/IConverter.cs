namespace ConnectionMetadataInjector.Conversion
{
    /// <summary>
    /// A converter between 2 types
    /// </summary>
    /// <typeparam name="TIn">The source type for conversion</typeparam>
    /// <typeparam name="TOut">The target type for conversion</typeparam>
    public interface IConverter<in TIn, out TOut>
    {
        /// <summary>
        /// Converts a value of the input type to a value of the output type.
        /// </summary>
        /// <param name="value">The value to convert</param>
        public TOut Convert(TIn value);
    }
}
