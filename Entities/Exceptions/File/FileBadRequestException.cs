namespace Entities.Exceptions.File
{
    public sealed class FileBadRequestException : BadRequestException
    {
        public FileBadRequestException()
            : base("Invalid file type or size.")
        {
        }
    }
}
