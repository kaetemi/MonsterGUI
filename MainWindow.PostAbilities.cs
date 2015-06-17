using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using SimpleJSON;

namespace MonsterGUI
{
	public partial class MainWindow
	{
		// Switched by the callbacks from the checkboxes in the GUI
		volatile bool autoClickerOn = false;
		volatile bool laneSwitcherOn = true;
		volatile bool targetSpawnersOn = true;
		volatile bool elementSwitcherOn = true;
		volatile bool goldLaneSwitcherOn = true;
		volatile bool bossLaneOn = true;
		volatile bool respawnerOn = true;
		volatile bool supportAbilitiesOn = false;
		volatile bool offensiveAbilitiesOn = false;
		volatile bool itemAbilitiesOn = false;
		volatile bool superWormholeOn = false;
		volatile bool multiWormholeOn = false;

		// Strategy control
		int speedThreshold_wchill = 2000;
		int rainingRounds_wchill = 100;
		int speedThreshold_steamdb = 1000;
		int rainingRounds_steamdb = 250;
		int wormHoleRounds = 500;
		int superWormholeRounds = 100;
		int superWormHoleDamageSafety = 5;
		int likeNewTimerMin = 400;
		int likeNewTimerMax = 4000;
		int likeNewChance = 20; // likeNewChance / NB Wormholes remaining
		int multiWormholeCount = 10;

		// Auto clicker runtime info
		long clickCount = 0;
		long addClicks = 0;
		int minClicks = 16;
		int maxClicks = 20;
		int clickBoost = 1;

		// Timed Lane Switcher runtime info
		int laneSwitcherTime = 1;
		int laneSwitcherTimeCounter = 0;

		// Lane Switcher runtime info
		int laneRequested = 0;

		// GUI info
		volatile bool printRequestedLaneSwitch = false;
		volatile bool printRequestedTargetSwitch = false;
		int printRequestedTarget = 0;

		// Abilities info
		volatile bool triggerHappy = false;
		long lastGoldRainLevel = 0;
		long lastBombLevel = 0;
		int rearmLikeNewAt = 0;
		long lastWormholeLevel = 0;

		/// <summary>
		/// App init
		/// </summary>
		private void postAbilitiesInit()
		{
			resultPostAbilitiesDelegate = new JsonCallback(resultPostAbilities);
			printRequestedLaneTargetDelegate = new EmptyCallback(printRequestedLaneTarget);
			autoClickerCheck.Checked = autoClickerOn;
			laneSwitcherCheck.Checked = laneSwitcherOn;
			elementSwitcherCheck.Checked = elementSwitcherOn;
			targetSpawnerCheck.Checked = targetSpawnersOn;
			goldLaneCheck.Checked = goldLaneSwitcherOn;
			bossLaneCheck.Checked = bossLaneOn;
			respawnerCheck.Checked = respawnerOn;
			supportAbilitiesCheck.Checked = supportAbilitiesOn;
			ovenzifCheck.Checked = offensiveAbilitiesOn;
			itemsCheck.Checked = itemAbilitiesOn;
			fireImmediatelyCheck.Checked = triggerHappy;
			fasterWormhole.Checked = superWormholeOn;
			multiWhCheck.Checked = multiWormholeOn;
		}

		/// <summary>
		/// User GO
		/// </summary>
		private void postAbilitiesGo()
		{
			clickCount = 0;
			clicksText.Text = "0";
			lastGoldRainLevel = 0;
			rearmLikeNewAt = 0;
		}

		EmptyCallback printRequestedLaneTargetDelegate;
		void printRequestedLaneTarget()
		{
			requestedLaneText.Enabled = printRequestedLaneSwitch;
			if (printRequestedLaneSwitch)
				requestedLaneText.Text = laneNumbers[laneRequested];
			requestedTargetText.Enabled = printRequestedTargetSwitch;
			if (printRequestedTargetSwitch)
				requestedTargetText.Text = targetNumbers[printRequestedTarget];
		}

		private JsonCallback resultPostAbilitiesDelegate;
		/// <summary>
		/// Success response from server
		/// </summary>
		/// <param name="json"></param>
		private void resultPostAbilities(JSONNode json)
		{
			JSONNode response = json["response"];
			if (response == null)
				return;
			JSONNode playerData = response["player_data"];
			if (playerData == null)
				return;
			clickCount += (long)addClicks;
			decodePlayerData(playerData);
			clicksText.Text = clickCount.ToString();
		}

