using SendEmail.Models;

using System.Net;
using System.Net.Mail;


namespace SendEmail.Code {
   /// <summary>
   /// ferramentas de uso genérico na app
   /// </summary>
   public class Ferramentas {

      // não esquecer, registar no Startup.cs
      // o serviço com 'singleton
      //  //criação do objeto que irá providenciar os recursos necessários
      // // genéricos à app
      // // email, logger, etc..
      // services.AddSingleton(typeof(Ferramentas));


      private readonly IConfiguration _configuracao;
      private readonly IWebHostEnvironment _webHostEnvironment;

      public Ferramentas(IConfiguration configuracao,
          IWebHostEnvironment webHostEnvironment) {
         this._configuracao = configuracao;
         this._webHostEnvironment = webHostEnvironment;
      }

      /// <summary>
      /// escreve num ficheiro de Log a ação que deu origem ao erro
      /// </summary>
      /// <param name="nomeController">nome do Controller onde o erro ocorreu</param>
      /// <param name="metodo">nome do Método onde o erro ocorreu</param>
      /// <param name="acao">Ação que deu origem ao erro</param>
      /// <param name="pessoa">Nome do Username que deu origem ao erro</param>
      /// <returns>0 - Log escrito com sucesso; 1 - Log não escrito </returns>
      public async Task<int> EscreveLogAsync(string nomeController, string metodo, string acao, string pessoa) {
         int resultado = 0; // var para exprimir o sucesso da operação de escrever o log: 0 - significa SUCESSO pleno

         // https://docs.microsoft.com/en-us/dotnet/api/system.io.directory.getcurrentdirectory?view=netcore-3.1
         //  string caminho = Directory.GetCurrentDirectory();

         string dataAtual = DateTime.Now.ToString("yyyyMMdd_HHmmss");
         if (!Directory.Exists(Path.Combine(_webHostEnvironment.ContentRootPath, "Logs"))) {
            Directory.CreateDirectory(Path.Combine(_webHostEnvironment.ContentRootPath, "Logs"));
         }
         var caminhoCompleto = Path.Combine(_webHostEnvironment.ContentRootPath, "Logs", dataAtual + ".txt");

         // cria o ficheiro e escreve nele
         var logFile = System.IO.File.Create(caminhoCompleto);
         var logWriter = new System.IO.StreamWriter(logFile);
         await logWriter.WriteLineAsync("Data: " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
         await logWriter.WriteLineAsync("User: " + pessoa);
         await logWriter.WriteLineAsync("Controller: " + nomeController);
         await logWriter.WriteLineAsync("Método: " + metodo);
         await logWriter.WriteLineAsync("Ação executada: " + acao);
         await logWriter.DisposeAsync();

         return resultado;
      }


      /// <summary>
      /// Função para enviar um email
      /// </summary>
      /// <param name="email">objeto com os dados do email (destinatário, subject, body)</param>
      /// <param name="bcc">se 'true' envia cópia do email para o Adminsitrador da app, em 'bcc'</param></param>
      /// <returns>0 - email enviado com sucesso; 1 - email NÃO enviado </returns>
      public async Task<int> EnviaEmailAsync(Email email, bool bcc = false) {

         int resultado = 0; // var para exprimir o sucesso da operação de enviar email: 0 - significa SUCESSO pleno
                            // recupera o endereço de email a utilizar no envio da mensagem

         string emailUserName = _configuracao["AppSettings:Email:SenderEmail"];
         string emailPassword = _configuracao["AppSettings:Email:Password"];
         string emailHost = _configuracao["AppSettings:Email:Host"];
         string emailPort = _configuracao["AppSettings:Email:Port"];


         // envia um email
         using (var client = new SmtpClient()) {
            // quando se utiliza o servidor de email do GOOGLE é necessário baixar a segurança da conta
            // senão o Google bloqueia o envio de emails
            // https://myaccount.google.com/lesssecureapps?pli=1

            try {
               // configurar o serviço de enviar emails
               client.Host = emailHost;
               client.Port = Convert.ToInt32(emailPort);
               client.EnableSsl = true;
               client.Credentials = new NetworkCredential(emailUserName, emailPassword);

               // criar a mensagem
               using (var message = new MailMessage()) {
                  message.From = new MailAddress(emailUserName, "app Envio de Emails");
                  message.To.Add(email.Destinatario);
                  // se necessário, pode ser configurada a opção de enviar emails com CC ou BCC
                  // apesar de referida na assinatura do método, esta opção não está funcional...
                  // if (bcc) { message.Bcc.Add(new MailAddress("endereço de email", "nome a aparecer no email")); }

                  message.Subject = email.Subject;
                  message.Body = email.Body;
                  message.IsBodyHtml = true;

                  // Enviar Email
                  await client.SendMailAsync(message);
               } // fim do 'using' (MailMessage)
            }
            catch (Exception ex) {
               // ocorreu um erro
               string auxErro = "Erro no envio do email.\r\n" + ex.Message + "\r\n\r\nInnerException\r\n" + ex.InnerException + "\r\n\r\nStackTrace\r\n" + ex.StackTrace;
               await EscreveLogAsync("Envio Email", "", auxErro, "");
               resultado = 1;
            } // fim TRY/CATCH

         } // fim do 'using' (SmtpClient)


         return resultado; // significa q enviou com sucesso
      }




   }
}
