using System;
using System.Net;
using System.Net.Mail;

namespace RedSismica.Models;

public static class InterfazEmail
{
    public static void EnviarEmails(string destinatario, string asunto, string cuerpo)
        {
            const string remitente = "redsismicadsi@gmail.com"; //no tocar
            const string password = "bzpr pexq nggv tpby";   // no tocar
    
            var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(remitente, password),
                EnableSsl = true,
            };
    
            var mensaje = new MailMessage(remitente, destinatario, asunto, cuerpo);
    
            try
            {
                smtp.Send(mensaje);
                Console.WriteLine("Correo enviado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo: {ex.Message}");
            }
        }
    
}