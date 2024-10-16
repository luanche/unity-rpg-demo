using System;
using UnityEngine;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class SwordSkill : Skill
{
    public SwordType swordType = SwordType.Regular;

    [Header("Skill info")]
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float returnSpeed;

    [Header("Bounce info")]
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bounceSpeed;

    [Header("Peirce info")]
    [SerializeField] private int peirceAmount;
    [SerializeField] private float peirceGravity;

    [Header("Spin info")]
    [SerializeField] private float hitCoolDown;
    [SerializeField] private float maxTravelDistance;
    [SerializeField] private float spinDuration;
    [SerializeField] private float spinGravity;

    private Vector2 finalDir;

    [Header("Aim dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotsParent;

    private GameObject[] dots;

    protected override void Start()
    {
        base.Start();
        GenerateDots();
    }

    private void SetupGravity()
    {
        switch (swordType)
        {
            case SwordType.Bounce:
                swordGravity = bounceGravity;
                break;
            case SwordType.Pierce:
                swordGravity = peirceGravity;
                break;
            case SwordType.Spin: 
                swordGravity = spinGravity;
                break;
            default:
                break;
        }
    }
     
    protected override void Update()
    {
        base.Update();
        SetupGravity();
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            finalDir = AimDirection() * launchForce;
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
            }
        }
    }

    public void CreateSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        SwordSkillController controller = newSword.GetComponent<SwordSkillController>();

        switch (swordType)
        {
            case SwordType.Bounce:
                controller.SetupBounce(true, bounceAmount, bounceSpeed);
                break;
            case SwordType.Pierce:
                controller.SetupPierce(peirceAmount);
                break;
            case SwordType.Spin:
                controller.SetupSpin(true, maxTravelDistance, spinDuration, hitCoolDown);
                break;
            default:
                break;
        }

        controller.SetupSword(finalDir, swordGravity, player, freezeTimeDuration, returnSpeed);

        player.AssignNewSword(newSword);

        DotsActive(false);
    }

    #region Aim
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePostion = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return (mousePostion - playerPosition).normalized;
    }

    public void DotsActive(bool _isActive)
    {
        foreach (var dot in dots)
        {
            dot.SetActive(_isActive);
        }
    }

    public void GenerateDots()
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float t)
    {
        return (Vector2)player.transform.position +
            (AimDirection() * launchForce) * t +
            .5f * (Physics2D.gravity * swordGravity) * (t * t);
    }
    #endregion
}