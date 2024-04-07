#region

using System.Linq;
using Imperium.Netcode;
using Imperium.Util;
using Imperium.Util.Binding;
using UnityEngine;

#endregion

namespace Imperium.Core;

internal class GameManager : ImpLifecycleObject
{
    internal GameManager(ImpBinaryBinding sceneLoaded, ImpBinding<int> playersConnected)
        : base(sceneLoaded, playersConnected)
    {
        TimeIsPaused = new ImpBinding<bool>(
            false,
            syncOnUpdate: value =>
                ImpNetTime.Instance.UpdateTimeServerRpc(TimeSpeed?.Value ?? ImpConstants.DefaultTimeSpeed, value)
        );
        TimeSpeed = new ImpBinding<float>(
            ImpConstants.DefaultTimeSpeed,
            syncOnUpdate: value =>
                ImpNetTime.Instance.UpdateTimeServerRpc(value, TimeIsPaused?.Value ?? false)
        );
    }

    internal readonly ImpBinding<int> CustomSeed = new(-1);

    internal readonly ImpBinding<bool> IndoorSpawningPaused = new(
        false,
        value =>
        {
            Imperium.Output.Send(
                value ? "Indoor spawning has been paused!" : "Indoor spawning has been resumed!",
                notificationType: NotificationType.Confirmation
            );
        }
    );

    internal readonly ImpBinding<bool> OutdoorSpawningPaused = new(
        false,
        value =>
        {
            Imperium.Output.Send(
                value ? "Outdoor spawning has been paused!" : "Outdoor spawning has been resumed!",
                notificationType: NotificationType.Confirmation
            );
        }
    );

    internal readonly ImpBinding<bool> DaytimeSpawningPaused = new(
        false,
        value =>
        {
            Imperium.Output.Send(
                value ? "Daytime spawning has been paused!" : "Daytime spawning has been resumed!",
                notificationType: NotificationType.Confirmation
            );
        }
    );

    internal readonly ImpBinding<bool> TimeIsPaused;
    internal readonly ImpBinding<float> TimeSpeed;

    internal readonly ImpBinding<float> MaxIndoorPower = new(
        Imperium.RoundManager.currentMaxInsidePower,
        value =>
        {
            Imperium.RoundManager.currentMaxInsidePower = value;
            Imperium.Output.Send($"Indoor Power set to {value}!", notificationType: NotificationType.Confirmation);
        },
        ignoreRefresh: true,
        syncOnUpdate: value => ImpNetSpawning.Instance.SetMaxIndoorPowerServerRpc(value)
    );

    internal readonly ImpBinding<float> MaxOutdoorPower = new(
        Imperium.RoundManager.currentMaxOutsidePower,
        value =>
        {
            Imperium.RoundManager.currentMaxOutsidePower = value;
            Imperium.Output.Send($"Outdoor Power set to {value}!", notificationType: NotificationType.Confirmation);
        },
        ignoreRefresh: true,
        syncOnUpdate: value => ImpNetSpawning.Instance.SetMaxOutdoorPowerServerRpc(value)
    );

    internal readonly ImpBinding<int> MaxDaytimePower = new(
        Imperium.RoundManager.currentLevel.maxDaytimeEnemyPowerCount,
        value =>
        {
            Imperium.RoundManager.currentLevel.maxDaytimeEnemyPowerCount = value;
            Imperium.Output.Send($"Daytime Power set to {value}!", notificationType: NotificationType.Confirmation);
        },
        ignoreRefresh: true,
        syncOnUpdate: value => ImpNetSpawning.Instance.SetMaxDaytimePowerServerRpc(value)
    );

    internal readonly ImpBinding<float> IndoorDeviation = new(
        Imperium.RoundManager.currentLevel.spawnProbabilityRange,
        value =>
        {
            Imperium.RoundManager.currentLevel.spawnProbabilityRange = value;
            Imperium.Output.Send($"Indoor deviation set to {value}!", notificationType: NotificationType.Confirmation);
        },
        ignoreRefresh: true,
        syncOnUpdate: value => ImpNetSpawning.Instance.SetIndoorDeviationServerRpc(value)
    );

    internal readonly ImpBinding<float> DaytimeDeviation = new(
        Imperium.RoundManager.currentLevel.daytimeEnemiesProbabilityRange,
        value =>
        {
            Imperium.RoundManager.currentLevel.daytimeEnemiesProbabilityRange = value;
            Imperium.Output.Send($"Daytime deviation set to {value}!", notificationType: NotificationType.Confirmation);
        },
        ignoreRefresh: true,
        syncOnUpdate: value => ImpNetSpawning.Instance.SetDaytimeDeviationServerRpc(value)
    );

