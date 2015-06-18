using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using SimpleJSON;

// This will be the home of automatic upgrades

namespace MonsterGUI
{
	public partial class MainWindow
	{
		volatile bool autoUpgradesOn = false;
		volatile bool autoBadgesOn = false;

		volatile bool refreshUpgrades = false;
		volatile int waitForNewPlayerData = 3;
		volatile bool waitForTuningData = true;
		bool waitForUpgradeData = true;

		decimal upgradeMaxHP = 10000m;
		decimal upgradeMaxDPS = 1000000000000000m;
		decimal upgradeMaxDamage = 1000000000000000m;
		decimal upgradeMaxCrit = 1000000000000000m;
		decimal upgradeMaxLoot = 1.25m;
		decimal upgradeMaxFire = 1m;
		decimal upgradeMaxWater = 1m;
		decimal upgradeMaxEarth = 1m;
		decimal upgradeMaxAir = 1m;

		const int badgeBuyCrit = 30;
		const int badgeBuyCritPrice = 10;
		const int badgeBuyRain = 30;
		const int badgeBuyRainPrice = 10;
		const int badgeBuyWormholePerTick = 350;
		const int badgeBuyWormholePrice = 100;
		const int badgeBuyLikeNewPerTick = 10;
		const int badgeBuyLikeNewPrice = 100;

		int[] upgradeAccelerateToLevel = new int[(int)UpgradeOption.Max] {
			20, // LightArmor = 0,
			20, // AutoFireCannon = 1,
			20, // ArmorPiercingRound = 2,
			0, // ElementalFire = 3,
			0, // ElementalWater = 4,
			0, // ElementalAir = 5,
			0, // ElementalEarth = 6,
			0, // LuckyShot = 7,
			10,// HeavyArmor = 8,
			10,// AdvancedTargeting = 9,
			10,// ExplosiveRounds = 10,
			0, // Medics = 11,
			0, // MoraleBooster = 12,
			0, // GoodLuckCharms = 13,
			0, // MetalDetector = 14,
			0, // DecreaseCooldowns = 15,
			0, // TacticalNuke = 16,
			0, // ClusterBomb = 17,
			0, // Napalm = 18,
			0, // BossLoot = 19,
			10, // EnergyShields = 20,
			10, // FarmingEquipment = 21,
			10, // Railgun = 22,
			10, // PersonalTraining = 23,
			10, // AFKEquipment = 24,
			10, // NewMouseButton = 25,
			10, // HPUpgrade5 = 26,
			10, // DPSUpgrade5 = 27,
			10, // ClickUpgrade5 = 28,
			0, // 29
			0, // 30
			0, // 31
			0, 0, 0, 0, 0, 0, 0, 0, // 39
			0, 0, 0, 0, 0, 0, 0, 0, // 47
			0, 0, 0, 0, 0, 0, 0, 0, 
			0, 0, 0, 0, 0, 0, 0, 0, 
		};

		bool[] printUpgradingType = new bool[(int)UpgradeType.Nb];

		private void postUpgradesInit()
		{
			resultPostUpgradesDelegate = new JsonCallback(resultPostUpgrades);
			upgrMaxHP.Value = upgradeMaxHP;
			upgrMaxDPS.Value = upgradeMaxDPS;
			upgrMaxDmg.Value = upgradeMaxDamage;
			upgrMaxCrit.Value = upgradeMaxCrit;
			upgrMaxLoot.Value = upgradeMaxLoot;
			upgrMaxFire.Value = upgradeMaxFire;
			upgrMaxWater.Value = upgradeMaxWater;
			upgrMaxEarth.Value = upgradeMaxEarth;
			upgrMaxAir.Value = upgradeMaxAir;
			autoUpgradesCheck.Checked = autoUpgradesOn;
			badgesCheck.Checked = autoBadgesOn;
		}

		/// <summary>
		/// When user pushes GO
		/// </summary>
		private void postUpgradesGo()
		{
			waitForNewPlayerData = 2;
			waitForTuningData = true;
			waitForUpgradeData = true;
		}

		private JsonCallback resultPostUpgradesDelegate;
		private void resultPostUpgrades(JSONNode json)
		{
			JSONNode response = json["response"];
			if (response == null)
				return;
			JSONNode techTree = response["tech_tree"];
			if (techTree == null)
				return;
			decodeTechTree(techTree);
		}

		private bool upgradeUnlocked(UpgradeOption upgrade)
		{
			return techTree.Upgrades[tuningData.Upgrades[(int)upgrade].RequiredUpgrade].Level
				>= tuningData.Upgrades[(int)upgrade].RequiredUpgradeLevel;
		}

