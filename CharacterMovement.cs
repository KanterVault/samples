using System;
using UnityEngine;
using Inventory.Model;
using System.Collections;
using Random = UnityEngine.Random;

namespace PlayerFunctions.PersonController
{
    public class CharacterMovement : MonoBehaviour
    {
        private void Awake() => SRef.CharacterMovement = this;
        
        public int viewType;
        public bool shiftL;
        public bool isJump;
        public bool isFlying;
        public int flyMode;
        public bool magic;
        public bool backWalk;
        public bool mouse0;
        public bool mouse1;
        public bool isJumpFall;
        public bool isGrounded = true;
        public bool walkOrTrot = true;
        public bool walkType = false;
        public bool isFastRotate;
        public bool movementAllow = true;
        public bool sitMovementAllow = true;
        public bool chatMovementAllow = true;
        public bool inventoryMovementAllow = true;
        public bool statMaxWeight = false;
        public bool statEnergyOut = false;

        private bool CheckFlyAllow()
        {
            if (statMaxWeight || statEnergyOut) return true;
            if (SRef.Authorization != null && SRef.Authorization.MainPlayerViewData.Race < 2)
            {     
                flyMode = 0;
                return true;
            }
            if (movementAllow && sitMovementAllow && chatMovementAllow &&
                SRef.ModelTransferViews.CurrentSitState == 0) return false;
            flyMode = 0;
            return true;
        }
        private void Magic()
        {
            if (SRef.Authorization.MainPlayerViewData.Race == 1) magic = !magic;
            else magic = false;

            SRef.ModelTransferViews.hoofs.MagicEffectEnable(magic);
        }
        public void BUTTON_Magic() => Magic();
        public void BUTTON_Flying() { if (CheckFlyAllow()) return; isFlying = !isFlying; }
        public void BUTTON_FLY_UP_upKey() { if (CheckFlyAllow()) return; flyMode = flyMode == 1 ? 0 : flyMode; }
        public void BUTTON_FLY_UP_downKey() { if (CheckFlyAllow()) return; flyMode = 1; }
        public void BUTTON_FLY_DOWN_upKey() { if (CheckFlyAllow()) return; flyMode = flyMode == -1 ? 0 : flyMode; }
        public void BUTTON_FLY_DOWN_downKey() { if (CheckFlyAllow()) return; flyMode = -1; }
        public void BUTTON_VIEW() => SRef.CamRotate.whellSpeed = SRef.CamRotate.whellSpeed < 2.4f ? 3.0f : 0.0f;
        public void BUTTON_BackWalk_Up() => backWalk = false;
        public void BUTTON_BackWalk_Down() => backWalk = true;
        public void BUTTON_CTRL() => walkType = !walkType;
        public void BUTTON_Shift_Down() { if (statMaxWeight || statEnergyOut) return; if (movementAllow && sitMovementAllow && chatMovementAllow) shiftL = true; }
        public void BUTTON_Shift_Up() { if (statMaxWeight || statEnergyOut) return; shiftL = false; }
        public void BUTTON_MOUSE_0_Down() => mouse0 = true;
        public void BUTTON_MOUSE_0_Up() => mouse0 = false;
        public void BUTTON_MOUSE_1_Down() => mouse1 = true;
        public void BUTTON_MOUSE_1_Up() => mouse1 = false;
        public void BUTTON_MOUSE_1() => mouse1 = !mouse1;
        public void BUTTON_LOCK_MODE()
        {
            if (SRef.CamRotate.mobileRotate) EnbLockMode();
            else DisLockMode();
        }
        public void BUTTON_JUMP()
        {
            if (!movementAllow || !sitMovementAllow || !chatMovementAllow || !isGrounded || isJump || backWalk || statMaxWeight || statEnergyOut) return;
            isJump = true;
        }

        public static void DisLockMode()
        {
            Cursor.lockState = CursorLockMode.None;  
            SRef.CamRotate.mobileRotate = true;
            Bootstrap.LookMode = false;
        }

