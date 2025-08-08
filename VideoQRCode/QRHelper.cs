//using System.Drawing;
//using ZXing.Common;
//using ZXing;
//using ZXing.BitmapExtensions;

//public static class QrHelper
//{
//    /// <summary>
//    /// Tenta ler um QRCode do Bitmap. Retorna o ZXing.Result ou null se nada encontrado.
//    /// </summary>
//    public static ZXing.Result? DecodeQrFromBitmap(Bitmap bitmap)
//    {
//        if (bitmap == null) return null;

//        // Converte para a fonte de luminância que o ZXing entende
//        var luminance = new BitmapLuminanceSource(bitmap);
//        var binarizer = new HybridBinarizer(luminance);
//        var binaryBitmap = new BinaryBitmap(binarizer);

//        // Leitor (baixo nível)
//        var reader = new MultiFormatReader();

//        // Dicas para melhorar a detecção (ajuste conforme necessário)
//        var hints = new Dictionary<DecodeHintType, object>
//        {
//            { DecodeHintType.TRY_HARDER, true },
//            { DecodeHintType.POSSIBLE_FORMATS, new List<BarcodeFormat> { BarcodeFormat.QR_CODE } }
//        };

//        try
//        {
//            // Use Decode (maiúsculo) — devolve null se não encontrar
//            var result = reader.Decode(binaryBitmap, hints);
//            return result;
//        }
//        catch
//        {
//            // Em caso de exceção, retorna null (não encontrou / erro ao decodificar)
//            return null;
//        }
//    }
//}