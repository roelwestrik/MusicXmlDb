using System.Xml.Schema;
using System.Xml;

namespace MusicXmlDb.Server.MusicXmlDocuments
{
    internal class MusicXmlValidator : IMusicXmlValidator
    {
        private const long maxFileSize = 5 * 1024 * 1024; // 5 MB
        private static readonly XmlSchemaSet? schemaSet;
        private readonly ILogger<MusicXmlValidator> logger;

        static MusicXmlValidator()
        {
            var xsdPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Schema", "4.1");

            schemaSet = new XmlSchemaSet();
            schemaSet.Add("", Path.Combine(xsdPath, "musicxml.xsd"));
            schemaSet.Add("http://www.w3.org/XML/1998/namespace", Path.Combine(xsdPath, "xml.xsd"));
            schemaSet.Add("http://www.w3.org/1999/xlink", Path.Combine(xsdPath, "xlink.xsd"));
        }

        public MusicXmlValidator(ILogger<MusicXmlValidator> logger)
        {
            this.logger = logger;
        }

        public string Validate(IFormFile formFile)
        {
            if (schemaSet == null)
            {
                throw new InvalidOperationException("Schema has not been loaded.");
            }

            var allowedExtensions = new[] { ".musicxml", ".xml" };
            var extension = Path.GetExtension(formFile.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                throw new MusicXmlValidationException("Invalid file type. Only .xml and .musicxml files are allowed.");
            }

            if (formFile.Length > maxFileSize)
            {
                throw new MusicXmlValidationException("File size exceeds the maximum allowed limit of 5 MB.");
            }

            var settings = new XmlReaderSettings();
            settings.Schemas.Add(schemaSet);
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += ValidationEventHandler;
            settings.DtdProcessing = DtdProcessing.Parse;

            using var stringWriter = new StringWriter();
            using var stream = formFile.OpenReadStream();
            using var xmlReader = XmlReader.Create(stream, settings);
            using var xmlWriter = XmlWriter.Create(stringWriter);

            while (xmlReader.Read())
            {
                xmlWriter.WriteNode(xmlReader, true);
            }

            // Flush the XmlWriter to ensure all data is written
            xmlWriter.Flush();
            return stringWriter.ToString();
        }

        private void ValidationEventHandler(object? sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                logger.LogWarning("XML Validation Warning: {message}", e.Message);
                return;
            }

            throw new MusicXmlValidationException($"XML Validation Error: {e.Message}");
        }
    }
}