		private void printRequestedLane()
		{

		}

		/// <summary>
		/// Check if there are any enemies alive in the specified lane (lane index 0, 1, 2)
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		private bool enemiesAliveInLane(int i)
		{
			for (int j = 0; j < gameData.Lanes[i].Enemies.Length; ++j)
			{
				if (gameData.Lanes[i].Enemies[j].Type != EnemyType.None && gameData.Lanes[i].Enemies[j].Hp != 0)
					return true;
			}
			return false;
		}

		private decimal highestMonsterOnLane(int i)
		{
			decimal highest = 0;
			for (int j = 0; j < gameData.Lanes[i].Enemies.Length; ++j)
			{
				if (gameData.Lanes[i].Enemies[j].Type != EnemyType.None && gameData.Lanes[i].Enemies[j].Hp > highest)
					highest = gameData.Lanes[i].Enemies[j].Hp;
			}
			return highest;
		}

		private int numberMonstersAliveOnLane(int i)
		{
			int count = 0;
			for (int j = 0; j < gameData.Lanes[i].Enemies.Length; ++j)
			{
				if (gameData.Lanes[i].Enemies[j].Type != EnemyType.None && gameData.Lanes[i].Enemies[j].Hp != 0)
					++count;
			}
			return count;
		}

		private bool bossMonsterOnLane(int i)
		{
			for (int j = 0; j < gameData.Lanes[i].Enemies.Length; ++j)
			{
				if (gameData.Lanes[i].Enemies[j].Type == EnemyType.Boss && gameData.Lanes[i].Enemies[j].Hp != 0)
					return true;
			}
			return false;
		}

		private bool treasureMonsterOnLane(int i)
		{
			for (int j = 0; j < gameData.Lanes[i].Enemies.Length; ++j)
			{
				if (gameData.Lanes[i].Enemies[j].Type == EnemyType.Treasure && gameData.Lanes[i].Enemies[j].Hp != 0)
					return true;
			}
			return false;
		}

		private int findSpawnerOnLane(int i)
		{
			for (int j = 0; j < gameData.Lanes[i].Enemies.Length; ++j)
			{
				if (gameData.Lanes[i].Enemies[j].Type == EnemyType.Spawner && gameData.Lanes[i].Enemies[j].Hp != 0)
					return j;
			}
			return -1;
		}

		private int findTreasureOnLane(int i)
		{
			for (int j = 0; j < gameData.Lanes[i].Enemies.Length; ++j)
			{
				if (gameData.Lanes[i].Enemies[j].Type == EnemyType.Treasure && gameData.Lanes[i].Enemies[j].Hp != 0)
					return j;
			}
			return -1;
		}

		private int countLiveMonstersOnLane(int i)
		{
			int count = 0;
			for (int j = 0; j < gameData.Lanes[i].Enemies.Length; ++j)
			{
				if (gameData.Lanes[i].Enemies[j].Type != EnemyType.None && gameData.Lanes[i].Enemies[j].Hp != 0)
					++count;
			}
			return count;
		}

		private decimal highestHpFactorOnLane(int i)
		{
			decimal highest = 0;
			for (int j = 0; j < gameData.Lanes[i].Enemies.Length; ++j)
			{
				if (gameData.Lanes[i].Enemies[j].Type != EnemyType.None && gameData.Lanes[i].Enemies[j].MaxHp != 0 && (gameData.Lanes[i].Enemies[j].Hp / gameData.Lanes[i].Enemies[j].MaxHp) > highest)
					return gameData.Lanes[i].Enemies[j].Hp / gameData.Lanes[i].Enemies[j].MaxHp;
			}
			return highest;
		}

		private bool farmingGoldOnLane(int i)
		{
			if (superWormholeOn)
			{
				return bossMonsterOnLane(i) && !useWormHoleOnLane(i);
			}
			else
			{
				return (
					(gameData.Level < Math.Min(speedThreshold_wchill, speedThreshold_steamdb)
					|| ((gameData.Level % rainingRounds_wchill) == 0)
					|| ((gameData.Level % rainingRounds_steamdb) == 0)
					)

					&& !useWormHoleOnLane(i)

					&& bossMonsterOnLane(i));
			}
		}

