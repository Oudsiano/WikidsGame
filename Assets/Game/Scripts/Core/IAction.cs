namespace RPG.Core
{
    // Интерфейс IAction представляет действие, которое может быть отменено.
    public interface IAction
    {
        // Метод Cancel() предназначен для отмены текущего действия.
        void Cancel();
    }
}
