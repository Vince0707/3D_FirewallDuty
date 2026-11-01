using UnityEngine;

// A person walking up and down a pavement, for the street outside the office (made by Tools > Firewall Duty > Office > Build Entrance Street). The
// street has no NavMesh, so this simply walks a straight line between two points: when it reaches one end it stops for a moment, turns around
// and walks back. The walking animation is the office's own (the Animator's "IsWalking" switch).
//
// The weather changes how people look and move (ApplyWeatherToStreet, called when the arrival starts):
//   clear  everyone walks normally, and the joggers are out running
//   rain   everyone has an umbrella (the right hand holds its handle), hurries, and hardly stops
//   snow   everyone wears a beanie and a scarf, and walks slowly and carefully
// The umbrellas, beanies and scarves are children named Weather_Umbrella, Weather_Beanie and Weather_Scarf (switched off in the scene), made by the setup tool.
[RequireComponent(typeof(Animator))]
public class Pedestrian : MonoBehaviour
{
    [SerializeField] private Vector3 pointA;
    [SerializeField] private Vector3 pointB;
    [SerializeField, Min(0.2f)] private float speed = 1.3f;
    [Tooltip("Where on the way from A to B this person is when the scene starts (0 = at A, 1 = at B).")]
    [SerializeField, Range(0f, 1f)] private float startAt = 0f;
    [SerializeField, Min(0f)] private float pauseMin = 0.5f;
    [SerializeField, Min(0f)] private float pauseMax = 2.5f;
    [SerializeField] private string walkingParameter = "IsWalking";

    [Header("Jogger")]
    [Tooltip("Runs instead of walking, and is only in the street on a clear day (it is switched off in the scene).")]
    [SerializeField] private bool jogger = false;
    [SerializeField] private string runningParameter = "IsRunning";
    [Tooltip("How fast the running animation plays (the clip is a sprint; a jogger is slower).")]
    [SerializeField, Range(0.4f, 1.2f)] private float runAnimationSpeed = 0.8f;

    private const string UmbrellaName = "Weather_Umbrella";
    private const string BeanieName = "Weather_Beanie";
    private const string ScarfName = "Weather_Scarf";

    private Animator animator;
    private bool hasParameter;
    private bool goingToB = true;
    private float pause;
    private float speedFactor = 1f;
    private float pauseFactor = 1f;

    public void Setup(Vector3 a, Vector3 b, float walkSpeed, float start)
    {
        pointA = a;
        pointB = b;
        speed = walkSpeed;
        startAt = start;
    }

    // Makes this person a jogger: runs, hardly stops at the ends, and is only there on a clear day.
    public void SetJogger()
    {
        jogger = true;
        pauseMin = 0f;
        pauseMax = 0.4f;
    }

    // ---- The weather -----------------------------------------------------------------------------------------------------------

    // 0 = clear, 1 = rain, 2 = snow (the same numbers as MissionIntro.Variant). Safe to call before the person has started (also while switched off).
    public static void ApplyWeatherToStreet(int variant)
    {
        foreach (Pedestrian person in FindObjectsByType<Pedestrian>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            person.ApplyWeather(variant);

            if (person.jogger)
                person.gameObject.SetActive(variant == 0);
        }
    }

    public void ApplyWeather(int variant)
    {
        bool rain = variant == 1;
        bool snow = variant == 2;

        speedFactor = rain ? 1.3f : snow ? 0.75f : 1f;
        pauseFactor = rain ? 0.4f : 1f;

        SetProp(UmbrellaName, rain);
        SetProp(BeanieName, snow);
        SetProp(ScarfName, snow);

        // The right hand holds the umbrella's handle (HandIK, off unless it rains).
        HandIK hand = GetComponent<HandIK>();
        if (hand != null)
            hand.enabled = rain;
    }

    private void SetProp(string propName, bool on)
    {
        // The umbrella is a child of the person, but the beanie and the scarf are children of the head and neck bones, so the search goes all the way
        // down (Transform.Find only looks at the direct children), and into things that are switched off.
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name == propName)
            {
                child.gameObject.SetActive(on);
                return;
            }
        }
    }

    // ---- Walking ---------------------------------------------------------------------------------------------------------------

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.applyRootMotion = false;

            string wanted = jogger ? runningParameter : walkingParameter;
            foreach (AnimatorControllerParameter p in animator.parameters)
            {
                if (p.name == wanted && p.type == AnimatorControllerParameterType.Bool)
                    hasParameter = true;
            }

            if (jogger)
                animator.speed = runAnimationSpeed;
        }
    }

    private void Start()
    {
        transform.position = Vector3.Lerp(pointA, pointB, startAt);
        goingToB = Random.value < 0.5f;
        Face(goingToB ? pointB : pointA);
        SetWalking(true);
    }

    private void Update()
    {
        if (pause > 0f)
        {
            pause -= Time.deltaTime;
            if (pause <= 0f)
            {
                goingToB = !goingToB;
                SetWalking(true);
            }

            return;
        }

        Vector3 target = goingToB ? pointB : pointA;
        Vector3 toTarget = target - transform.position;
        toTarget.y = 0f;

        float step = speed * speedFactor * Time.deltaTime;
        if (toTarget.magnitude <= step)
        {
            // At the end of the walk: stop for a moment, then turn around.
            transform.position = new Vector3(target.x, transform.position.y, target.z);
            pause = Random.Range(pauseMin, Mathf.Max(pauseMin, pauseMax)) * pauseFactor;
            SetWalking(false);
            return;
        }

        transform.position += toTarget.normalized * step;

        Quaternion look = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, 8f * Time.deltaTime);
    }

    private void Face(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
    }

    private void SetWalking(bool walking)
    {
        if (animator != null && hasParameter)
            animator.SetBool(jogger ? runningParameter : walkingParameter, walking);
    }
}
