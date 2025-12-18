public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;
    
    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    
    public T GetById(int id) => _dbSet.Find(id);
    
    public IEnumerable<T> GetAll() => _dbSet.ToList();
    
    public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        => _dbSet.Where(predicate).ToList();
    
    public void Add(T entity) => _dbSet.Add(entity);
    
    public void Update(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }
    
    public void Remove(T entity) => _dbSet.Remove(entity);
    
    public void SaveChanges() => _context.SaveChanges();
}