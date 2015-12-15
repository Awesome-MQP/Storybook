using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code.Scripts.Game.Player
{
    public class PlayerManager : Photon.PunBehaviour
    {
        private static PlayerManager s_instance;

        private Dictionary<PhotonPlayer, PlayerEntity> m_playerLookup = new Dictionary<PhotonPlayer, PlayerEntity>();

        [SerializeField]
        private ResourceAsset m_playerEntityPrefab = new ResourceAsset(typeof(PlayerEntity));

        public static PlayerManager Instance
        {
            get { return s_instance; }
        }

        /// <summary>
        /// Gets a player entity from a photon player.
        /// </summary>
        /// <param name="player">The photon player of the player entity.</param>
        /// <returns>The player entity registered to this photon player.</returns>
        public PlayerEntity this[PhotonPlayer player]
        {
            get
            {
                PlayerEntity playerEntity;
                m_playerLookup.TryGetValue(player, out playerEntity);
                return playerEntity;
            }
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            if (!IsMine)
                return;

            PlayerEntity newPlayerEntity = PhotonNetwork.Instantiate<PlayerEntity>(m_playerEntityPrefab, Vector3.zero, Quaternion.identity, 0);
            newPlayerEntity.Construct(newPlayer);
        }

        public virtual void RegisterPlayer(PlayerEntity playerEntity)
        {
            PhotonPlayer representedPlayer = playerEntity.RepresentedPlayer;
            if (!m_playerLookup.ContainsKey(representedPlayer))
                m_playerLookup.Add(representedPlayer, playerEntity);
        }

        public virtual void UnregisterPlayer(PlayerEntity playerEntity)
        {
            PhotonPlayer representedPlayer = playerEntity.RepresentedPlayer;
            if (m_playerLookup.ContainsKey(representedPlayer))
                m_playerLookup.Remove(representedPlayer);
        }
    }
}