		private bool avoidExtraDamageOnLane(int i)
		{
			return farmingGoldOnLane(i) || useWormHoleOnLane(i)
				|| (superWormholeOn && ((gameData.Level % superWormholeRounds) >= (superWormholeRounds - superWormHoleDamageSafety)));
		}
		
		private bool useWormHoleOnLane(int i)
		{
			if (superWormholeOn)
			{
				return ((gameData.Level % superWormholeRounds) == 0)
					&& bossMonsterOnLane(i);
			}
			else
			{
				return ((gameData.Level % wormHoleRounds) == 0)
					&& bossMonsterOnLane(i);
			}
		}

		private bool nukeOnLane(int i)
		{
			if (gameData.Level > Math.Max(speedThreshold_wchill, speedThreshold_steamdb))
			{
				return ((gameData.Level % rainingRounds_wchill) != 0) && ((gameData.Level % rainingRounds_steamdb) != 0) && bossMonsterOnLane(laneRequested)
					&& !avoidExtraDamageOnLane(i);
			}
			else
			{
				return (findSpawnerOnLane(i) >= 0) && ((gameData.Level % 10) > 0) && ((gameData.Level % 10) < 8) && !avoidExtraDamageOnLane(i);
			}
		}

		private bool bombOnLane(int i)
		{
			return !bossMonsterOnLane(laneRequested)
				&& (findSpawnerOnLane(i) >= 0)
				&& (countLiveMonstersOnLane(i) >= 3)
				&& ((gameData.Level % 10) > 0) && ((gameData.Level % 10) < 8)
				&& !avoidExtraDamageOnLane(i);
		}

		private int bestElementLevel()
		{
			int bestLevel = techTree.Upgrades[(int)UpgradeOption.ElementalAir].Level;
			if (techTree.Upgrades[(int)UpgradeOption.ElementalEarth].Level > bestLevel)
				bestLevel = techTree.Upgrades[(int)UpgradeOption.ElementalEarth].Level;
			if (techTree.Upgrades[(int)UpgradeOption.ElementalFire].Level > bestLevel)
				bestLevel = techTree.Upgrades[(int)UpgradeOption.ElementalFire].Level;
			if (techTree.Upgrades[(int)UpgradeOption.ElementalWater].Level > bestLevel)
				bestLevel = techTree.Upgrades[(int)UpgradeOption.ElementalWater].Level;
			return bestLevel;
		}

