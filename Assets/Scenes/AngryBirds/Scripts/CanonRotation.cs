using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CanonRotation : MonoBehaviour, InputSystem_Actions.ICannonActions
{
    public Vector3 _maxRotation;
    public Vector3 _minRotation;
    private float _offset = -51.6f;
    public GameObject ShootPoint;
    public GameObject Bullet;
    public float ProjectileSpeed = 0;
    public float MaxSpeed;
    public float MinSpeed;
    public GameObject PotencyBar;
    private float _initialScaleX;
    private Vector2 _distanceBetweenMouseAndPlayer;
    private bool isRaising = false;
    public GameObject CannonPivot;
    private Camera camera;
    private InputSystem_Actions inputActions;
    [SerializeField] private float _multiplier = 10f;
    private void Awake()
    {
        _initialScaleX = PotencyBar.transform.localScale.x;
        camera = Camera.main;

        inputActions = new InputSystem_Actions();
        inputActions.Cannon.SetCallbacks(this);
    }
    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }
    void Update()
    {
        Vector2 mousePos = inputActions.Cannon.Aim.ReadValue<Vector2>(); //obtenir el valor del click del cursor (Fer amb new input system)
        _distanceBetweenMouseAndPlayer = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y); //obtenir el vector distància entre el canó i el cursor
        var ang = Mathf.Atan2(_distanceBetweenMouseAndPlayer.y, _distanceBetweenMouseAndPlayer.x) * Mathf.Rad2Deg;
        var fixedAng = Mathf.Clamp(ang, _minRotation.z, _maxRotation.z);
        CannonPivot.transform.rotation = Quaternion.Euler(0,0,ang); //en quin dels tres eixos va l'angle?

        if (isRaising)
        {
            ProjectileSpeed = Time.deltaTime * _multiplier + ProjectileSpeed; //acotar entre dos valors (mirar variables)
            ProjectileSpeed = Mathf.Clamp(ProjectileSpeed, MinSpeed, MaxSpeed);
            CalculateBarScale();
        }
        
        CalculateBarScale();

    }
    public void CalculateBarScale()
    {
        PotencyBar.transform.localScale = new Vector3(Mathf.Lerp(0, _initialScaleX, ProjectileSpeed / MaxSpeed),
            transform.localScale.y,
            transform.localScale.z);
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
		if (context.started)
		{
			isRaising = true;
		}
		if (context.canceled)
		{
			var projectile = Instantiate(Bullet, ShootPoint.transform.position, Quaternion.identity); //canviar la posició on s'instancia
            var dirNormalized = _distanceBetweenMouseAndPlayer.normalized;
            projectile.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(dirNormalized.x * ProjectileSpeed, dirNormalized.y * ProjectileSpeed);
			ProjectileSpeed = 0f;
			isRaising = false;
		}
	}

    public void OnAim(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
    }
    /*public void OnLeftClick(InputAction.CallbackContext context)
{
   if (context.started)
   {
       isRaising = true;
   }
   if (context.canceled)
   {
       var projectile = Instantiate(Bullet, transform.position, Quaternion.identity); //canviar la posició on s'instancia
       projectile.GetComponent<Rigidbody2D>().linearVelocity =
       ProjectileSpeed = 0f;
       isRaising = false;
   }
}*/
}
