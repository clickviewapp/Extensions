namespace ClickView.Extensions.RestClient.Http
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class CompressedContent : HttpContent
    {
        private readonly CompressionMethod _compressionMethod;
        private readonly HttpContent _originalContent;

        public CompressedContent(HttpContent content, CompressionMethod compressionMethod)
        {
            if (compressionMethod == CompressionMethod.None) throw new ArgumentNullException(nameof(compressionMethod));

            _originalContent = content ?? throw new ArgumentNullException(nameof(content));
            _compressionMethod = compressionMethod;

            foreach (var header in _originalContent.Headers) Headers.TryAddWithoutValidation(header.Key, header.Value);

            Headers.ContentEncoding.Add(_compressionMethod.ToString().ToLowerInvariant());
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;

            return false;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            Stream compressedStream;

            switch (_compressionMethod)
            {
                case CompressionMethod.GZip:
                    compressedStream = new GZipStream(stream, CompressionMode.Compress, true);
                    break;
                case CompressionMethod.Deflate:
                    compressedStream = new DeflateStream(stream, CompressionMode.Compress, true);
                    break;
                case CompressionMethod.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return _originalContent.CopyToAsync(compressedStream, context)
                .ContinueWith(_ => compressedStream?.Dispose());
        }
    }
}