        public static void EnbLockMode()
        {
            Cursor.lockState = CursorLockMode.Locked;
            SRef.CamRotate.mobileRotate = false;
            Bootstrap.LookMode = true;
        }
        
        [Serializable]
        public class LerpMul
        {
            public float animationStep;
            public float forwardTilt;
            public float speedMovement;
            public float moveVelocitySpeed;
            public float speedMovVectorRotation;
            public float fastRotateCoif;
        }
        [Space(20.0f)] public LerpMul[] listLerpMul = {
            new LerpMul()
            {
                animationStep = 1.0f,
                forwardTilt = 1.0f,
                speedMovement = 1.0f,
                moveVelocitySpeed = 8.0f,
                speedMovVectorRotation = 140.0f,
                fastRotateCoif = 300.0f
            },
            new LerpMul()
            {
                animationStep = 2.0f,
                forwardTilt = 1f,
                speedMovement = 2.75f,
                moveVelocitySpeed = 6.0f,
                speedMovVectorRotation = 220.0f,
                fastRotateCoif = 500.0f
            },
            new LerpMul()
            {
                animationStep = 3.0f,
                forwardTilt = 2.0f,
                speedMovement = 6.0f,
                moveVelocitySpeed = 3.0f,
                speedMovVectorRotation = 240.0f,
                fastRotateCoif = 0
            },
            new LerpMul()
            {
                animationStep = 1.0f,
                forwardTilt = 2.0f,
                speedMovement = 2.0f,
                moveVelocitySpeed = 6.0f,
                speedMovVectorRotation = 400.0f,
                fastRotateCoif = 0
            },
            new LerpMul()
            {
                animationStep = 3.0f,
                forwardTilt = 10.0f,
                speedMovement = 8.0f,
                moveVelocitySpeed = 1.0f,
                speedMovVectorRotation = 150.0f,
                fastRotateCoif = 0
            }
        };
        [HideInInspector] public LerpMul SelfLerpMul = new LerpMul();
        [HideInInspector] public int CurrentMovementState;
        private const float SwitcherLerp = 4.0f;
        private void SetUpSpeedLerp()
        {
            SelfLerpMul.fastRotateCoif = listLerpMul[CurrentMovementState].fastRotateCoif;
            SelfLerpMul.animationStep = listLerpMul[CurrentMovementState].animationStep;
            SelfLerpMul.forwardTilt = Mathf.Lerp(SelfLerpMul.forwardTilt, listLerpMul[CurrentMovementState].forwardTilt, SwitcherLerp * Time.deltaTime);
            SelfLerpMul.speedMovement = Mathf.Lerp(SelfLerpMul.speedMovement, listLerpMul[CurrentMovementState].speedMovement, SwitcherLerp * Time.deltaTime);
            SelfLerpMul.moveVelocitySpeed = Mathf.Lerp(SelfLerpMul.moveVelocitySpeed, listLerpMul[CurrentMovementState].moveVelocitySpeed, SwitcherLerp * Time.deltaTime);
            SelfLerpMul.speedMovVectorRotation = Mathf.Lerp(SelfLerpMul.speedMovVectorRotation, listLerpMul[CurrentMovementState].speedMovVectorRotation, SwitcherLerp * Time.deltaTime);
        }

        private Transform directionVector;
        [HideInInspector] public Vector2 MoveInputForce = Vector2.zero;
        [HideInInspector] public Vector2 MoveInputForceForAnimation = Vector2.zero;
        [HideInInspector] public float MovementVelocity;
        private void MovementDirectionVector()
        {
            var rotation = directionVector.rotation;
            var toRotate =
                Quaternion.LookRotation(Vector3.ProjectOnPlane(SRef.CamRotate.transform.forward * 1000.0f, Vector3.up), Vector3.up) *
                Quaternion.LookRotation(Vector3.right * MoveInputForce.x * 1000.0f + Vector3.forward * MoveInputForce.y * 1000.0f);
            
            if (Quaternion.Angle(rotation, toRotate) > 90.0f + 85.0f && SelfLerpMul.fastRotateCoif > 0.1f) isFastRotate = true;
            else if (isFastRotate && Quaternion.Angle(rotation, toRotate) < 5.0f) isFastRotate = false;

            rotation = isFastRotate ?
                Quaternion.RotateTowards(rotation, toRotate, SelfLerpMul.fastRotateCoif * Time.deltaTime) :
                Quaternion.RotateTowards(rotation, toRotate, SelfLerpMul.speedMovVectorRotation * MovementVelocity * Time.deltaTime);

            directionVector.position = controller.transform.position;
            directionVector.rotation = rotation;
        }

