namespace ECOSOL.API.Constants
{
    public static class FileUploadRules
    {
        public static readonly string[] ExtensoesPermitidas = [".pdf", ".jpg", ".jpeg", ".png"];
        public const long TamanhoMaximoEmBytes = 5 * 1024 * 1024; // 5 MB
    }
}
