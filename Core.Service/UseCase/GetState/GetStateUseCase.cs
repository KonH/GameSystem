using System.Threading.Tasks;
using Core.Common.State;
using Core.Service.Extension;
using Core.Service.Repository.State;

namespace Core.Service.UseCase.GetState {
	public sealed class GetStateUseCase<TState> : IUseCase<GetStateRequest, GetStateResponse>
		where TState : IState, new() {
		readonly IStateRepository<TState> _stateRepository;

		public GetStateUseCase(IStateRepository<TState> stateRepository) {
			_stateRepository = stateRepository;
		}

		public async Task<GetStateResponse> Handle(GetStateRequest request) {
			var validateError = Validate(request);
			if ( validateError != null ) {
				return validateError;
			}
			var state = await _stateRepository.Get(request.UserId);
			if ( state == null ) {
				state = CreateState();
				await _stateRepository.Add(request.UserId, state);
			}
			return Found(state);
		}

		GetStateResponse Validate(GetStateRequest request) {
			if ( request == null ) {
				return BadRequest("null request");
			}
			return null;
		}

		TState CreateState() => new TState();

		static GetStateResponse.BadRequest BadRequest(string description) {
			return new GetStateResponse.BadRequest(description);
		}

		static GetStateResponse Found(TState state) {
			return new GetStateResponse.Found<TState>(state);
		}
	}
}