using System.IO;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Un Stream Wrapper que delega todas las operaciones al Stream subyacente
/// pero anula (ignora) la llamada a Dispose. Esto se usa para prevenir que
/// librerías externas cierren prematuramente un MemoryStream que se está
/// consumiendo perezosamente (IAsyncEnumerable).
/// </summary>
public class NonClosingStreamWrapper : Stream
{
    private readonly Stream _stream;

    public NonClosingStreamWrapper(Stream stream)
    {
        _stream = stream;
    }

    protected override void Dispose(bool disposing)
    {
    }
    public override ValueTask DisposeAsync()
    {
        return default;
    }

    public override bool CanRead => _stream.CanRead;
    public override bool CanSeek => _stream.CanSeek;
    public override bool CanWrite => _stream.CanWrite;
    public override long Length => _stream.Length;

    public override long Position
    {
        get => _stream.Position;
        set => _stream.Position = value;
    }

    public override void Flush() => _stream.Flush();

    public override int Read(byte[] buffer, int offset, int count) => _stream.Read(buffer, offset, count);
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        => _stream.ReadAsync(buffer, offset, count, cancellationToken);

    public override long Seek(long offset, SeekOrigin origin) => _stream.Seek(offset, origin);
    public override void SetLength(long value) => _stream.SetLength(value);
    public override void Write(byte[] buffer, int offset, int count) => _stream.Write(buffer, offset, count);
}