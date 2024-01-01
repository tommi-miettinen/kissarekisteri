namespace Kissarekisteri.ErrorHandling
{
    public static class CatShowErrors
    {
        public static Error NotFound { get; } = new Error("Cat show not found", "NotFound");
    }
}
