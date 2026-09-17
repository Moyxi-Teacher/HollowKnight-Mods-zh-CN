
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Satchel.EnemyUtils;
using static Satchel.GameObjectUtils;

namespace Konpanion
{
    public enum State {
        Idle = 0,
        IdleFidget1,
        IdleFidget2,
        Turn,
        Walk,
        Jump,
        Teleport
    }
    public enum Direction {
        Left = 0,
        Right
    }
    public class CompanionControl : MonoBehaviour
    {

        public bool isNetworkControlled = false;
        
        public tk2dSpriteAnimator animator;
        public Rigidbody2D rb;
        public BoxCollider2D collider;
        public AudioSource audioSource;

        public float scale = 0.6f;
        public float moveSpeed = 15f;
        public float IdleShuffleDistance = 0.1f;

        public GameObject followTarget;
        public float followDistance = 2f;
        public float teleportDistance = 15f;

        public AudioClip teleport,walk,yay;
        public Dictionary<State,string> Animations = new Dictionary<State,string>();

        public float followClipChance = 0.03f, teleportClipChance =  0.60f,turnClipChance = 30f, yayClipChance = 0.01f;
        internal State state = State.Idle;
        internal Direction lookDirection = Direction.Left;

        private bool changeDirection = false;
        internal bool moveToNext = true;
        
        public GameObject getFollowTarget(){
            if(followTarget == null){
                // 回主菜单 / 读档时 HeroController.instance 可能已销毁，直接取 .gameObject
                // 会抛空引用；而 MainLoop 是协程，协程内抛异常会**整个终止** ——
                // 表现就是"伴侣从此不再跟随、不动"。这里安全回退到自身，
                // 且**不缓存**回退值，等英雄回来会自动重新绑定。
                var hc = HeroController.instance;
                if(hc == null || hc.gameObject == null) return gameObject;
                followTarget = hc.gameObject;
            }
            return followTarget;
        }

        void Start(){
            collider = gameObject.GetComponent<BoxCollider2D>();
            rb = gameObject.GetAddComponent<Rigidbody2D>();
            animator = gameObject.GetComponent<tk2dSpriteAnimator>();
            audioSource = gameObject.GetAddComponent<AudioSource>();

            rb.bodyType = RigidbodyType2D.Dynamic;
            collider.isTrigger = false;
            gameObject.SetScale(scale,scale);
            StartCoroutine(MainLoop());
        }

        public void playAnim(string clip,bool onlyNew = false) {
            if(animator != null){
                if(!onlyNew || !animator.IsPlaying(clip)){
                    animator.PlayFromFrame(clip, 0);
                }
            }
        }

        public void playAnimForState(bool onlyNew = false){
            if(Animations.TryGetValue(state, out var anim)){
                playAnim(anim,onlyNew);
            } else if(Animations.TryGetValue(State.Idle, out var fallbackAnim)){
                playAnim(fallbackAnim,onlyNew);
            }
        }

        private bool heroFartherThan(float distance){
            var displacement = getFollowTarget().transform.position - transform.position;
            return displacement.magnitude > distance;
        }

        private bool heroFartherThanY(float distance){
            var displacement = getFollowTarget().transform.position - transform.position;
            return displacement.y > distance;
        }

        private bool UpdateLookDirection(){
           Direction newLookDirection;
           changeDirection = false;

           if(getFollowTarget().transform.position.x > transform.position.x){
               newLookDirection = Direction.Right;
           } else {
               newLookDirection = Direction.Left;
           }
           if(newLookDirection != lookDirection){
               changeDirection = true;
               lookDirection = newLookDirection;
           }
           return changeDirection;
        }

        private void fixRotation(){
            //fix rotation
            gameObject.transform.rotation = Quaternion.identity;
        }

        private IEnumerator TurnToHero(){
           fixRotation();
           playAnimForState();
           if(teleport != null && !audioSource.isPlaying && Random.Range(0.0f, 1.0f) < turnClipChance){
               audioSource.PlayOneShot(teleport);
           }
           yield return new WaitForSeconds(0.5f); // atleast stay here for this duration
           var ls = gameObject.transform.localScale;
           ls.x = (lookDirection == Direction.Left? 1f : -1f)*Mathf.Abs(ls.x);
           gameObject.transform.localScale = ls;
           state = State.Idle;
           moveToNext = true;
        }

