using Microsoft.AspNetCore.Mvc;

using SendEmail.Code;
using SendEmail.Models;

using System.Diagnostics;

namespace SendEmail.Controllers {
   public class HomeController : Controller {

      private readonly Ferramentas _ferramenta;
      private readonly ILogger<HomeController> _logger;

      public HomeController(ILogger<HomeController> logger, Ferramentas ferramenta) {
         this._logger = logger;
         this._ferramenta = ferramenta;
      }


      public IActionResult Index() {

         return View();
      }

      [HttpPost]
      public async Task<IActionResult> Index([Bind("Subject,Body,Destinatario")] Email email) {

         if (ModelState.IsValid) {

            // enviar email
            var resposta = await _ferramenta.EnviaEmailAsync(email);

            // escrever no disco rígido os dados do email enviado
            // é óbvio que esta tarefa não é aqui necessária.
            // está aqui apenas para exemplificação
            string texto = "Email enviado em: " + DateTime.Now.ToString() +
                     "\r\n" + "Destinatário: " + email.Destinatario +
                     "\r\n" + "Assunto: " + email.Subject +
                     "\r\n" + "Corpo Email: " + email.Body;

            await _ferramenta.EscreveLogAsync("Home", "Index", texto, "");

            // preparar resposta para o utilizador
            if (resposta == 0) {
               TempData["Mensagem"] = "S#:Email Enviado com sucesso.";
               return RedirectToAction("Index");
            }
            else {
               TempData["Mensagem"] = "E#:Ocorreu um erro com o envio do Email.";
            }
         }

         return View();
      }



      [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
      public IActionResult Error() {
         return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
      }
   }
}