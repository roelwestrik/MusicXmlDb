namespace MusicXmlDb.Server.MusicXmlDocuments
{
    public interface IMusicXmlValidator
    {
        string Validate(IFormFile formFile);
    }
}


