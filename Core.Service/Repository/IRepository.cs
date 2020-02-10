namespace Core.Service.Repository {
	public interface IRepository<TModel> {
		void Add(string id, TModel model);
		TModel Get(string id);
		void Update(string id, TModel model);
	}
}