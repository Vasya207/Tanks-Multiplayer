using System;
using System.Collections.Generic;
using System.Linq;
using Core.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Leaderboard
{
    public class Leaderboard : NetworkBehaviour
    {
        [SerializeField] private Transform leaderboardEntityHolder;
        [SerializeField] private LeaderboardEntityDisplay leaderboardEntityPrefab;

        private NetworkList<LeaderboardEntityState> _leaderboardEntities;
        private List<LeaderboardEntityDisplay> _entityDisplays = new List<LeaderboardEntityDisplay>();

        private void Awake()
        {
            _leaderboardEntities = new NetworkList<LeaderboardEntityState>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                _leaderboardEntities.OnListChanged += HandleLeaderboardEntitiesChanged;
                foreach (LeaderboardEntityState leaderboardEntity in _leaderboardEntities)
                {
                    HandleLeaderboardEntitiesChanged(new NetworkListEvent<LeaderboardEntityState>
                    {
                        Type = NetworkListEvent<LeaderboardEntityState>.EventType.Add,
                        Value = leaderboardEntity
                    });
                }
            }
            
            if (IsServer)
            {
                TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
                foreach (var player in players)
                {
                    HandlePlayerSpawned(player);
                }

                TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
                TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;
            }
        }

        private void HandleLeaderboardEntitiesChanged(NetworkListEvent<LeaderboardEntityState> changeEvent)
        {
            switch (changeEvent.Type)
            {
                case NetworkListEvent<LeaderboardEntityState>.EventType.Add:
                    if (!_entityDisplays.Any(x => x.ClientId == changeEvent.Value.ClientId))
                    {
                        LeaderboardEntityDisplay leaderboardEntity = 
                            Instantiate(leaderboardEntityPrefab, leaderboardEntityHolder);
                        leaderboardEntity.Initialise(
                            changeEvent.Value.ClientId, 
                            changeEvent.Value.PlayerName,
                            changeEvent.Value.Coins);
                        _entityDisplays.Add(leaderboardEntity);
                    }
                    break;
                case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:
                    LeaderboardEntityDisplay displayToRemove = 
                        _entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);

                    if (displayToRemove != null)
                    {
                        displayToRemove.transform.SetParent(null);
                        Destroy(displayToRemove.gameObject);
                        _entityDisplays.Remove(displayToRemove);
                    }
                    break;
                case NetworkListEvent<LeaderboardEntityState>.EventType.Value:
                    LeaderboardEntityDisplay displayToUpdate = 
                        _entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);

                    if (displayToUpdate != null)
                    {
                        displayToUpdate.UpdateCoins(changeEvent.Value.Coins);
                    }
                    break;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsClient)
            {
                _leaderboardEntities.OnListChanged -= HandleLeaderboardEntitiesChanged;
            }
            
            TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
        }

        private void HandlePlayerSpawned(TankPlayer player)
        {
            _leaderboardEntities.Add(new LeaderboardEntityState()
            {
                ClientId = player.OwnerClientId,
                PlayerName = player.PlayerName.Value,
                Coins = 0
            });
        }

        private void HandlePlayerDespawned(TankPlayer player)
        {
            if (_leaderboardEntities == null) return;
            
            foreach (var entity in _leaderboardEntities)
            {
                if (entity.ClientId != player.OwnerClientId) continue;

                _leaderboardEntities.Remove(entity);
                break;
            }
        }
    }
}
