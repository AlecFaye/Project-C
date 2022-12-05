using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Cinemachine;
using UnityEngine;
using QFSW.QC;

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
        // Player Stats
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed = 1.0f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

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

        // Player Dash Stats
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
        
        // Player Attack Stats
        [Header("Attack")]
        [Tooltip("If the character can Attack or not.")]
        public bool CanAttack = true;

        [Tooltip("If the character is Attacking or not.")]
        public bool IsAttacking = false;
        
        [Tooltip("How long the Attack goes for (float).")]
        public float AttackingTime = 0.8f;
                
        [Tooltip("Attack Cooldown value (float).")]
        public float AttackingCooldown = 1f;

        // Player Ground Variables
        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        // Player Camera Variables
        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

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

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private GameObject _followCamera;
        private CinemachineVirtualCamera _cinemaCamera;
        private Cinemachine3rdPersonFollow _cinemaBody;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsFirstPerson = false;
        //private bool IsThirdPerson = true;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }


        private void Awake()
        {
            // get a reference to our main camera and the player follow camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
                _followCamera = GameObject.FindGameObjectWithTag("PlayerFollowCamera");
                _cinemaCamera = _followCamera.GetComponent<CinemachineVirtualCamera>();
                _cinemaBody = _followCamera.GetComponentInChildren<Cinemachine3rdPersonFollow>();
            }
        }

        private void Start()
        {

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();

            if (IsOwner) { // Checks if you are owner of this Player -- This will run only if your the owner of the character
                _input = GetComponent<StarterAssetsInputs>();
                _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y; // Grabs Something related to rotation speed
                _cinemaCamera.m_Follow = this.transform.GetChild(0).transform; // Will set PlayerCameraRoot (Where the camera should be looking) to be followed by the main camera
                // this.transform.GetChild(0).transform ---> gets the PlayerCameraRoot from the player
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

            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void LateUpdate()
        {
            if (!IsOwner) return; // Checks if you are owner of this Player
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
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

        private void CameraRotation()
        {
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
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
            
        }

        // Function used to swap between 1st person and 3rd person modes (basically edits the PlayerFollowCamera)
        private void OnCameraSwap() {
            if (!IsFirstPerson) { // If not in 1st person mode - sets it to true then changes settings to 1st person mode
                IsFirstPerson = true;

                _cinemaBody.ShoulderOffset = new Vector3(0f, 0.15f, 0.1f);
                _cinemaBody.CameraDistance = (0f);
            }
            else { // Else swaps back to 3rd person mode
                IsFirstPerson = false;

                _cinemaBody.ShoulderOffset = new Vector3(1f, 0f, 0f);
                _cinemaBody.CameraDistance = (4f);
            }
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // Hardcoded camera movement so that the characters head doesn't appear infront of the camera
            if (_input.sprint && IsFirstPerson) _cinemaBody.ShoulderOffset = new Vector3(0f, 0.15f, 0.4f);

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
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

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

        private IEnumerator OnAttack()
        {
            if (CanAttack && !IsAttacking && Grounded && IsOwner){
                CanAttack = false;
                IsAttacking = true;
                _animator.SetTrigger("Attack");
                transform.rotation = Quaternion.Euler(0.0f, _cinemachineTargetYaw, 0.0f); // Rotates the player to where the camera is facing (only on y axis)
                yield return new WaitForSeconds(AttackingTime);
                IsAttacking = false;
                yield return new WaitForSeconds(AttackingCooldown);
                CanAttack = true;
            }
        }


        // Quantum Commands
        //====================================================================================================================================================

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
    }
}