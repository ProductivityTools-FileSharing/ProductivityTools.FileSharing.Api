namespace ProductivityTools.FileSharing.Api.Model
{
    public class AzureFile
    {
        public string Name { get; set; }
        public int Size { get; set; }

        public DateTime? Created { get; set; }
    }
}
