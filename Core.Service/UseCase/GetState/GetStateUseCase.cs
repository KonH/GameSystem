using Core.Common.State;
using Core.Service.Extension;
using Core.Service.Repository.State;

namespace Core.Service.UseCase.GetState {
	public sealed class GetStateUseCase<TState> : IUseCase<GetStateRequest, GetStateResponse<TState>>
		where TState : IState {
		readonly IStateRepository<TState> _stateRepository;

		public GetStateUseCase(IStateRepository<TState> stateRepository) {
			_stateRepository = stateRepository;
		}

		public GetStateResponse<TState> Handle(GetStateRequest request) {
			var validateError = Validate(request);
			if ( validateError != null ) {
				return validateError;
			}
			var state = _stateRepository.Get(request.UserId);
			return (state != null) ? Found(state) : NotFound();
		}

		GetStateResponse<TState> Validate(GetStateRequest request) {
			if ( request == null ) {
				return BadRequest("null request");
			}
			return null;
		}

		static GetStateResponse<TState>.BadRequest BadRequest(string description) {
			return new GetStateResponse<TState>.BadRequest(description);
		}

		static GetStateResponse<TState>.NotFound NotFound() {
			return new GetStateResponse<TState>.NotFound();
		}

		static GetStateResponse<TState> Found(TState state) {
			return new GetStateResponse<TState>.Found(state);
		}
	}
}