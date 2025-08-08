using Microsoft.AspNetCore.Mvc;
using FFMpegCore;
using ZXing;
using System.Drawing;
//using ZXing.ZKWeb;
using ZXing.Windows.Compatibility;
using System.Diagnostics.Contracts;


namespace VideoQRCode.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Video : ControllerBase
    {

        [HttpPost("upload")]
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {
            var results = new List<(string Conteudo, string Timestamp)>();
            try
            {
                //salva o arquivo em um diretorio local (na pasta de temps)
                if (file == null || file.Length == 0)
                    return BadRequest("Nenhum arquivo enviado");

                var tempVideoPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".mp4");
                using (var stream = new FileStream(tempVideoPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                //cria outra temp pra guardar os frames (png)
                var tempFramesDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempFramesDir);

                // Extrair frames a cada 1 segundo
                await FFMpegArguments.FromFileInput(tempVideoPath).OutputToFile(Path.Combine(tempFramesDir, "frame_%04d.png"), true, options =>
                    options.WithCustomArgument("-vf fps=1")).ProcessAsynchronously();

                var reader = new BarcodeReader();

                //pega os frames extraidos
                var frames = Directory.GetFiles(tempFramesDir, "*.png");
                int frameIndex = 0;

                // varre a lista de pngs e verificar se tem algum QRCode. Se for diferente de null pega o conteudo do QRCode e o timestamp e add no results
                foreach (var framePath in frames)
                {
                    using (var bitmap = (Bitmap)Image.FromFile(framePath))
                    {
                        var result = reader.Decode(bitmap);
                        if (result != null)
                        {
                            var timestamp = TimeSpan.FromSeconds(frameIndex).ToString(@"hh\:mm\:ss");
                            results.Add((result.Text, timestamp));
                        }
                    }
                    frameIndex++;
                }

                // Salvar resultados em TXT
                var txtPath = Path.Combine(Path.GetTempPath(), "qrcodes_detectados.txt");
                using (var writer = new StreamWriter(txtPath))
                {
                    foreach (var item in results)
                        writer.WriteLine($"{item.Timestamp} - {item.Conteudo}");
                }

                // Retornar caminho do TXT
                return Ok(new
                {
                    Mensagem = "Processamento concluído",
                    Resultado = results,
                    ArquivoTxt = txtPath
                });
            }
            catch (Exception e)
            {
                return Ok(new
                {
                    Mensagem = e.Message,
                    Resultado = results,
                    ArquivoTxt = string.Empty
                });
            }

        }
    }
}