
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Cotecna.Voc.Web.Common
{

    public class RegexUtilities
    {
       bool invalid = false;

        /// <summary>
        /// Validate if the string has Cotecna Password Format
        /// </summary>
        /// <param name="pass"></param>
        /// <returns></returns>
       public bool IsValidCotecnaPasswordFormat(string pass)
       {
        
           // Return true if strIn is in valid password format.
           try
           {
               string stringValidador = "((?=.*\\d)(?=.*[@#$% \\[\\],\\+\\?~\\{\\}\\+\\.!&*'+/=^_`|-¡¿;<>°\\(\\)\\\\:]).{8,20})";
               Regex regEx = new Regex(stringValidador);
               bool answer = regEx.IsMatch(pass);
               return answer;
           }
           catch (RegexMatchTimeoutException)
           {
               return false;
           }
           catch (Exception)
           {
               return false;
           }

       }

        /// <summary>
        /// Validate if the string has email format
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
       public bool IsValidEmail(string strIn)
       {
           invalid = false;
           if (String.IsNullOrEmpty(strIn))
              return false;

           // Use IdnMapping class to convert Unicode domain names.
           try {
              strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper,
                                    RegexOptions.None, TimeSpan.FromMilliseconds(200));
           }
           catch (RegexMatchTimeoutException) {
             return false;
           }

           if (invalid) 
              return false;

           // Return true if strIn is in valid e-mail format.
           try {
              return Regex.IsMatch(strIn, 
                    @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" + 
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$", 
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
           }  
           catch (RegexMatchTimeoutException) {
              return false;
           }
       }

       private string DomainMapper(Match match)
       {
          // IdnMapping class with default property values.
          IdnMapping idn = new IdnMapping();

          string domainName = match.Groups[2].Value;
          try {
             domainName = idn.GetAscii(domainName);
          }
          catch (ArgumentException) {
             invalid = true;      
          }      
          return match.Groups[1].Value + domainName;
       }
    }
}