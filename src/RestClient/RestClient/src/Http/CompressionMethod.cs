namespace ClickView.Extensions.RestClient.Http
{
    public enum CompressionMethod
    {
        /// <summary>
        ///     Do not use compression.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Use the gZip compression-decompression algorithm.
        /// </summary>
        GZip = 1,

        /// <summary>
        ///     Use the deflate compression-decompression algorithm.
        /// </summary>
        Deflate = 2
    }
}