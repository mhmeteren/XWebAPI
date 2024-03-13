namespace Entities.UtilityClasses.Minio
{
    public class CustomMinioConfig
    {
        public string Bucket { get; set; }
        public string Endpoint { get; set; }
        public bool SSL { get; set; }
        public AccessKeys FullAccess { get; set; }
        public AccessKeys ReadOnlyAccess { get; set; }
    }
}
