using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arcadia.globals
{
	class Constants
	{
		public static List<WeaponHash> SingleAmmoWeapons = new List<WeaponHash>
		{
			WeaponHash.Parachute,
			WeaponHash.Dagger,
			WeaponHash.Bat,
			WeaponHash.Bottle,
			WeaponHash.Crowbar,
			WeaponHash.Flashlight,
			WeaponHash.Golfclub,
			WeaponHash.Hammer,
			WeaponHash.Hatchet,
			WeaponHash.Knuckle,
			WeaponHash.Knife,
			WeaponHash.Machete,
			WeaponHash.Switchblade,
			WeaponHash.Nightstick,
			WeaponHash.Wrench,
			WeaponHash.Battleaxe,
			WeaponHash.Poolcue,
			WeaponHash.Stone_hatchet
		};

		public static List<Weather> BannedWeathers = new List<Weather>
		{
			Weather.NEUTRAL,
			Weather.SNOW,
			Weather.BLIZZARD,
			Weather.SNOWLIGHT,
			Weather.XMAS
		};

		// Colors
		public const string COLOR_ORANGE = "!{#ff8000}";
		public const string COLOR_PRIMARY = "!{#007bff}";
		public const string COLOR_SECONDARY = "!{#6c757d}";
		public const string COLOR_SUCCESS = "!{#28a745}";
		public const string COLOR_DANGER = "!{#dc3545}";
		public const string COLOR_WARNING = "!{#ffc107}";
		public const string COLOR_INFO = "!{#f0e802}";
		public const string COLOR_DARK = "!{#343a40}";
		public const string COLOR_WHITE = "!{#ffffff}";

		//gender
		public const string GENDER_MALE = "Male";
		public const string GENDER_FEMALE = "Female";

		//commands descriptions
		public const string fixveh = "Ремонтирует транспортное средство. Необходимо указать таргет (id игрока), который в свою очередь должен находиться в транспортном средсве.";
	}
}
