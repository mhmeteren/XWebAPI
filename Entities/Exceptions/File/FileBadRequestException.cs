namespace Entities.Exceptions.File
{
    public sealed class FileBadRequestException : BadRequestException
    {
        public FileBadRequestException()
            : base("Geçersiz dosya türü veya boyutu.")
        {
        }
    }
}
