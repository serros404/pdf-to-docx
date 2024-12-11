using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace PdfToDocxConverter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfToDocxController : ControllerBase
    {
        private readonly string _uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        public PdfToDocxController()
        {
            if (!Directory.Exists(_uploadDirectory))
            {
                Directory.CreateDirectory(_uploadDirectory);
            }
        }

        [HttpPost("convert")]
        public async Task<IActionResult> ConvertPdfToDocx(IFormFile file)
        {
            // Verifica se o arquivo foi enviado
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var filePath = Path.Combine(_uploadDirectory, file.FileName);
            var docxFilePath = Path.Combine(_uploadDirectory, Path.GetFileNameWithoutExtension(file.FileName) + ".docx");

            // Salva o arquivo PDF enviado
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Converte o arquivo PDF para DOCX usando LibreOffice
            try
            {
                ProcessStartInfo pro = new ProcessStartInfo
                {
                    FileName = @"C:\Program Files\LibreOffice\program\soffice.exe", // Caminho para o LibreOffice
                    Arguments = $"--headless --convert-to docx {filePath} --outdir {_uploadDirectory}",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process? process = Process.Start(pro);
                process?.WaitForExit();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error during conversion: {ex.Message}");
            }

            // Retorna o arquivo DOCX convertido
            var docxFile = System.IO.File.OpenRead(docxFilePath);
            return File(docxFile, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", Path.GetFileName(docxFilePath));
        }
    }
}
