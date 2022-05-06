using System.ComponentModel.DataAnnotations;

namespace SendEmail.Models {
   /// <summary>
   /// Classe para recolher os dados de um Email
   /// </summary>
   public class Email {


      /// <summary>
      /// Endereço de email do destinatário do email
      /// </summary>
      [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
      [EmailAddress(ErrorMessage = "Deve escrever um endereço de email válido...")]
      [Display(Name = "Destinatário")]
      public string Destinatario { get; set; }

      /// <summary>
      /// Assunto do email
      /// </summary>
      [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
      [Display(Name = "Assunto do email")]
      public string Subject { get; set; }

      /// <summary>
      /// Corpo do email
      /// </summary>
      [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
      [Display(Name = "Corpo do email")]
      public string Body { get; set; }

   }
}
