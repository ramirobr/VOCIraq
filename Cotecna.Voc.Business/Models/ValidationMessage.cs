using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cotecna.Voc.Business
{
    /// <summary>
    /// Complex object used for sending messages of validation
    /// </summary>
    /// 
    public enum StatusProcess { Success, Error, Warning, GenericWarning, GenericError };

    public class ValidationMessage
    {
        /// <summary>
        /// Object identifier
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// Status Process type 
        /// </summary>
        public StatusProcess Status { get; set; }
        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; }


    }
}
