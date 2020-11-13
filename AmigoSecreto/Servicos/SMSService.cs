using RestSharp;
using System.Configuration;

namespace AmigoSecreto.Servicos
{
    public static class SMSService
    {
        private static string _emProducao = ConfigurationManager.AppSettings["Producao"];

        public static string SendSMS(string nTelemovel, string mensagem)
        {
            if (mensagem == null || mensagem.Length <= 0) return "A mensagem a enviar está vazia";

            if (nTelemovel == null || nTelemovel.Length <= 0) return "Número de telemóvel não indicado";

            if (nTelemovel.Length != 9) return "Número de telemóvel inválido: " + nTelemovel;

            if (IsSistemaEmProducao())
            {
                var client = new RestClient("https://http-api.d7networks.com/send?username=********&password=*********&dlr-method=POST&dlr-url=https://4ba60af1.ngrok.io/receive&dlr=yes&dlr-level=3&from=AmigoSecret&content=" + mensagem + "&to=+351" + nTelemovel);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AlwaysMultipartFormData = true;
                IRestResponse response = client.Execute(request);

                return (response.Content);
            }
            else
                return mensagem;
        }

        public static bool IsSistemaEmProducao()
        {
            if (_emProducao == null) return false;

            if (_emProducao.Contains("true") || _emProducao.Contains("True"))
                return true;
            else
                return false;
        }

    }
}