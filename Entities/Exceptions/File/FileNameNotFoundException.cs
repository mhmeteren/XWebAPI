namespace Entities.Exceptions.File
{
    public class FileNameNotFoundException : NotFoundException
    {
        public FileNameNotFoundException()
            : base("File not found.")
        {
        }
    }
}
