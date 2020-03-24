using Core.Service.Model;

namespace Core.Client.Shared {
	public sealed class UserIdSource {
		readonly ISettingsSource _settings;
		readonly UserIdGenerator _idGenerator;

		public UserIdSource(ISettingsSource settings, UserIdGenerator idGenerator) {
			_settings    = settings;
			_idGenerator = idGenerator;
		}

		public UserId GetOrCreateUserId() {
			var settings = _settings.GetLocalSettings();
			if ( string.IsNullOrWhiteSpace(settings.UserId) ) {
				var newId = _idGenerator.GetNewUserId();
				settings.UserId = newId.ToString();
				_settings.UpdateSettings(settings);
				return newId;
			}
			return new UserId(settings.UserId);
		}
	}
}