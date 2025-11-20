using src.Repositories.Interfaces;

using ZXing;
using ZXing.Common;
using ZXing.ImageSharp;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;

using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using System.IO;



namespace src.External;

public class ZxIngBarcodeAdapter : IBarcodeAdapter
{
    public ZxIngBarcodeAdapter()
    {
    }

    public async Task<string> GenerarBase64CodigoBarraGS1EAN13Async(
        string codigo,
        int width = 300,
        int height = 150,
        int margin = 2,
        bool pureBarcode = false,
        int? fontSize = null
    )
    {
        if (!ValidarFormatoGS1EAN13Async(codigo))
            throw new ArgumentException("El código proporcionado no es válido según el estándar GS1 EAN-13.");

        if (width < 30 || width > 500)
            throw new ArgumentException($"El ancho (width) debe estar entre 30 y 500 píxeles. Valor actual: {width}.");

        if (height < 20 || height > 300)
            throw new ArgumentException($"La altura (height) debe estar entre 20 y 300 píxeles. Valor actual: {height}.");

        if (margin < 0 || margin > 50)
            throw new ArgumentException($"El margen (margin) debe ser un valor positivo, entre 0 y 50 píxeles. Valor actual: {margin}.");

        if (fontSize < 0 || fontSize > 20)
            throw new ArgumentException($"El tamaño de fuente (fontSize) debe ser positivo y no exceder los 20 puntos. Valor actual: {fontSize}.");

        var pixelData = CrearPixelData(codigo, width, height, margin, pureBarcode);

        string codigoFormateado = FormatearEAN13(codigo);

        using var barcodeImage = Image.LoadPixelData<Rgba32>(pixelData.Pixels, pixelData.Width, pixelData.Height);

        (Font font, TextOptions textOptions) = ObtenerConfiguracionDeFuente(codigo, fontSize, pixelData.Width, 72);

        FontRectangle size = TextMeasurer.MeasureBounds(codigoFormateado, textOptions);

        int extraTextHeight = (int)Math.Ceiling(size.Height + 10);
        int totalHeight = pixelData.Height + extraTextHeight;

        using var finalImage = new Image<Rgba32>(pixelData.Width, totalHeight);
        finalImage.Mutate(ctx => ctx.Fill(Color.White));

        finalImage.Mutate(ctx => ctx.DrawImage(barcodeImage, new Point(0, 0), 1f));

        float centerX = (pixelData.Width - size.Width) / 2;

        float startY = pixelData.Height + 5;
        finalImage.Mutate(ctx =>
        {
            ctx.DrawText(
                codigoFormateado,
                font,
                Color.Black,
                new PointF(centerX, startY)
            );
        });

        using var ms = new MemoryStream();
        finalImage.Save(ms, new PngEncoder());
        return Convert.ToBase64String(ms.ToArray());
    }

    private ZXing.Rendering.PixelData CrearPixelData(string codigo, int width, int height, int margin, bool pureBarcode)
    {
        var writer = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.EAN_13,
            Options = new EncodingOptions
            {
                Height = height,
                Width = width,
                Margin = margin,
                PureBarcode = pureBarcode
            }
        };
        return writer.Write(codigo);
    }

    private string FormatearEAN13(string codigo)
    {
        if (codigo.Length != 13) return codigo;

        string s = codigo;
        s = s.Insert(7, " ");
        s = s.Insert(1, " ");
        return s;
    }

    private (Font font, TextOptions textOptions) ObtenerConfiguracionDeFuente(string codigo, int? fontSize, int anchoImagen, int dpi)
    {
        int finalFontSize = fontSize ?? CalcularTamañoFuenteOptimo(anchoImagen);

        var fontFamily = SystemFonts.Families
            .Where(ff => ff.Name.Contains("Sans", System.StringComparison.OrdinalIgnoreCase)
                        || ff.Name.Contains("DejaVu", System.StringComparison.OrdinalIgnoreCase)
                        || ff.Name.Contains("Liberation", System.StringComparison.OrdinalIgnoreCase))
            .First();

        var font = fontFamily.CreateFont(finalFontSize);

        TextOptions textOptions = new(font)
        {
            Dpi = dpi
        };

        return (font, textOptions);
    }

    private int CalcularTamañoFuenteOptimo(int anchoImagen)
    {
        return (int)Math.Max(12, anchoImagen / 15);
    }
    public bool ValidarFormatoGS1EAN13Async(string codigo)
    {
        if (string.IsNullOrEmpty(codigo) || codigo.Length != 13 || !codigo.All(char.IsDigit))
        {
            return false;
        }

        int sum = 0;

        for (int i = 0; i < 12; i++)
        {
            int d = codigo[i] - '0';
            sum += (i % 2 == 0) ? d : d * 3;
        }

        int check = (10 - (sum % 10)) % 10;
        return check == (codigo[12] - '0');
    }
}