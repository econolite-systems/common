// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Helpers
{
	public record Coordinates(double Latitude, double Longitude)
	{
		private static bool Validate(double latitude, double longitude)
			=> latitude is >= -90 and <= 90
			   && longitude is >= -180 and <= 180
			   && (latitude != default && longitude != default);

		public bool IsValid => Validate(Latitude, Longitude);
	};
}