        [Space(20.0f)]
        [HideInInspector] public float gravity;
        [SerializeField] private AnimationCurve curveJump;
        public CharacterController controller;
        private Vector3 motion = Vector3.zero;
        [SerializeField] private float jumpForce = 0.07f;
        [SerializeField] private float jumpTime = 0.04f;
        [SerializeField] private float gravityForce = 0.0035f;

        private readonly WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        private IEnumerator Jump()
        {
            var jumpTimer = 0.0f;
            while (true)
            {
                jumpTimer += jumpTime;
                gravity = curveJump.Evaluate(jumpTimer) * jumpForce;
                if (jumpTimer >= 1.0f || isFlying)
                {
                    isJumpFall = false;
                    yield break;
                }

                yield return waitForFixedUpdate;
            }
        }
        
        [SerializeField] private float flyGravityLerp2 = 0.04f;
        [SerializeField] private float flyGravityMaxForce1 = 0.05f;
        [SerializeField] private float flyGravityMaxForce2 = 0.16f;
        [SerializeField] private float maxMoveMagnitude = 0.2f;
        [SerializeField] private float wallOffsetForce = 0.03f;
        [SerializeField] private float maxYSideNormal = 0.8f;

        private float _flyRandomTimer;
        private Vector3 _randomVector;
        private bool _isGroundedGravityTriggerOnce;
        private void MovementController()
        {
            if (isGrounded || isFlying)
            {
                if (backWalk && CurrentMovementState == 0)
                {
                    motion = Vector3.Lerp(
                        motion,
                        controller.transform.forward * (-1.0f * (MovementVelocity * SelfLerpMul.speedMovement)),
                        //controller.transform.forward * MovementVelocity * SelfLerpMul.speedMovement,
                        0.1f);
                }
                else motion = controller.transform.forward * (MovementVelocity * SelfLerpMul.speedMovement);
                controller.transform.rotation = directionVector.rotation;
            }
            else motion = Vector3.Lerp(motion, Vector3.zero, 0.002f);

            if (isFlying)
            {
                _flyRandomTimer += 0.1f;
                if (_flyRandomTimer > Random.Range(1.0f, 6.0f))
                {
                    _flyRandomTimer = 0.0f;
                    _randomVector =
                        Vector3.right * Random.Range(-16, 17) +
                        Vector3.up * Random.Range(-16, 17) +
                        Vector3.forward * Random.Range(-16, 17);
                }

                motion *= Mathf.Cos(Mathf.Deg2Rad * SRef.ModelTransferViews.bodyGravityAngle);
                if (isGrounded) motion += Vector3.down * 0.1f;
                else motion = Vector3.Lerp(motion, motion + _randomVector, 0.01f);
            }

            isGrounded = SRef.ModelTransferViews.downRayDistance < 0.1f ||
                (controller.collisionFlags == (CollisionFlags.Sides | CollisionFlags.Below) ||
                controller.collisionFlags == CollisionFlags.Below ||
                (int)controller.collisionFlags == 7);

            controller.Move(Vector3.ClampMagnitude(motion * 0.02f + Vector3.up * gravity, maxMoveMagnitude));
            if (!isFlying && SRef.CharacterCollisionDetect.CollisionNormal.y < maxYSideNormal)
            {
                if (isJump) isJump = false;
                gravity = Mathf.Clamp(gravity, -0.2f, 0.2f);
                controller.Move(Vector3.ClampMagnitude(
                    (Vector3.ProjectOnPlane(
                        SRef.CharacterCollisionDetect.CollisionNormal,
                        Vector3.up) * 1000.0f).normalized *
                    wallOffsetForce + Vector3.up * Mathf.Clamp(gravity, -100.0f, 0.0f), maxMoveMagnitude));
            }

            if (SRef.Authorization != null && SRef.Authorization.MainPlayerViewData.Race < 2) isFlying = false;
            if (isFlying)
            {
                SRef.CharacterCollisionDetect.CollisionNormal = Vector3.up;
                isJump = false;

                gravity = Mathf.Lerp(
                    gravity,

                    (flyMode * (shiftL ? flyGravityMaxForce2 : flyGravityMaxForce1)) +
                    (shiftL ? ((SRef.CamRotate.angY * -0.002f) + 0.025f) : 0.0f),

                    flyGravityLerp2);
            }

            if (!isJumpFall && !isFlying)
            {
                if (!isJump || !isGrounded)
                {
                    isJump = false;
                }
                else
                {
                    isJump = false;
                    isJumpFall = true;
                    StartCoroutine(Jump());
                }
            }

            if (isFlying)
            {
                if (isGrounded)
                {
                    gravity = Mathf.Clamp(gravity, -0.06f, 10000.0f);
                }
                _isGroundedGravityTriggerOnce = false;
            }
            else
            {
                if (isGrounded)
                {
                    gravity = -0.06f;
                    _isGroundedGravityTriggerOnce = true;
                }
                else
                {
                    if (_isGroundedGravityTriggerOnce)
                    {
                        _isGroundedGravityTriggerOnce = false;
                        gravity = 0.0f;
                    }
                    gravity = gravity - gravityForce;
                }
            }
        }
        
