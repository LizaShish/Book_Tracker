namespace Book_Tracker.Interface
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();  //метод для получения всех записей.
        T GetById(int id); //метод для получения записи по идентификатору.
        void Add(T entity); //метод для добавления новой записи
        void Update(T entity); //метод для обновления существующей записи.
        void Delete(T entity); //метод для удаления записи.
        void Save(); //метод для сохранения изменений в базе данных.

    }
}