        private void decideNextState(){
            var shouldFollowTarget = true;
            if(getFollowTarget() == HeroController.instance){
                shouldFollowTarget = HeroController.instance.cState.onGround || Random.Range(0.0f, 1.0f) < 0.3f;
            }

            if(UpdateLookDirection()){
                state = State.Turn;
            } else if(heroFartherThan(teleportDistance) && shouldFollowTarget){
                state = State.Teleport;
            } else if(heroFartherThan(followDistance) && shouldFollowTarget){
                if(heroFartherThanY(0.5f) && Random.Range(0.0f, 1.0f) < 0.3f){
                    state = State.Jump;
                } else {
                    state = State.Walk;
                }
            } else {
                state = State.Idle;
            }
        }
        private IEnumerator Idle(){
            fixRotation();
            playAnimForState(true);
            if(yay != null && !audioSource.isPlaying && Random.Range(0.0f, 1.0f) < yayClipChance){
                audioSource.PlayOneShot(yay);
            }
            rb.velocity = new Vector2(0.0f, 0.0f);

            //var force = new Vector2(Random.Range(-1.0f, 1.0f),Random.Range(-1.0f, 1.0f));
            //yield return rb.moveTowards(-force,IdleShuffleDistance,0.2f);
            
            var lastState = state;
            decideNextState();
            if(state == State.Idle && (lastState == State.IdleFidget1 || lastState == State.IdleFidget2)){
                state = lastState;
                yield return new WaitForSeconds(0.1f); // atleast stay idle for this duration
            } else if(lastState == State.Idle){
                var roll = Random.Range(0.0f, 1.0f);
                if(roll < 0.001f){
                    state = State.IdleFidget1;
                }else if(roll < 0.011f){
                    state = State.IdleFidget2;
                }
                yield return new WaitForSeconds(0.1f); // atleast stay idle for this duration
            } else {
                if(!(lastState == state && state == State.Walk)){
                    yield return new WaitForSeconds(1f); // atleast stay idle for this duration
                }
            }
            moveToNext = true;

        }
        private IEnumerator Teleport(){
            //play Teleport animation
            fixRotation();
            if(teleport != null && !audioSource.isPlaying && Random.Range(0.0f, 1.0f) < teleportClipChance){
                audioSource.PlayOneShot(teleport);
            }
            playAnimForState();
            var deltaToPlayer = Random.Range(-0.5f, 0.5f);
            if(Random.Range(0.0f, 1.0f) < 0.50f){
                gameObject.transform.position = getFollowTarget().transform.position + new Vector3(0.5f + deltaToPlayer,0f,0f);
            } else {
                gameObject.transform.position = getFollowTarget().transform.position + new Vector3(-0.5f + deltaToPlayer,0f,0f);
            }
            yield return new WaitForSeconds(0.1f); // atleast stay here for this duration

            state = State.Idle;
            moveToNext = true;
        }

        private IEnumerator Follow(){
            //play follow animation
            playAnimForState();
            if(walk != null && !audioSource.isPlaying && Random.Range(0.0f, 1.0f) < followClipChance){
                audioSource.PlayOneShot(walk);
            }

            //get displacement to player
            Vector2 displacement;
            displacement = getFollowTarget().transform.position - transform.position;
            displacement += new Vector2(Random.Range(-0.01f, 0.01f),Random.Range(-0.01f, 0.01f));
            if(state == State.Jump){
                displacement += new Vector2(0,3f);
            }
            var followDistanceR = followDistance * (1f+Random.Range(-0.25f, 0.50f));
            var distance = Mathf.Min(teleportDistance,displacement.magnitude-followDistanceR);
            var moveSpeedR = moveSpeed * (1f+Random.Range(-0.25f, 0.50f));
            yield return rb.moveTowards(displacement,distance*0.5f,distance*0.5f/moveSpeedR);
            playAnimForState();
            yield return rb.moveTowards(displacement,distance*0.5f,distance*0.5f/moveSpeedR);


            state = State.Idle;
            moveToNext = true;
        }
        public Vector2 networkMovementTarget = new Vector2(0,0);
        
        public Coroutine networkCoro;
        private IEnumerator ApplyNetworkState(){
            yield return null;
            // play animation
            playAnimForState();
            // update direction
            var ls = gameObject.transform.localScale;
            ls.x = (lookDirection == Direction.Left? 1f : -1f)*Mathf.Abs(ls.x);
            gameObject.transform.localScale = ls;
            //interpolate movement
            var value = 0.1f;
            if(state == State.Teleport){
                transform.position = networkMovementTarget;
            } else {
                while((Vector2)transform.position != networkMovementTarget){
                    transform.position = Vector2.Lerp(transform.position, networkMovementTarget, value);
                    value += 0.1f;
                    yield return new WaitForSeconds(0.02f);
                }
            }
        }
        internal void UpdateNetworkCoro(){
            if(networkCoro != null){
                StopCoroutine(networkCoro);
            }
            networkCoro = StartCoroutine(ApplyNetworkState());
        }

        private IEnumerator MainLoop(){
            while(true){
                yield return new WaitWhile(()=>!moveToNext);
                moveToNext = false;
                if (isNetworkControlled)
                {
                    continue;
                }
                if (Konpanion.HasPouch()) { 
                    PouchIntegration.SendUpdate(this);
                }

                if (state == State.Idle || state == State.IdleFidget1 || state == State.IdleFidget2){
                    StartCoroutine(Idle());
                } else if(state == State.Walk){
                    StartCoroutine(Follow());
                } else if(state == State.Jump){
                    StartCoroutine(Follow());
                } else if(state == State.Teleport){
                    StartCoroutine(Teleport());
                } else if(state == State.Turn){
                    StartCoroutine(TurnToHero());
                }
            }
        }
    }
}