namespace AzureBrasilCloudVaga.ApiService.Models
{
    public static class GraphErrorTranslator
    {

        private static readonly Dictionary<string, string> _translations = new()
        {
            { "Authorization_RequestDenied", "Não foi possível completar esta ação porque sua conta não possui permissão no Microsoft Azure.entre em contato com o administrador da sua organização." },
            { "Authentication_RequestFromNonPremiumTenantOrB2CTenant", "Seu tenant não possui licença Premium para acessar este recurso.Entre em contato com o suporte." }
        };


        public static string GetMessage(string? errorCode,string? graphMesssage)
        {
            if (string.IsNullOrWhiteSpace(errorCode))
                return "Ocorreu um erro inesperado.";

            return _translations.TryGetValue(errorCode, out var message)
                ? message
                : graphMesssage ??"Ocorreu um erro inesperado.";
        }
    }
}
