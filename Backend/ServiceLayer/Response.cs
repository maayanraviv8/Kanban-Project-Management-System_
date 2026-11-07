using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class Response
    {
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ErrorMessage { get; set; }

        public Object? ReturnValue { get; set; }
        public Response() { }

        public bool isError() {
            if(ErrorMessage != null)
                return true;
            return false;
            }
        public Response(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
        public Response (string errorMessage, Object objValue)
        {
            ErrorMessage = errorMessage;
            ReturnValue = objValue;
        }
        public Response( Object objValue)
        {
            ReturnValue = objValue;
        }

    }
}
