using System;
using System.Collections;
using CastlePrototype.Battle.Visuals.Effects;
using OneDay.Core.Modules.Pooling;
using TowerDefensePrototype.Battle.Visuals.Effects;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CastlePrototype.Battle.Visuals
{
    public class VisualObject : MonoBehaviour, IPoolable
    {
        public string Key { get; set; }
        
        [Header("Id")]
        [SerializeField] private string id;

        [Header("Modules")]
        [SerializeField] private AnimationModule animationModule;
        [SerializeField] private EffectModule effectModule;

        [Header("Progress bars")]
        [SerializeField] private ProgressBar hpProgressBar;
        [SerializeField] private ProgressBar cooldownProgressBar;
       
        [Header("Settings")]
        [SerializeField] private float defaultHeight;
        [SerializeField] private bool permanentHpBar;
        [Tooltip("This transform's rotation will be rotated, by default root transform, but could be maze as well")]
        [SerializeField] private Transform objectToRotate;
        
        public string Id => id;
        public float DefaultHeight => defaultHeight;
        public int Index { get; private set; }

        private void Awake()
        {
            effectModule = GetComponent<EffectModule>();
            if (objectToRotate == null)
            {
                objectToRotate = transform;
            }
        }
       
        public void SetPosition(float3 position) => transform.position = position + new float3(0,defaultHeight,0);
        public void SetRotation(quaternion rotation) => objectToRotate.rotation = rotation;

        public void SetMoveSpeed(float moveSpeed)
        {
            if (animationModule != null)
            {
                animationModule.SetFloat(AnimationModule.Speed, moveSpeed);
            }
        }
        public void Attack()
        {
            if (animationModule != null)
            {
                animationModule.PlayAttack();
            }

            if (effectModule != null)
            {
                effectModule.PlayEffect("Attack");
            }
        }


        public void ShowDamage(float amount)
        {
            VisualManager.Default.PlayEffect(EffectKeys.HpDamageText, transform.position, amount);
        }
        
        public void Die(bool immediate, Action onFinished)
        {
            if (hpProgressBar != null)
            {
                hpProgressBar.ReturnFromCanvas();
                hpProgressBar.enabled = false;
            }
            if (cooldownProgressBar != null)
            {
                cooldownProgressBar.ReturnFromCanvas();
                cooldownProgressBar.enabled = false;
            }

            if (immediate || animationModule == null)
            {
                onFinished();
            }
            else
            { 
                animationModule.PlayDeath();
                StartCoroutine(RunIn(onFinished, 2.0f));
            }
        }

        public void SetAttackCooldown(float progress01)
        {
            if (cooldownProgressBar == null)
                return;
            
            cooldownProgressBar.SetProgress(progress01);
        }
        
        public void SetHp(float progress01)
        {
            if (hpProgressBar == null)
                return;
            
            hpProgressBar.gameObject.SetActive(permanentHpBar || progress01 < 1);
            hpProgressBar.SetProgress(progress01);
        }

        public void OnGetFromPool()
        {
            if (VisualManager.Default == null)
                return;
            
            Index = VisualManager.Default.TrackVisualObject(this);
            
            if (hpProgressBar != null)
            {
                hpProgressBar.PlaceToCanvas(transform);
                hpProgressBar.enabled = true;
            }
            if (cooldownProgressBar != null)
            {
                cooldownProgressBar.PlaceToCanvas(transform);
                cooldownProgressBar.enabled = true;
            }
        }

        public void OnReturnToPool()
        {
            if (hpProgressBar != null)
            {
                hpProgressBar.ReturnFromCanvas();
            }
            if (cooldownProgressBar != null)
            {
                cooldownProgressBar.ReturnFromCanvas();
            }
            
            
            if (VisualManager.Default == null)
                return;
            
            VisualManager.Default.UnTrackVisualObject(this);
        }

        private IEnumerator RunIn(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action();
        }
    }
}