    internal readonly ImpBinding<int> MinIndoorSpawns = new(
        Imperium.RoundManager.minEnemiesToSpawn,
        value =>
        {
            Imperium.RoundManager.minEnemiesToSpawn = value;
            Imperium.Output.Send($"Daytime deviation set to {value}!", notificationType: NotificationType.Confirmation);
        },
        ignoreRefresh: true,
        syncOnUpdate: value => ImpNetSpawning.Instance.SetMinIndoorEntitiesServerRpc(value)
    );

    internal readonly ImpBinding<int> MinOutdoorSpawns = new(
        Imperium.RoundManager.minOutsideEnemiesToSpawn,
        value =>
        {
            Imperium.RoundManager.minOutsideEnemiesToSpawn = value;
            Imperium.Output.Send(
                $"Minimum outdoor spawn has been set to {value}!",
                notificationType: NotificationType.Confirmation
            );
        },
        ignoreRefresh: true,
        syncOnUpdate: value => ImpNetSpawning.Instance.SetMinOutdoorEntitiesServerRpc(value)
    );

    internal readonly ImpBinding<float> WeatherVariable1 = new(
        Imperium.TimeOfDay.currentWeatherVariable,
        value =>
        {
            if (Imperium.RoundManager.currentLevel.currentWeather == LevelWeatherType.Eclipsed)
            {
                Imperium.RoundManager.minEnemiesToSpawn = (int)value;
                Imperium.RoundManager.minOutsideEnemiesToSpawn = (int)value;
            }

            Imperium.TimeOfDay.currentWeatherVariable = value;
            Imperium.Output.Send($"Weather variable 1 set to {value}!", notificationType: NotificationType.Confirmation);
        },
        ignoreRefresh: true,
        syncOnUpdate: value => ImpNetSpawning.Instance.SetWeatherVariable1ServerRpc(value)
    );

    internal readonly ImpBinding<float> WeatherVariable2 = new(
        Imperium.TimeOfDay.currentWeatherVariable2,
        value =>
        {
            Imperium.TimeOfDay.currentWeatherVariable2 = value;
            Imperium.Output.Send($"Weather variable 2 set to {value}!", notificationType: NotificationType.Confirmation);
        },
        ignoreRefresh: true,
        syncOnUpdate: value => ImpNetSpawning.Instance.SetWeatherVariable2ServerRpc(value)
    );

    internal readonly ImpBinding<int> GroupCredits = new(
        Imperium.Terminal.groupCredits,
        onUpdate: value =>
        {
            Imperium.Output.Send(
                $"Successfully changed group credits to {value}!",
                notificationType: NotificationType.Confirmation
            );
        },
        ignoreRefresh: true,
        syncOnUpdate: value => ImpNetQuota.Instance.SetGroupCreditsServerRpc(value)
    );

    internal readonly ImpBinding<int> ProfitQuota = new(
        Imperium.TimeOfDay.profitQuota,
        onUpdate: value =>
        {
            Imperium.Output.Send(
                $"Successfully changed profit quota to {value}!",
                notificationType: NotificationType.Confirmation
            );
        },
        ignoreRefresh: true,
        syncOnUpdate: value => ImpNetQuota.Instance.SetProfitQuotaServerRpc(value)
    );

    internal readonly ImpBinding<int> QuotaDeadline = new(
        Imperium.TimeOfDay.daysUntilDeadline,
        onUpdate: value =>
        {
            Imperium.Output.Send(
                $"Successfully changed quota deadline to {value}!",
                notificationType: NotificationType.Confirmation
            );
        },
        ignoreRefresh: true,
        syncOnUpdate: value => ImpNetQuota.Instance.SetDeadlineDaysServerRpc(value)
    );

    [ImpAttributes.RemoteMethod]
    internal void FulfillQuota()
    {
        Imperium.TimeOfDay.daysUntilDeadline = -1;
        Imperium.TimeOfDay.quotaFulfilled = Imperium.TimeOfDay.profitQuota;
        Imperium.TimeOfDay.SetNewProfitQuota();
        ProfitQuota.Set(Imperium.TimeOfDay.profitQuota);
    }

    [ImpAttributes.RemoteMethod]
    internal void ResetQuota()
    {
        Imperium.TimeOfDay.SyncNewProfitQuotaClientRpc(
            Imperium.TimeOfDay.quotaVariables.startingQuota, 0, 0
        );
        ProfitQuota.Set(Imperium.TimeOfDay.quotaVariables.startingQuota);
    }

    protected override void OnSceneLoad()
    {
        MaxIndoorPower.Refresh(Imperium.RoundManager.currentMaxInsidePower);
        MaxOutdoorPower.Refresh(Imperium.RoundManager.currentMaxOutsidePower);
        MaxDaytimePower.Refresh(Imperium.RoundManager.currentLevel.maxDaytimeEnemyPowerCount);

        IndoorDeviation.Refresh(Imperium.RoundManager.currentLevel.spawnProbabilityRange);
        DaytimeDeviation.Refresh(Imperium.RoundManager.currentLevel.daytimeEnemiesProbabilityRange);

        MinIndoorSpawns.Refresh(Imperium.RoundManager.minEnemiesToSpawn);
        MinOutdoorSpawns.Refresh(Imperium.RoundManager.minOutsideEnemiesToSpawn);

        WeatherVariable1.Refresh(Imperium.TimeOfDay.currentWeatherVariable);
        WeatherVariable2.Refresh(Imperium.TimeOfDay.currentWeatherVariable2);

        ProfitQuota.Refresh(Imperium.TimeOfDay.profitQuota);
        QuotaDeadline.Refresh(Imperium.TimeOfDay.daysUntilDeadline);
        GroupCredits.Refresh(Imperium.Terminal.groupCredits);
    }

