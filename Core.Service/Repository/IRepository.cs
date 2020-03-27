using System.Threading.Tasks;

namespace Core.Service.Repository {
	public interface IRepository<TModel> {
		Task Add(string id, TModel model);
		Task<TModel> Get(string id);
		Task Update(string id, TModel model);
	}
}