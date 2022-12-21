using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Cinemachine;
using UnityEngine;
using QFSW.QC;
using Unity.Services.Lobbies.Models;
using UnityEngine.UIElements;
using Mono.CSharp;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets {
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : NetworkBehaviour {
        //s;
        #region Player Variables

        #region Player Movement Stat Variables

        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("Speed of the character during aim in m/s")]
        public float AimSpeed = 1f;

        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed = 1.0f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        #endregion
        
        #region Audio Variables

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        #endregion

        #region Player Jump Stat Variables

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        #endregion

        #region Player Dash Stat Variables

        [Header("Dash")]
        [Tooltip("Trail Renderer.")]
        [SerializeField] public GameObject TR;

        [Tooltip("If the character can Dash or not.")]
        public bool CanDash = true;

        [Tooltip("If the character is Dashing or not.")]
        public bool IsDashing = false;
        
        [Tooltip("Dash intial power (float).")]
        public float DashingPower = 24f;

        [Tooltip("Gravity during Dash (float).")]
        public float DashingGravity = 0.0f;
                
        [Tooltip("How long the Dash goes for (float).")]
        public float DashingTime = 0.2f;
                
        [Tooltip("Dash Cooldown value (float).")]
        public float DashingCooldown = 1f;

        #endregion

        #region Player Ground Stat Variables

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        #endregion

        #region Player Camera Variables
        
        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        [SerializeField] private Transform cameraRoot;

        [Tooltip("How far in degrees can you move the camera up (1st Person Mode)")]
        public float First_TopClamp = 89.0f;

        [Tooltip("How far in degrees can you move the camera down (1st Person Mode)")]
        public float First_BottomClamp = -89.0f;

        [Tooltip("How far in degrees can you move the camera up (3rd Person Mode)")]
        public float Third_TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down (3rd Person Mode)")]
        public float Third_BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        #endregion

        #region Player Aim Variables
        
        [Header("Attack/Camera-Aim Variables")]
        [Tooltip("Checks if the player is Attacking or not")]
        public bool IsAttacking = false;
        
        [Tooltip("Checks if the player's aim is constant or not")]
        public bool IsConstantAim = false;
        
        [Tooltip("Checks if the player's aim is constant or not")]
        public Vector3 aimTarget = Vector3.zero;
    
        [Tooltip("Reuturns the player's aim is constant or not")]
        public Vector3 mouseWorldPosition = Vector3.zero;

        [Tooltip("Layer Mask for aiming")]
        [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();

        #endregion

        #region Player Animation Parameter Variables

        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        // My variables Below
        private int _animIDBowStartAim;
        private int _animIDTomeStartAim;
        private int _animIDAiming;

        #endregion

        #region LOL No Clue what these are for

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        #if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
        #endif
        public Animator _animator; // change to private and fix errors later
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private CinemachineVirtualCamera _noAimCamera;
        private CinemachineVirtualCamera _bowAimCamera;
        private CinemachineVirtualCamera _tomeAimCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        #endregion

        #region Additional Transforms/GameObjects

        public Transform MouseWorldTransform; // Gets the position of where the player is aiming 
        
        public GameObject HotbarContoller; // Lets the editor grab the Hotbar Controller

        public Transform Self;

        #endregion

        #endregion

        #region (Awake, Start, Update) Functions

        private bool IsCurrentDeviceMouse {
            get {
            #if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                return _playerInput.currentControlScheme == "KeyboardMouse";
            #else
                return false;
            #endif
            }
        }

        private void Awake() {
            // get a reference to our main camera and the player follow camera
            if (_mainCamera == null) {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
                _noAimCamera = GameObject.FindGameObjectWithTag("NoAimCamera").GetComponent<CinemachineVirtualCamera>();
                _bowAimCamera = GameObject.FindGameObjectWithTag("BowAimCamera").GetComponent<CinemachineVirtualCamera>();
                _tomeAimCamera = GameObject.FindGameObjectWithTag("TomeAimCamera").GetComponent<CinemachineVirtualCamera>();
            }
        }

        private void Start()
        {
            if (IsOwner) { // Checks if you are owner of this Player -- This will run only if your the owner of the character
                _hasAnimator = TryGetComponent(out _animator);
                _controller = GetComponent<CharacterController>();
                _input = GetComponent<StarterAssetsInputs>();
                _cinemachineTargetYaw = cameraRoot.rotation.eulerAngles.y; // Grabs Something related to rotation speed
                // vvv Will set PlayerCameraRoot (Where the camera should be looking) to be followed by the main camera
                _noAimCamera.m_Follow = cameraRoot;
                _bowAimCamera.m_Follow = cameraRoot;
                _tomeAimCamera.m_Follow = cameraRoot;
            }


#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            if (!IsOwner) return; // Checks if you are owner of this Player

            _hasAnimator = TryGetComponent(out _animator);

            RaycastMouse();

            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void LateUpdate()
        {
            if (!IsOwner) return; // Checks if you are owner of this Player
            CameraRotation();
        }

        #endregion

        private void AssignAnimationIDs()
        {
            if (!IsOwner) return; // Checks if you are owner of this Player
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Is Grounded");
            _animIDJump = Animator.StringToHash("Is Jumping");
            _animIDFreeFall = Animator.StringToHash("Is FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDBowStartAim = Animator.StringToHash("Bow Aim");
            _animIDTomeStartAim = Animator.StringToHash("Tome Aim");
            _animIDAiming = Animator.StringToHash("IsAiming");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        #region Camera Stuff (Camera and Player rotations)
        
        private void CameraRotation() {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition) {
                //Don't multiply mouse input by Time.deltaTime;
                //float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
                float deltaTimeMultiplier = 1.0f; // Fixed an issue where new players added would have their IsCurrentDeviceMouse equal to false so sensitivity value was extremely low causeing no movement

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, Third_BottomClamp, Third_TopClamp);

            // Cinemachine will follow this target
            cameraRoot.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }

        private void RaycastMouse() {
            Vector2 screenCentrePoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = Camera.main.ScreenPointToRay(screenCentrePoint);

            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask)) {
                mouseWorldPosition = raycastHit.point;
                MouseWorldTransform.position = mouseWorldPosition;
            }

            if (!IsAttacking) return;

            if (IsConstantAim)
                PlayerRotateToAim(mouseWorldPosition);
            else
                PlayerRotateToAim(aimTarget);
        }

        public void PlayerRotateToAim(Vector3 worldAimTarget) {
            worldAimTarget.y = Self.position.y;
            
            Vector3 aimDirection = (worldAimTarget - Self.position).normalized;
            Self.forward = Vector3.Lerp(Self.forward, aimDirection, Time.deltaTime * 20f);
        }

        // {P.S. maybe the Tome can zoom a little while charging} || reduce the sway to zero || Have player move slower while they do this aswell
        public void TriggerAim(float aimTime, Weapon.WeaponType weapon) {
            switch (weapon) {
                case Weapon.WeaponType.Bow:
                    _animator.SetTrigger(_animIDBowStartAim);
                    _bowAimCamera.gameObject.SetActive(IsAttacking);
                    break;
                case Weapon.WeaponType.Tome:
                    _animator.SetTrigger(_animIDTomeStartAim);
                    _tomeAimCamera.gameObject.SetActive(IsAttacking);
                    break;
            }
            
            Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = aimTime; // Sets the main camera to a slower or faster zoom depending on required speed
            
            _noAimCamera.gameObject.SetActive(!IsAttacking);
        }

        #endregion

        #region Movement and Jump Functions
        
        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero && !IsDashing) targetSpeed = 0.0f;
            
            if (IsDashing) targetSpeed = DashingPower; 
            
            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                if (!IsAttacking) 
                    // rotate to face input direction relative to camera position
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        #endregion

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (!IsOwner) return; // Checks if you are owner of this Player

            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (!IsOwner) return; // Checks if you are owner of this Player

            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        private IEnumerator OnDash()
        {
            if (CanDash && !IsDashing && IsOwner){
                CanDash = false;
                IsDashing = true;
                float originalGravity = Gravity;
                Gravity = DashingGravity;
                TR.GetComponent<TrailRenderer>().emitting = true;
                yield return new WaitForSeconds(DashingTime);
                TR.GetComponent<TrailRenderer>().emitting = false;
                Gravity = originalGravity;
                IsDashing = false;
                yield return new WaitForSeconds(DashingCooldown);
                CanDash = true;
            }
        }

       
        #region Quantum Commands
        [Command("Player.Set_Move_Speed")]
        private void SetMoveSpeed(float newValue) {
            MoveSpeed = newValue;
        }
        
        [Command("Player.Set_Sprint_Speed")]
        private void SetSprintSpeed(float newValue) {
            SprintSpeed = newValue;
        }

        [Command("Player.Set_Acceleration")]
        private void SetSpeedChangeRate(float newValue) {
            SpeedChangeRate = newValue;
        }
        
        [Command("Player.Set_Jump_Height")]
        private void SetJumpHeight(float newValue) {
            JumpHeight = newValue;
        }

        [Command("Player.Set_Gravity")]
        private void SetGravity(float newValue) {
            Gravity = newValue;
        }

        [Command("Player.Set_Jump_CD_Timer")]
        private void SetJumpTimeout(float newValue) {
            JumpTimeout = newValue;
        }

        [Command("Player.Set_Fall_Start_Timer")]
        private void SetFallTimeout(float newValue) {
            FallTimeout = newValue;
        }

        [Command("Player.Get_All_Stats")]
        private void GetStats() {
            Debug.Log("Save the following via screenshot and post it once you like the movement");
            Debug.Log("Move Speed: " + MoveSpeed);
            Debug.Log("Sprint Speed: " + SprintSpeed);
            Debug.Log("Acceleration: " + SpeedChangeRate);
            Debug.Log("Jump Height: " + JumpHeight);
            Debug.Log("Gravity: " + Gravity);
            Debug.Log("Jump CD Timer: " + JumpTimeout);
            Debug.Log("Fall Start Timer: " + FallTimeout);
        }
        #endregion
    }
}