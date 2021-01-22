using System;

namespace UspsOpenApi.Models
{
    public class UspsOpenApiException : ApplicationException
    {
        public UspsOpenApiException(string ErrorMessage)
        {
            _Exception = new Exception(ErrorMessage);
        }

        public UspsOpenApiException(string ErrorMessage, Exception ex)
        {
            _Exception = new Exception(ErrorMessage, ex);
        }

        public UspsOpenApiException(Exception ex)
        {
            _Exception = ex;
        }

        private readonly Exception _Exception;

        public override string Message
        {
            get { return _Exception.Message; }
        }

        public override string Source
        {
            get { return _Exception.Source; }
            set { _Exception.Source = value; }
        }

        public override string StackTrace
        {
            get { return _Exception.StackTrace; }
        }

        public override System.Collections.IDictionary Data
        {
            get { return _Exception.Data; }
        }

        public override string HelpLink
        {
            get { return _Exception.HelpLink; }
            set { _Exception.HelpLink = value; }
        }
    }
}