		/*static decimal getDamage(decimal dmgBase, decimal baseMultiplier, decimal critPercentage, decimal critMultiplier, decimal eleMultiplier)
		{
			decimal critPercentageCapped = Math.Min(critPercentage, 1m);
			decimal critPercentageInv = 1m - critPercentageCapped;
			return ((dmgBase * baseMultiplier * eleMultiplier) * critPercentageInv)
				+ ((dmgBase * critMultiplier) * critPercentageCapped);
		}*/ // TODO

		/// <summary>
		/// Thread calling POST ChooseUpgrade
		/// </summary>
		private void postUpgradesThread()
		{
			// ChooseUpgrade: {"gameid":"6059","upgrades":[4,4,5,6]}

			WebClient wc = new TimeoutWebClient();
			const int notSentCountLimit = 60; // Send a blank upgrade request every 60 ticks to force update the player state
			int notSentCount = notSentCountLimit;
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

					bool hasBadgePoints = this.techTree.BadgePoints > 0;

					bool upgrades = false;
					string upgrades_json = hasBadgePoints
						? "{\"gameid\":\"" + room + "\",\"ability_items\":["
						: "{\"gameid\":\"" + room + "\",\"upgrades\":[";

					if (waitForNewPlayerData <= 0 && !waitForUpgradeData && !waitForTuningData)
					{
						for (int i = 0; i < printUpgradingType.Length; ++i)
						{
							printUpgradingType[i] = false;
						}
						if (hasBadgePoints && autoBadgesOn)
						{
							int countLimit = 500;
							long remainingBadgePoints = this.techTree.BadgePoints;
							Action<Abilities, int, int> buyWithPoints = (ability, repeat, price) =>
							{
								for (int i = 0; i < repeat; ++i)
								{
									if (countLimit <= 0)
										break;
									if (remainingBadgePoints < (long)price)
										break;
									if (upgrades) upgrades_json += ",";
									upgrades_json += Convert.ToString((int)ability, System.Globalization.CultureInfo.InvariantCulture);
									upgrades = true;
									remainingBadgePoints -= (long)price;
									--countLimit;
								}
							};
							int remainingCrit = Math.Max(0, badgeBuyCrit - itemCount(Abilities.IncreaseCritPercentagePermanently));
							int remainingRain = Math.Max(0, badgeBuyRain - itemCount(Abilities.GoldRain));
							buyWithPoints(Abilities.IncreaseCritPercentagePermanently, remainingCrit, badgeBuyCritPrice);
							buyWithPoints(Abilities.GoldRain, remainingRain, badgeBuyRainPrice);
							buyWithPoints(Abilities.Wormhole, badgeBuyWormholePerTick, badgeBuyWormholePrice);
							buyWithPoints(Abilities.ClearCool, badgeBuyLikeNewPerTick, badgeBuyWormholePrice);
							if (remainingBadgePoints < badgeBuyWormholePrice)
								buyWithPoints(Abilities.IncreaseHPPermanently, (int)remainingBadgePoints, 1);
						}
						else if (autoUpgradesOn && !hasBadgePoints)
						{
							// Upgrade prices can be found under techTree
							// Example how to add an upgrade
							/*
								if (upgrades) upgrades_json += ",";
								upgrades_json += "4";
								upgrades = true;
							*/

							decimal goldRemaining = playerData.Gold;

							Action<UpgradeOption> upgradeOption = upgrade =>
							{
								int ldif = upgradeAccelerateToLevel[(int)upgrade] - techTree.Upgrades[(int)upgrade].Level;
								int repeat = ldif > 0 ? ldif : 1;
								goldRemaining -= techTree.Upgrades[(int)upgrade].CostForNextLevel * (decimal)repeat; // Not accurate but whatever.
								for (int i = 0; i < repeat; ++i)
								{
									if (upgrades) upgrades_json += ",";
									upgrades_json += Convert.ToString((int)upgrade, System.Globalization.CultureInfo.InvariantCulture);
									upgrades = true;
									printUpgradingType[(int)tuningData.Upgrades[(int)upgrade].Type] = true;
								}
							};

							int maxNumClick = 20;
							decimal cps = ((decimal)Math.Min(minClicks, maxClicks) + (decimal)Math.Min(maxClicks, maxNumClick)) * 0.5m;

							decimal dmgBase = tuningData.Player.DamagePerClick;
							decimal mulBase = techTreeUpgradeMultipliers[(int)UpgradeType.ClickDamage];
							decimal mulCrit = techTreeUpgradeMultipliers[(int)UpgradeType.DamageMultiplier_Crit];
							decimal dpc = dmgBase * mulBase;
							decimal autoDpsMul = techTreeUpgradeMultipliers[(int)UpgradeType.DPS];
							decimal dpcCrit = dpc * mulCrit;
							decimal mulEle = Math.Max(techTreeUpgradeMultipliers[(int)UpgradeType.DamageMultiplier_Air],
								Math.Max(techTreeUpgradeMultipliers[(int)UpgradeType.DamageMultiplier_Earth],
								Math.Max(techTreeUpgradeMultipliers[(int)UpgradeType.DamageMultiplier_Fire],
								techTreeUpgradeMultipliers[(int)UpgradeType.DamageMultiplier_Water])));

							decimal critPercentage = techTree.CritPercentage;
							critPercentage = Math.Min(critPercentage, 1m);
							decimal invCritPercentage = 1m - critPercentage;

							// decimal upgrClickPU = dmgBase;
							decimal cheapestClickDamagePPU = decimal.MaxValue;
							decimal cheapestAutoClickPPU = decimal.MaxValue;
							decimal cheapestHpPPU = decimal.MaxValue;
							UpgradeOption cheapestClickDamage = UpgradeOption.ArmorPiercingRound;
							UpgradeOption cheapestAutoClick = UpgradeOption.AutoFireCannon;
							UpgradeOption cheapestHp = UpgradeOption.LightArmor;

							for (int i = 0; i < tuningData.Upgrades.Length; ++i)
							{
								if (upgradeUnlocked((UpgradeOption)i))
								{
									if (tuningData.Upgrades[i].Multiplier > 0m)
									{
										decimal ppu = techTree.Upgrades[i].CostForNextLevel / tuningData.Upgrades[i].Multiplier;
										switch (tuningData.Upgrades[i].Type)
										{
											case UpgradeType.ClickDamage:
												if (ppu < cheapestClickDamagePPU)
												{
													cheapestClickDamagePPU = ppu;
													cheapestClickDamage = (UpgradeOption)i;
												}
												break;
											case UpgradeType.DPS:
												if (ppu < cheapestAutoClickPPU)
												{
													cheapestAutoClickPPU = ppu;
													cheapestAutoClick = (UpgradeOption)i;
												}
												break;
											case UpgradeType.HitPoints:
												if (ppu < cheapestHpPPU)
												{
													cheapestHpPPU = ppu;
													cheapestHp = (UpgradeOption)i;
												}
												break;
										}
									}
								}
							}

							// Override DPS upgrade for unlock
							if (techTree.Upgrades[(int)UpgradeOption.AutoFireCannon].Level < 20)
								cheapestAutoClick = UpgradeOption.AutoFireCannon;

							/*decimal upgrClickDamageDpsPU = clickDpsEstimate / tuningData.Upgrades[(int)cheapestClickDamage].Multiplier;
							decimal upgrAutoClickDpsPU = autoDps * tuningData.Upgrades[(int)cheapestAutoClick].Multiplier;
							decimal upgrElementalDpsPU = clickDpsEstimate * tuningData.Upgrades[(int)UpgradeOption.ElementalFire].Multiplier;
							decimal upgrCritDmgDpsPU = dpc * cps * critChanceFactor * tuningData.Upgrades[(int)UpgradeOption.LuckyShot].Multiplier;*/

							decimal unitAutoDpsEstimate = dmgBase; // (dmgBase * invCritPercentage) + (dmgBase * mulCrit * critPercentage); // TODO: Does crit effect auto dps?
							decimal unitClickDpsEstimate = unitAutoDpsEstimate * cps;

							// Damage Per Gold
							decimal upgrClickDamage = (unitClickDpsEstimate * mulEle)
								* tuningData.Upgrades[(int)cheapestClickDamage].Multiplier;
							decimal upgrClickDamageDPG = upgrClickDamage
								/ techTree.Upgrades[(int)cheapestClickDamage].CostForNextLevel;
							decimal upgrAutoClick = (unitAutoDpsEstimate * mulEle)
								* tuningData.Upgrades[(int)cheapestAutoClick].Multiplier;
							decimal upgrAutoClickDPG = upgrAutoClick
								/ techTree.Upgrades[(int)cheapestAutoClick].CostForNextLevel;
							decimal upgrElementalFire = (unitClickDpsEstimate * mulBase)
								* tuningData.Upgrades[(int)UpgradeOption.ElementalFire].Multiplier;
							decimal upgrElementalFireDPG = upgrElementalFire
								/ techTree.Upgrades[(int)UpgradeOption.ElementalFire].CostForNextLevel;
							decimal upgrElementalWater = (unitClickDpsEstimate * mulBase)
								* tuningData.Upgrades[(int)UpgradeOption.ElementalWater].Multiplier;
							decimal upgrElementalWaterDPG = upgrElementalWater
								/ techTree.Upgrades[(int)UpgradeOption.ElementalWater].CostForNextLevel;
							decimal upgrElementalEarth = (unitClickDpsEstimate * mulBase)
								* tuningData.Upgrades[(int)UpgradeOption.ElementalEarth].Multiplier;
							decimal upgrElementalEarthDPG = upgrElementalEarth
								/ techTree.Upgrades[(int)UpgradeOption.ElementalEarth].CostForNextLevel;
							decimal upgrElementalAir = (unitClickDpsEstimate * mulBase)
								* tuningData.Upgrades[(int)UpgradeOption.ElementalAir].Multiplier;
							decimal upgrElementalAirDPG = upgrElementalAir
								/ techTree.Upgrades[(int)UpgradeOption.ElementalAir].CostForNextLevel;
							decimal upgrCrit = (dpc * critPercentage * cps * mulEle)
								* tuningData.Upgrades[(int)UpgradeOption.LuckyShot].Multiplier;
							decimal upgrCritDPG = upgrCrit
								/ techTree.Upgrades[(int)UpgradeOption.LuckyShot].CostForNextLevel;
							decimal upgrEleDPG = Math.Max(upgrElementalFireDPG, Math.Max(upgrElementalWaterDPG, Math.Max(upgrElementalEarthDPG, upgrElementalAirDPG))); // Should be all the same really

							/*Console.WriteLine("upgrClickDamageDPG: " + upgrClickDamageDPG);
							Console.WriteLine("upgrAutoClickDPG: " + upgrAutoClickDPG);
							Console.WriteLine("upgrElementalFireDPG: " + upgrElementalFireDPG);
							Console.WriteLine("upgrElementalWaterDPG: " + upgrElementalWaterDPG);
							Console.WriteLine("upgrElementalEarthDPG: " + upgrElementalEarthDPG);
							Console.WriteLine("upgrElementalAirDPG: " + upgrElementalAirDPG);
							Console.WriteLine("upgrCritDPG: " + upgrCritDPG);*/

							if (techTree.Upgrades[(int)cheapestClickDamage].CostForNextLevel < goldRemaining
								&& techTreeUpgradeMultipliers[(int)UpgradeType.ClickDamage] < upgradeMaxDamage)
							{
								upgradeOption(cheapestClickDamage);
							}

							if (upgrAutoClickDPG >= upgrClickDamageDPG)
							{
								if (techTree.Upgrades[(int)cheapestAutoClick].CostForNextLevel < goldRemaining
									&& techTreeUpgradeMultipliers[(int)UpgradeType.DPS] < upgradeMaxDPS)
								{
									upgradeOption(cheapestAutoClick);
								}
							}
							else if (techTree.Upgrades[(int)UpgradeOption.AutoFireCannon].Level < 20) // Special rule
							{
								if (techTree.Upgrades[(int)UpgradeOption.AutoFireCannon].CostForNextLevel < goldRemaining)
								{
									upgradeOption(UpgradeOption.AutoFireCannon);
								}
							}

							if (upgrCritDPG >= upgrClickDamageDPG)
							{
								if (techTree.Upgrades[(int)UpgradeOption.LuckyShot].CostForNextLevel < goldRemaining
									&& techTreeUpgradeMultipliers[(int)UpgradeType.DamageMultiplier_Crit] < upgradeMaxCrit)
								{
									upgradeOption(UpgradeOption.LuckyShot);
								}
							}

							if (upgrEleDPG >= upgrClickDamageDPG)
							{
								UpgradeOption lowestEle = UpgradeOption.Nb;
								decimal lowestElem = decimal.MaxValue;
								decimal[] upgrMaxEle = new decimal[4] {
									upgradeMaxFire,
									upgradeMaxWater,
									upgradeMaxAir,
									upgradeMaxEarth
								};
								for (int i = (int)UpgradeType.DamageMultiplier_Fire; i <= (int)UpgradeType.DamageMultiplier_Earth; ++i)
								{
									if (techTreeUpgradeMultipliers[i] < upgrMaxEle[i - (int)UpgradeType.DamageMultiplier_Fire]
										&& techTreeUpgradeMultipliers[i] < lowestElem)
									{
										lowestEle = (UpgradeOption)(i - (int)UpgradeType.DamageMultiplier_Fire + (int)UpgradeOption.ElementalFire);
										lowestElem = techTreeUpgradeMultipliers[i];
									}
								}
								if (lowestEle != UpgradeOption.Nb)
								{
									if (techTree.Upgrades[(int)lowestEle].CostForNextLevel < goldRemaining
										&& lowestElem < upgrMaxEle[(int)lowestEle - (int)UpgradeType.DamageMultiplier_Fire])
									{
										upgradeOption(lowestEle);
									}
								}
							}

							if (techTree.Upgrades[(int)cheapestHp].CostForNextLevel < goldRemaining
								&& techTreeUpgradeMultipliers[(int)UpgradeType.HitPoints] < upgradeMaxHP)
							{
								upgradeOption(cheapestHp);
							}

							if (upgradeUnlocked(UpgradeOption.BossLoot))
							{
								if (techTree.Upgrades[(int)UpgradeOption.BossLoot].CostForNextLevel < goldRemaining
									&& techTreeUpgradeMultipliers[(int)UpgradeType.BossLootDropPercentage] < upgradeMaxLoot)
								{
									upgradeOption(UpgradeOption.BossLoot);
								}
							}

							if (upgradeUnlocked(UpgradeOption.LuckyShot))
							{
								if (techTree.Upgrades[(int)UpgradeOption.LuckyShot].CostForNextLevel < goldRemaining
									&& techTreeUpgradeMultipliers[(int)UpgradeType.DamageMultiplier_Crit] < upgradeMaxCrit)
								{
									upgradeOption(UpgradeOption.LuckyShot);
								}
							}

							for (int i = 0; i < tuningData.Upgrades.Length; ++i)
							{
								if (upgradeUnlocked((UpgradeOption)i))
								{
									if (tuningData.Upgrades[i].Type == UpgradeType.PurchaseAbility)
									{
										if (techTree.Upgrades[i].Level < 1)
										{
											if (techTree.Upgrades[i].CostForNextLevel < goldRemaining)
											{
												upgradeOption((UpgradeOption)i);
											}
										}
									}
								}
							}
						}
					}

					// Send the upgrade packet
					if (upgrades)
					{
						upgrades_json += "]}";
						StringBuilder url = new StringBuilder();
						url.Append("https://");
						url.Append(host);
						if (hasBadgePoints) url.Append("UseBadgePoints/v0001/");
						else url.Append("ChooseUpgrade/v0001/");
						StringBuilder query = new StringBuilder();
						query.Append("input_json=");
						query.Append(WebUtilities.UrlEncode(upgrades_json));
						query.Append("&access_token=");
						query.Append(accessToken);
						query.Append("&format=json");
						wc.Headers[HttpRequestHeader.AcceptCharset] = "utf-8";
						wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
						if (!exiting) Invoke(enableDelegate, postUpgradesState, true);
						Console.WriteLine(upgrades_json);
						string res = wc.UploadString(url.ToString(), query.ToString());
						if (showResponsesOn) Console.WriteLine(res);
						JSONNode json = JSON.Parse(res);
						if (!exiting) Invoke(resultPostUpgradesDelegate, json);
						notSentCount = 0;
						refreshUpgrades = false;
						waitForNewPlayerData = 2; // Wait for two new player data packages to be sure we have the latest gold
					}
					else if ((notSentCount >= notSentCountLimit || refreshUpgrades || waitForUpgradeData) && !string.IsNullOrEmpty(steamId))
					{
						StringBuilder url = new StringBuilder();
						url.Append("http://");
						url.Append(host);
						url.Append("GetPlayerData/v0001/?gameid=");
						url.Append(room);
						url.Append("&steamid=");
						url.Append(steamId);
						url.Append("&include_tech_tree=1&format=json");
						if (!exiting) Invoke(enableDelegate, postUpgradesState, true);
						string res = wc.DownloadString(url.ToString());
						JSONNode json = JSON.Parse(res);
						if (!exiting) Invoke(resultPostUpgradesDelegate, json);
						notSentCount = 0;
						refreshUpgrades = false;
					}
					else
					{
						++notSentCount;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
				if (!exiting) Invoke(enableDelegate, postUpgradesState, false);
				int endTick = System.Environment.TickCount;
				int toSleep = 1000 - (endTick - startTick);
				if (toSleep > 0) System.Threading.Thread.Sleep(toSleep);
			}
			wc.Dispose();
			Invoke(endedThreadDelegate);
		}
	}
}