        private void SetUpMovementDirection()
        {
            directionVector = new GameObject("DirectionVector").transform;
            directionVector.SetParent(this.transform, false);
        }
        
        private void Start()
        {
            SetUpMovementDirection();
//#if UNITY_STANDALONE_WIN
//            BUTTON_LOCK_MODE();
//#endif
        }

        private void FixedUpdate()
        {
            MovementController();
            SRef.ModelTransferViews.ForwardBodyRotation();
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
            
            //Unload references
            SRef.CharacterMovement = null;
            SRef.ModelTransferViews = null;
            SRef.CamRotate = null;
            SRef.Chat = null;
            SRef.Joystick = null;
        }

        public void MovePlayerToPosition(PlayerPositionPackage posPack)
        {
            controller.enabled = false;

            SRef.CharacterMovement.isFlying = posPack.IsFly;
            SRef.CharacterMovement.magic = posPack.Magic == 1;

            SRef.ModelTransferViews.BUTTON_Up();
            SRef.ModelTransferViews.BUTTON_Up();
            SRef.ModelTransferViews.BUTTON_Up();

            magic = false; //posPack.Magic == 1;
            gravity = 0.0f;
            motion = Vector3.zero;
            MovementVelocity = 0.0f;                      
            MoveInputForce = Vector2.zero;
            MoveInputForceForAnimation = Vector2.zero;
            directionVector.position = posPack.ModelRootPos + Vector3.up * 0.26f;
            directionVector.rotation = Quaternion.AngleAxis(posPack.ModelRootRot.y, Vector3.up);
            controller.transform.position = posPack.ModelRootPos + Vector3.up * 0.26f;
            controller.transform.rotation = Quaternion.AngleAxis(posPack.ModelRootRot.y, Vector3.up);

            SRef.ModelTransferViews.ResetRotations();
            SRef.ModelTransferViews.hoofs.MagicEffectEnable(magic);
            SRef.ModelTransferViews.modelRoot.position = posPack.ModelRootPos + Vector3.up * 0.26f;
            SRef.ModelTransferViews.modelRoot.rotation = Quaternion.AngleAxis(posPack.ModelRootRot.y, Vector3.up);
            SRef.CamRotate.ResetCameraPosition();

            controller.enabled = true;

            //Logger.Log(this, $"MovePlayerToPosition: { posPack.ModelRootPos}");
        }

