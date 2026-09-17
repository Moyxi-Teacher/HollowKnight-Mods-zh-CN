using Modding.Utils;
using System;
using System.Collections;
using UnityEngine;

namespace InfectedHallownest
{
    /// <summary>
    /// Component to mark an enemy as extra infected, causing them to periodically flash orange
    /// </summary>
    public class Infection : MonoBehaviour
    {
        /// <summary>
        /// Enemy's health tracker so we know when they're dead
        /// </summary>
        private HealthManager healthManager;

        public void Awake()
        {
            healthManager = this.gameObject.GetComponent<HealthManager>();
            StartCoroutine(FlashInfection());
        }

        /// <summary>
        /// Handles the Sprite Flash so its clear that a character is infected
        /// </summary>
        /// <returns></returns>
        private IEnumerator FlashInfection()
        {
            while (!healthManager.isDead)
            {
                try
                {
                    SpriteFlash spriteFlash = this.gameObject.GetOrAddComponent<SpriteFlash>();
                    spriteFlash.flashInfected();
                }
                catch (Exception e)
                {
                    InfectedHallownest.Instance.Log($"Error flashing: {e.Message}\n\n{e.StackTrace}");
                }

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}