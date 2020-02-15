using Core.Common.State;
using Core.Service.Extension;
using Core.Service.Repository.State;

namespace Core.Service.UseCase.GetState {
	public sealed class GetStateUseCase<TState> : IUseCase<GetStateRequest, GetStateResponse>
		where TState : IState {
		readonly IStateRepository<TState> _stateRepository;

		public GetStateUseCase(IStateRepository<TState> stateRepository) {
			_stateRepository = stateRepository;
		}

		public GetStateResponse Handle(GetStateRequest request) {
			var validateError = Validate(request);
			if ( validateError != null ) {
				return validateError;
			}
			var state = _stateRepository.Get(request.UserId);
			return (state != null) ? Found(state) : NotFound();
		}

		GetStateResponse Validate(GetStateRequest request) {
			if ( request == null ) {
				return BadRequest("null request");
			}
			return null;
		}

		static GetStateResponse.BadRequest BadRequest(string description) {
			return new GetStateResponse.BadRequest(description);
		}

		static GetStateResponse.NotFound NotFound() {
			return new GetStateResponse.NotFound();
		}

		static GetStateResponse Found(TState state) {
			return new GetStateResponse.Found<TState>(state);
		}
	}
}