        private float _statMaxWeightTimer;
        private float _shiftTimer;
        private bool _shiftTriggerOnce_OneClick = true;
        private bool _shiftTriggerOnce_Pressed = true;
        private float _flySpaceTimer;
        private short _spaceButtonCounter = 0;
        private float _backWalkTimer = 0.0f;

        private void Update()
        {
            if (statMaxWeight)
            {
                walkOrTrot = true;
                isFlying = false;
                shiftL = false;
                isJump = false;
                if (_statMaxWeightTimer < Time.time)
                {
                    _statMaxWeightTimer = Time.time + 10.0f;
                    SRef.PlayerStatsAPI.PushNotify(LangTable.GetActualWords("WarningPlayerMaxWeight"));
                }
            }
            if (statEnergyOut)
            {
                magic = false;
                walkOrTrot = true;
                isFlying = false;
                shiftL = false;
                isJump = false;
                SRef.ClientData.PlayerStats.Energy.x = 0.0f;
                SRef.ModelTransferViews.hoofs.MagicEffectEnable(magic);

                if (_statMaxWeightTimer < Time.time)
                {
                    _statMaxWeightTimer = Time.time + 10.0f;
                    SRef.PlayerStatsAPI.PushNotify(LangTable.GetActualWords("WarningPlayerEnergyMiss"));
                }
            }
            if (Input.GetKeyUp(KeyCode.Escape)) DisLockMode();
            if (movementAllow && chatMovementAllow && inventoryMovementAllow)
            {
                if (SRef.CharacterMovement.MoveInputForceForAnimation.magnitude > 0.5f &&
                    SRef.ModelTransferViews.CurrentSitState != 0) SRef.ModelTransferViews.BUTTON_Up();

                //if (Input.GetKeyDown(KeyCode.LeftControl)) BUTTON_CapsL();
                if (Input.GetKeyDown(KeyCode.V)) BUTTON_VIEW();
                if (Input.GetKeyDown(KeyCode.X)) SRef.ModelTransferViews.BUTTON_Down();

                if (Input.GetKeyDown(KeyCode.B)) SRef.ModelTransferViews.BUTTON_Boop();
                if (Input.GetKeyDown(KeyCode.H)) SRef.ModelTransferViews.BUTTON_BackHit();
                
                if (Input.GetKeyUp(KeyCode.C))
                {
                    BUTTON_FLY_DOWN_upKey();
                    BUTTON_BackWalk_Up();
                    SRef.ModelTransferViews.BUTTON_Up();
                }
                if (Input.GetKeyDown(KeyCode.C))
                {
                    _backWalkTimer = Time.time + 0.65f;
                    BUTTON_FLY_DOWN_downKey();
                    BUTTON_BackWalk_Down();
                }

                if (Input.GetKeyDown(KeyCode.M)) BUTTON_Magic();
                if (Input.GetKeyDown(KeyCode.F)) BUTTON_Flying();

                if (Input.GetKeyDown(KeyCode.LeftControl)) BUTTON_CTRL();

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _flySpaceTimer = Time.time + 0.3f;  
                    _spaceButtonCounter++;
                    BUTTON_JUMP();
                    BUTTON_FLY_UP_downKey();
                }
                if (Input.GetKeyUp(KeyCode.Space)) { BUTTON_FLY_UP_upKey(); }
  
                if (_flySpaceTimer < Time.time || _spaceButtonCounter >= 2)
                {
                    if (_spaceButtonCounter >= 2)
                    {
                        //if (!CheckFlyAllow()) isFlying = !isFlying;
                        if (!CheckFlyAllow() && !isFlying) isFlying = true;
                    }
                    _spaceButtonCounter = 0;
                }

                if (Input.GetKeyDown(KeyCode.LeftShift)) _shiftTimer = Time.time + 0.25f;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (_shiftTimer > Time.time) _shiftTriggerOnce_OneClick = true;
                    else if (_shiftTimer < Time.time && _shiftTriggerOnce_Pressed)
                    {
                        _shiftTriggerOnce_Pressed = false;
                        BUTTON_Shift_Down();
                        walkOrTrot = !walkOrTrot;
                    }
                }
                if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    if (_shiftTriggerOnce_OneClick)
                    {
                        walkOrTrot = !walkOrTrot;
                    }
                    if (!_shiftTriggerOnce_Pressed)
                    {
                        walkOrTrot = false;
                    }
                    _shiftTriggerOnce_OneClick = false;
                    _shiftTriggerOnce_Pressed = true;
                    BUTTON_Shift_Up();
                }
        