		/// <summary>
		/// Thread which posts abilities
		/// </summary>
		private void postAbilitiesThread()
		{
			// UseAbilities: {"requested_abilities":[{"ability":2,"new_lane":0},{"ability":4,"new_target":0},{"ability":1,"num_clicks":1}],"gameid":"6059"}
			// 1: Click [num_clicks]
			// 2: Switch Lane [new_lane]
			// 3: Respawn
			// 4: Switch Target [new_target] (NOTE: Server automatically switches targets as well)

			Random random = new Random();
			WebClient wc = new TimeoutWebClient();
			while (running)
			{
				int startTick = System.Environment.TickCount;
				try
				{
					if (string.IsNullOrEmpty(accessToken))
					{
						Console.WriteLine("Access token not set");
						break;
					}

					bool abilities = false;
					string abilties_json = "{\"gameid\":\"" + room + "\",\"requested_abilities\":[";

					if (respawnerOn)
					{
						if (playerData.TimeDied != 0)
						{
							// Respawn
							if (abilities) abilties_json += ",";
							abilties_json += "{\"ability\":3}";
							abilities = true;
						}
					}

					// Before lane switched

					if (laneSwitcherOn) // Timed lane switcher
					{
						laneSwitcherTimeCounter++;
						laneSwitcherTimeCounter %= laneSwitcherTime;

						if (laneSwitcherTimeCounter == 0)
						{
							// ++laneRequested;
							laneRequested += random.Next(1, 3); // incl min, excl max
							laneRequested %= 3;
						}
					}

					bool smartLane = false;
					if (elementSwitcherOn)
					{
						int bestElement = bestElementLevel();
						int originalRes = laneRequested;
						for (int i = 0; i < 3; ++i)
						{
							// Cycle until an appropriate lane found
							if (techTree.Upgrades[(int)gameData.Lanes[laneRequested].Element].Level < bestElement
								|| !enemiesAliveInLane(laneRequested))
							{
								++laneRequested;
								laneRequested %= 3;
							}
							else
							{
								break;
							}
						}
						if (originalRes != laneRequested)
							smartLane = true;
					}

					if (goldLaneSwitcherOn)
					{
						int bestLane = -1;
						decimal bestGold = 0.0m;
						for (int i = 0; i < gameData.Lanes.Length; ++i)
						{
							if (gameData.Lanes[i].ActivePlayerAbilityGoldPerClick > bestGold
								&& enemiesAliveInLane(i))
							{
								bestLane = i;
								bestGold = gameData.Lanes[i].ActivePlayerAbilityGoldPerClick;
							}
						}
						if (bestLane >= 0)
						{
							laneRequested = bestLane;
							smartLane = true;
						}
					}

					if (bossLaneOn)
					{
						for (int i = 0; i < gameData.Lanes.Length; ++i)
						{
							if (treasureMonsterOnLane(i))
							{
								laneRequested = i;
								smartLane = true;
								break;
							}
						}
						for (int i = 0; i < gameData.Lanes.Length; ++i)
						{
							if (bossMonsterOnLane(i))
							{
								laneRequested = i;
								smartLane = true;
								break;
							}
						}
					}

					bool monsterLaneOn = laneSwitcherOn || elementSwitcherOn || goldLaneSwitcherOn || bossLaneOn;
					if (monsterLaneOn && !smartLane) // Already handled by smart lane switches
					{
						int originalRes = laneRequested;
						for (int i = 0; i < 3; ++i)
						{
							if (!enemiesAliveInLane(laneRequested))
							{
								++laneRequested;
								laneRequested %= 3;
							}
							else
							{
								break;
							}
						}
						if (originalRes != laneRequested)
							smartLane = true;
					}

					printRequestedLaneSwitch = false;
					if (laneSwitcherOn || smartLane) // If any lane switching algorithm is enabled
					{
						if (laneRequested != playerData.CurrentLane)
						{
							// Switch lane if requested
							if (abilities) abilties_json += ",";
							abilties_json += "{\"ability\":2,\"new_lane\":" + laneRequested + "}";
							abilities = true;
							printRequestedLaneSwitch = true;
						}
					}
					else
					{
						laneRequested = playerData.CurrentLane;
					}

					// After lane switched
					// NOTE: Target switching is already done by the server so not entirely useful since the monsters die too fast

					printRequestedTargetSwitch = false;
					if (targetSpawnersOn) // Useful for nuking spawners at early levels and treasure
					{
						int preferTarget = findTreasureOnLane(laneRequested);
						if (preferTarget < 0)
							preferTarget = findSpawnerOnLane(laneRequested);
						if (preferTarget >= 0)
						{
							if (preferTarget != playerData.Target)
							{
								if (abilities) abilties_json += ",";
								abilties_json += "{\"ability\":4,\"new_target\":" + preferTarget + "}"; // Only using this for spawners
								abilities = true;
								printRequestedTargetSwitch = true;
								printRequestedTarget = preferTarget;
							}
						}
					}

					BeginInvoke(printRequestedLaneTargetDelegate);

					bool requestTreeRefresh = false;

					if (enemiesAliveInLane(laneRequested) && (triggerHappy || enemiesAliveInLane(playerData.CurrentLane)))
					{
						if (supportAbilitiesOn)
						{
							if (hasPurchasedAbility(Abilities.Medics) && !isAbilityCoolingDown(Abilities.Medics))
							{
								// Medics, always spam them as soon as possible
								if (abilities) abilties_json += ",";
								abilties_json += "{\"ability\":" + (int)Abilities.Medics + "}";
								abilities = true;
							}
							if (triggerHappy || laneRequested == playerData.CurrentLane) // Really sure to work on the current lane
							{
								if (!avoidExtraDamageOnLane(laneRequested)) // Don't do extra damage when farming gold
								{
									if (hasPurchasedAbility(Abilities.MoraleBooster) && !isAbilityCoolingDown(Abilities.MoraleBooster))
									{
										if (highestMonsterOnLane(laneRequested) > 100000000 && numberMonstersAliveOnLane(laneRequested) >= 2)
										{
											if (abilities) abilties_json += ",";
											abilties_json += "{\"ability\":" + (int)Abilities.MoraleBooster + "}"; // More Damage
											abilities = true;
										}
									}
									if (hasPurchasedAbility(Abilities.GoodLuckCharm) && !isAbilityCoolingDown(Abilities.GoodLuckCharm))
									{
										if (abilities) abilties_json += ",";
										abilties_json += "{\"ability\":" + (int)Abilities.GoodLuckCharm + "}"; // IncreaseCritPercentage
										abilities = true;
									}
								}
								else // We're farming gold
								{
									if (hasPurchasedAbility(Abilities.MetalDetector) && !isAbilityCoolingDown(Abilities.MetalDetector))
									{
										if (highestHpFactorOnLane(laneRequested) > 0.75m)
										{
											if (abilities) abilties_json += ",";
											abilties_json += "{\"ability\":" + (int)Abilities.MetalDetector + "}"; // Increase Gold Drops
											abilities = true;
										}
									}
								}
							}
							// Cooldown: DecreaseCooldowns .. ?
						}

						if (offensiveAbilitiesOn)
						{
							if (triggerHappy || laneRequested == playerData.CurrentLane) // Really sure to work on the current lane
							{
								if (hasPurchasedAbility(Abilities.Nuke) && !isAbilityCoolingDown(Abilities.Nuke))
								{
									if (nukeOnLane(laneRequested))
									{
										if (highestHpFactorOnLane(laneRequested) > 0.65m)
										{
											if (abilities) abilties_json += ",";
											abilties_json += "{\"ability\":" + (int)Abilities.Nuke + "}"; // Nuke
											abilities = true;
										}
									}
								}

								if (hasPurchasedAbility(Abilities.ClusterBomb) && !isAbilityCoolingDown(Abilities.ClusterBomb))
								{
									if (lastBombLevel != gameData.Level) // Don't bomb/napalm on same level to get better spread
									{
										if (bombOnLane(laneRequested))
										{
											if (abilities) abilties_json += ",";
											abilties_json += "{\"ability\":" + (int)Abilities.ClusterBomb + "}";
											abilities = true;
											lastBombLevel = gameData.Level;
										}
									}
								}

								if (hasPurchasedAbility(Abilities.Napalm) && !isAbilityCoolingDown(Abilities.Napalm))
								{
									if (lastBombLevel != gameData.Level) // Don't bomb/napalm on same level to get better spread
									{
										if (bombOnLane(laneRequested))
										{
											if (abilities) abilties_json += ",";
											abilties_json += "{\"ability\":" + (int)Abilities.Napalm + "}";
											abilities = true;
											lastBombLevel = gameData.Level;
										}
									}
								}
							}
						}

						if (itemAbilitiesOn)
						{
							if (hasPurchasedAbility(Abilities.IncreaseCritPercentagePermanently) && !isAbilityCoolingDown(Abilities.IncreaseCritPercentagePermanently))
							{
								// Permanent upgrades, always spam them as soon as possible
								if (abilities) abilties_json += ",";
								abilties_json += "{\"ability\":" + (int)Abilities.IncreaseCritPercentagePermanently + "}";
								abilities = true;
								requestTreeRefresh = true;
							}
							if (hasPurchasedAbility(Abilities.IncreaseHPPermanently) && !isAbilityCoolingDown(Abilities.IncreaseHPPermanently))
							{
								// Permanent upgrades, always spam them as soon as possible
								if (abilities) abilties_json += ",";
								abilties_json += "{\"ability\":" + (int)Abilities.IncreaseHPPermanently + "}";
								abilities = true;
								requestTreeRefresh = true;
							}
							if (useWormHoleOnLane(playerData.CurrentLane)) // TODO: Or endgame
							{
								bool doMultiWormhole = multiWormholeOn && lastWormholeLevel == gameData.Level && highestHpFactorOnLane(laneRequested) > 0.5m;
								if (hasPurchasedAbility(Abilities.Wormhole) && (!isAbilityCoolingDown(Abilities.Wormhole) || doMultiWormhole))
								{
									// Permanent upgrades, always spam them as soon as possible
									int nb = doMultiWormhole ? multiWormholeCount : 1;
									for (int i = 0; i < nb; ++i)
									{ 
										if (abilities) abilties_json += ",";
										abilties_json += "{\"ability\":" + (int)Abilities.Wormhole + "}";
										abilities = true;
									}
									lastWormholeLevel = gameData.Level;
									requestTreeRefresh = true;
									rearmLikeNewAt = System.Environment.TickCount + likeNewTimerMin + random.Next(likeNewTimerMax - likeNewTimerMin);
								}
								else if (rearmLikeNewAt < System.Environment.TickCount) // Launch Like new in the follow tick if ok
								{
									if (hasPurchasedAbility(Abilities.ClearCool) && !isAbilityCoolingDown(Abilities.ClearCool))
									{
										if (random.Next(itemCount(Abilities.Wormhole)) < likeNewChance)
										{
											if (abilities) abilties_json += ",";
											abilties_json += "{\"ability\":" + (int)Abilities.Wormhole + "}";
											abilities = true;
											requestTreeRefresh = true;
											rearmLikeNewAt = System.Environment.TickCount + likeNewTimerMin + random.Next(likeNewTimerMax - likeNewTimerMin);
										}
									}
								}
							}
							if (triggerHappy || laneRequested == playerData.CurrentLane) // Really sure to work on the current lane
							{
								bool farmingGold = farmingGoldOnLane(laneRequested);
								bool avoidDamageOnLane = avoidExtraDamageOnLane(laneRequested);

								if (farmingGold)
								{
									// Gold rain
									if ((itemCount(Abilities.GoldRain) > 12 || lastGoldRainLevel != gameData.Level) // If not many left over, only fire maximum of one per level
										&& hasPurchasedAbility(Abilities.GoldRain) && !isAbilityCoolingDown(Abilities.GoldRain))
									{
										// When already done gold rain on this level, allow for a lower HP, as this means the boss is going down slowly
										if (highestHpFactorOnLane(laneRequested) >= ((lastGoldRainLevel != gameData.Level) ? 0.75m : 0.25m))
										{
											if (abilities) abilties_json += ",";
											abilties_json += "{\"ability\":" + (int)Abilities.GoldRain + "}";
											abilities = true;
											lastGoldRainLevel = gameData.Level;
											requestTreeRefresh = true;
										}
									}
								}

								if (hasPurchasedAbility(Abilities.GiveGold) && !isAbilityCoolingDown(Abilities.GiveGold)) // Treasure
								{
									// When no player money or under same circumstances as Metal Detector
									if ((playerData.Gold < 100000.0m)
										|| (avoidDamageOnLane && highestHpFactorOnLane(laneRequested) > 0.25m))
									{
										if (abilities) abilties_json += ",";
										abilties_json += "{\"ability\":" + (int)Abilities.GiveGold + "}";
										abilities = true;
										requestTreeRefresh = true;
									}
								}

								if ((gameData.Level % 10) == 0 && !avoidDamageOnLane) // Use this on boss levels, but not when farming gold
								{
									if (bossMonsterOnLane(laneRequested) || countLiveMonstersOnLane(laneRequested) >= 2) // Either in boss lane or in a lane with enough live monsters
									{
										if (hasPurchasedAbility(Abilities.ReflectDamage) && !isAbilityCoolingDown(Abilities.ReflectDamage))
										{
											if (abilities) abilties_json += ",";
											abilties_json += "{\"ability\":" + (int)Abilities.ReflectDamage + "}";
											abilities = true;
											requestTreeRefresh = true;
										}

										if (hasPurchasedAbility(Abilities.MaximizeElement) && !isAbilityCoolingDown(Abilities.MaximizeElement))
										{
											if (abilities) abilties_json += ",";
											abilties_json += "{\"ability\":" + (int)Abilities.MaximizeElement + "}";
											abilities = true;
											requestTreeRefresh = true;
										}
									}
								}
								
								// Use this on lanes with enough live monsters
								if (countLiveMonstersOnLane(laneRequested) >= 3 || (bossMonsterOnLane(laneRequested) && avoidDamageOnLane) || (((gameData.Level % 10) == 0) && countLiveMonstersOnLane(laneRequested) >= 2))
								{
									if (highestHpFactorOnLane(laneRequested) > (((gameData.Level % 10) == 0) ? 0.35m : 0.75m))
									{
										if (hasPurchasedAbility(Abilities.StealHealth) && !isAbilityCoolingDown(Abilities.StealHealth))
										{
											if (abilities) abilties_json += ",";
											abilties_json += "{\"ability\":" + (int)Abilities.StealHealth + "}";
											abilities = true;
											requestTreeRefresh = true;
										}

										if ((gameData.Level % 10) == 0) // Use this on boss levels
										{
											if (hasPurchasedAbility(Abilities.Revive) && !isAbilityCoolingDown(Abilities.Revive)) // Resurrection
											{
												if (abilities) abilties_json += ",";
												abilties_json += "{\"ability\":" + (int)Abilities.Revive + "}";
												abilities = true;
												requestTreeRefresh = true;
											}
										}
									}
								}

								if (!avoidDamageOnLane)
								{
									if (gameData.Level > Math.Max(speedThreshold_wchill, speedThreshold_steamdb))
									{
										if (hasPurchasedAbility(Abilities.CrippleMonster) && !isAbilityCoolingDown(Abilities.CrippleMonster)) // Cripple Monster
										{
											if (bossMonsterOnLane(laneRequested)) // Cripple bosses
											{
												if (highestHpFactorOnLane(laneRequested) >= 0.5m)
												{
													if (abilities) abilties_json += ",";
													abilties_json += "{\"ability\":" + (int)Abilities.CrippleMonster + "}";
													abilities = true;
													requestTreeRefresh = true;
												}
											}
										}
									}

									if (hasPurchasedAbility(Abilities.CrippleSpawner) && !isAbilityCoolingDown(Abilities.CrippleSpawner)) // Cripple Spawner
									{
										int spawner = findSpawnerOnLane(laneRequested);
										if (spawner >= 0)
										{
											if (gameData.Lanes[laneRequested].Enemies[spawner].MaxHp != 0)
											{
												decimal ratio = gameData.Lanes[laneRequested].Enemies[spawner].Hp / gameData.Lanes[laneRequested].Enemies[spawner].MaxHp;
												if (ratio > 0.95m)
												{
													if (abilities) abilties_json += ",";
													abilties_json += "{\"ability\":" + (int)Abilities.CrippleSpawner + "}";
													abilities = true;
													requestTreeRefresh = true;
												}
											}
										}
									}
								}
							}
						}
					}

					long ac = 0;
					if (autoClickerOn && maxClicks >= minClicks
						&& (!superWormholeOn || ((gameData.Level % superWormholeRounds) != 0)))
					{
						for (int i = 0; i < clickBoost; ++i) // Send clicks ability multiple times
						{
							int nb = useWormHoleOnLane(laneRequested) ? 1 : // Less clicking on wormhole boss
								minClicks + random.Next(maxClicks - minClicks); // Random clicks number

							ac += (long)nb;
							if (abilities) abilties_json += ",";
							abilties_json += "{\"ability\":1,\"num_clicks\":" + nb + "}";
							abilities = true;
						}
					}
					addClicks = ac;

					if (abilities && (this.techTree.BadgePoints <= 0))
					{
						// Send the UseAbilities POST request
						abilties_json += "]}";
						StringBuilder url = new StringBuilder();
						url.Append("https://");
						url.Append(host);
						url.Append("UseAbilities/v0001/");
						StringBuilder query = new StringBuilder();
						query.Append("input_json=");
						query.Append(WebUtilities.UrlEncode(abilties_json));
						query.Append("&access_token=");
						query.Append(accessToken);
						query.Append("&format=json");
						wc.Headers[HttpRequestHeader.AcceptCharset] = "utf-8";
						wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
						if (!exiting) Invoke(enableDelegate, postAbilitiesState, true);
						Console.WriteLine(abilties_json);
						string res = wc.UploadString(url.ToString(), query.ToString());
						if (showResponsesOn) Console.WriteLine(res);
						JSONNode json = JSON.Parse(res);
						if (!exiting) Invoke(resultPostAbilitiesDelegate, json);
					}
					else if (!string.IsNullOrEmpty(steamId))
					{
						StringBuilder url = new StringBuilder();
						url.Append("http://");
						url.Append(host);
						url.Append("GetPlayerData/v0001/?gameid=");
						url.Append(room);
						url.Append("&steamid=");
						url.Append(steamId);
						url.Append("&include_tech_tree=0&format=json");
						if (!exiting) Invoke(enableDelegate, postAbilitiesState, true);
						string res = wc.DownloadString(url.ToString());
						JSONNode json = JSON.Parse(res);
						if (!exiting) Invoke(resultPostAbilitiesDelegate, json);
					}

					if (requestTreeRefresh)
						refreshUpgrades = true;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
				if (!exiting) Invoke(enableDelegate, postAbilitiesState, false);
				int endTick = System.Environment.TickCount;
				int toSleep = 1000 - (endTick - startTick);
				if (toSleep > 0) System.Threading.Thread.Sleep(toSleep);
			}
			wc.Dispose();
			Invoke(endedThreadDelegate);
		}
	}
}