    [ImpAttributes.LocalMethod]
    internal static void ChangeWeather(int levelIndex, LevelWeatherType weatherType)
    {
        Imperium.StartOfRound.levels[levelIndex].currentWeather = weatherType;

        RefreshWeather();

        var planetName = Imperium.StartOfRound.levels[levelIndex].PlanetName;
        var weatherName = weatherType.ToString();
        Imperium.Output.Send($"Successfully changed the weather on {planetName} to {weatherName}",
            notificationType: NotificationType.Confirmation);
    }

    [ImpAttributes.LocalMethod]
    private static void RefreshWeather()
    {
        Reflection.Invoke(Imperium.RoundManager, "SetToCurrentLevelWeather");
        Imperium.StartOfRound.SetMapScreenInfoToCurrentLevel();
        for (var i = 0; i < Imperium.TimeOfDay.effects.Length; i++)
        {
            var weatherEffect = Imperium.TimeOfDay.effects[i];
            var isEnabled = (int)Imperium.StartOfRound.currentLevel.currentWeather == i;
            weatherEffect.effectEnabled = isEnabled;
            if (weatherEffect.effectPermanentObject != null)
            {
                weatherEffect.effectPermanentObject.SetActive(value: isEnabled);
            }

            // The game doesn't usually remove the sun animator when switching from eclipsed to another weather
            if (!string.IsNullOrEmpty(weatherEffect.sunAnimatorBool) && !isEnabled && Imperium.TimeOfDay.sunAnimator)
            {
                Imperium.TimeOfDay.sunAnimator.SetBool(weatherEffect.sunAnimatorBool, value: false);
            }
        }

        // This prevents the player from permanently getting stuck in the underwater effect when turning
        // off flooded weather while being underwater
        if (Imperium.StartOfRound.currentLevel.currentWeather != LevelWeatherType.Flooded)
        {
            Imperium.Player.isUnderwater = false;
            Imperium.Player.sourcesCausingSinking = Mathf.Clamp(Imperium.Player.sourcesCausingSinking - 1, 0, 100);
            Imperium.Player.isMovementHindered = Mathf.Clamp(Imperium.Player.isMovementHindered - 1, 0, 100);
            Imperium.Player.hinderedMultiplier = 1;
        }
    }

    internal static void NavigateTo(int levelIndex)
    {
        Imperium.StartOfRound.ChangeLevelServerRpc(levelIndex, Imperium.Terminal.groupCredits);

        // Send scene refresh so moon related data is refreshed
        Imperium.IsSceneLoaded.Refresh();
    }

    internal static void ToggleDoors(bool isOn)
    {
        Imperium.ObjectManager.CurrentLevelDoors.Value
            .Where(obj => obj)
            .ToList()
            .ForEach(door => door.OpenOrCloseDoor(Imperium.Player));
    }

    internal static void ToggleDoorLocks(bool isOn)
    {
        Imperium.ObjectManager.CurrentLevelDoors.Value
            .Where(obj => obj)
            .ToList()
            .ForEach(door =>
            {
                if (isOn)
                {
                    door.LockDoor();
                }
                else
                {
                    door.UnlockDoor();
                }
            });
    }

    public static void ToggleSecurityDoors(bool isOn)
    {
        Imperium.ObjectManager.CurrentLevelSecurityDoors.Value
            .Where(obj => obj)
            .ToList()
            .ForEach(door => door.OnPowerSwitch(isOn));
    }

    public static void ToggleTurrets(bool isOn)
    {
        Imperium.ObjectManager.CurrentLevelTurrets.Value
            .Where(obj => obj)
            .ToList()
            .ForEach(turret => turret.ToggleTurretEnabled(isOn));
    }

    public static void ToggleLandmines(bool isOn)
    {
        Imperium.ObjectManager.CurrentLevelLandmines.Value
            .Where(obj => obj)
            .ToList()
            .ForEach(mine => mine.ToggleMine(isOn));
    }

    public static void ToggleBreakers(bool isOn)
    {
        Imperium.ObjectManager.CurrentLevelBreakerBoxes.Value
            .Where(obj => obj)
            .ToList()
            .ForEach(box =>
            {
                if (isOn)
                {
                    box.leversSwitchedOff = 0;
                }
                else
                {
                    box.SetSwitchesOff();
                }
            });
    }
}