                if (Bootstrap.LookMode)
                {
                    if (Input.GetMouseButtonDown(0)) BUTTON_MOUSE_0_Down();
                    if (Input.GetMouseButtonUp(0)) BUTTON_MOUSE_0_Up();

                    if (Input.GetMouseButtonDown(1)) BUTTON_MOUSE_1_Down();
                    if (Input.GetMouseButtonUp(1)) BUTTON_MOUSE_1_Up();
                }
            }

            if (isFlying)
            {
                if (isGrounded) CurrentMovementState = !shiftL ? 1 : 2;
                else CurrentMovementState = !shiftL ? 3 : 4;
            }
            else
            {
                if (shiftL) CurrentMovementState = 2; //Running
                else CurrentMovementState = walkOrTrot ? 0 : 1;
            }

            if (_backWalkTimer < Time.time && backWalk && !isFlying) walkOrTrot = true;

            //Movement vectors
            MoveInputForceForAnimation =
                (Vector2.right * Input.GetAxis("Horizontal") + Vector2.up * (Input.GetAxis("Vertical") + (_backWalkTimer < Time.time && backWalk && !isFlying ? 2.0f : 0.0f)) +
                (SRef.Joystick != null ? SRef.Joystick.moveVector : Vector2.zero)).normalized;

            if (movementAllow && sitMovementAllow && chatMovementAllow && inventoryMovementAllow) MoveInputForce = MoveInputForceForAnimation;
            else MoveInputForce = Vector2.zero;

            if (MoveInputForce.magnitude > 0.1f)
            {
                if (isGrounded || isFlying) MovementDirectionVector();
                MovementVelocity = Mathf.Clamp(MovementVelocity + SelfLerpMul.moveVelocitySpeed * Time.deltaTime, 0.0f, 1.0f);
            }
            else MovementVelocity = Mathf.Clamp(MovementVelocity - SelfLerpMul.moveVelocitySpeed * Time.deltaTime, 0.0f, 1.0f);
            if (MovementVelocity < 0.01f)
            {
                isFastRotate = false;
                if (isGrounded || isFlying) motion = Vector3.zero;
            }
            if (MovementVelocity < 0.1f)
            {
                walkOrTrot = walkType;
            }

            //UpdateFunctions
            SetUpSpeedLerp();
            SRef.ModelTransferViews.TerrainNoiseAttentions();
            SRef.ModelTransferViews.Animation();
            SRef.ModelTransferViews.HeadRotation();
            SRef.ModelTransferViews.LerpTransform();

            SRef.ModelTransferViews.hoofs.animDistance = SRef.ModelTransferViews.animDistance;
            SRef.ModelTransferViews.hoofs.speed = SRef.ModelTransferViews.speed;
            SRef.ModelTransferViews.hoofs.downRayDistance = SRef.ModelTransferViews.downRayDistance;
            SRef.ModelTransferViews.hoofs.currentMovementState = Mathf.Clamp(CurrentMovementState, 0, 4);
            SRef.ModelTransferViews.hoofs.forwardDirection = controller.transform.forward;
            SRef.ModelTransferViews.hoofs.movementVelocity = MovementVelocity;
            SRef.ModelTransferViews.hoofs.hoofsDecalGenerator.BackWalk = backWalk;
            SRef.ModelTransferViews.hoofs.UpdateEffects();
        }
    }
}