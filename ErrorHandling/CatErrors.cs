namespace Kissarekisteri.ErrorHandling
{
    public static class CatErrors
    {
        public static Error NotFound { get; } = new Error("Cat not found", "NotFound");
        public static Error PhotoUploadError { get; } = new Error("Error uploading a photo", "NotFound");
    }
}
