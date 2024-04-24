using Entities.UtilityClasses.FileTransactions;

namespace Services.Contracts
{
    public interface IFileUploadService
    {

        Task<string> Upload(FileUpload fileUpload);
        Task Delete(string path);

    }
}
