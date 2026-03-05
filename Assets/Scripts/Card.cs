using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Represents a single card on the board.
/// Attach to a GameObject that has a SpriteRenderer and a BoxCollider2D.
/// The card starts face-down showing BackSprite, and flips to FrontSprite when revealed.
/// </summary>
[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class Card : MonoBehaviour
{
    // ── Inspector fields ──────────────────────────────────────────
    [HideInInspector] public int CardId;           // pair identifier (two cards share the same id)
    [HideInInspector] public Sprite FrontSprite;   // the face of this card
    [HideInInspector] public Color CardColor = Color.white; // tint colour for the sprite

    public Sprite BackSprite;                       // shared "card back" image

    // ── Runtime state ─────────────────────────────────────────────
    public bool IsRevealed  { get; private set; }
    public bool IsMatched   { get; private set; }
    public bool IsAnimating { get; private set; }

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D  col;
    private GameManager    gameManager;

    // ── Animation settings ────────────────────────────────────────
    [Header("Flip Animation")]
    public float flipDuration = 0.3f;

    // ── Lifecycle ─────────────────────────────────────────────────
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col            = GetComponent<BoxCollider2D>();
    }

    /// <summary>Call once after instantiation to wire everything up.</summary>
    public void Init(int id, Sprite front, Color color, Sprite back, GameManager gm)
    {
        // Ensure components are resolved even if Awake hasn't fired yet
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (col == null)            col            = GetComponent<BoxCollider2D>();

        CardId      = id;
        FrontSprite = front;
        CardColor   = color;
        BackSprite  = back;
        gameManager = gm;

        IsRevealed = false;
        IsMatched  = false;
        spriteRenderer.sprite = BackSprite;
        spriteRenderer.color  = Color.white;
    }

    // ── Input ─────────────────────────────────────────────────────
    void OnMouseDown()
    {
        if (!IsRevealed && !IsMatched && !IsAnimating && gameManager != null)
        {
            gameManager.CardClicked(this);
        }
    }

    // ── Public API ────────────────────────────────────────────────
    public void Reveal()
    {
        if (IsRevealed) return;
        IsRevealed = true;
        StartCoroutine(AnimateFlip(FrontSprite, CardColor));
    }

    public void Hide()
    {
        if (!IsRevealed) return;
        IsRevealed = false;
        StartCoroutine(AnimateFlip(BackSprite, Color.white));
    }

    public void MatchFound()
    {
        IsMatched  = true;
        IsRevealed = true;
        col.enabled = false;
        spriteRenderer.sprite = FrontSprite;
        spriteRenderer.color  = CardColor;
        StartCoroutine(MatchPop());
    }

    /// <summary>Instantly mark as matched and show the front sprite (used when restoring a saved game).</summary>
    public void RestoreMatched()
    {
        IsMatched  = true;
        IsRevealed = true;
        col.enabled = false;
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = FrontSprite;
        spriteRenderer.color  = CardColor;
    }

    // ── Animations ────────────────────────────────────────────────
    private IEnumerator AnimateFlip(Sprite target, Color targetColor)
    {
        IsAnimating = true;
        float half = flipDuration * 0.5f;

        // Shrink on X
        float t = 0f;
        Vector3 original = transform.localScale;
        while (t < half)
        {
            t += Time.deltaTime;
            float ratio = t / half;
            transform.localScale = new Vector3(
                Mathf.Lerp(original.x, 0f, ratio),
                original.y,
                original.z);
            yield return null;
        }

        // Swap sprite at midpoint
        spriteRenderer.sprite = target;
        spriteRenderer.color  = targetColor;

        // Expand back
        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float ratio = t / half;
            transform.localScale = new Vector3(
                Mathf.Lerp(0f, original.x, ratio),
                original.y,
                original.z);
            yield return null;
        }
        transform.localScale = original;
        IsAnimating = false;
    }

    private IEnumerator MatchPop()
    {
        // Quick scale-up then back down to signal a match
        Vector3 orig = transform.localScale;
        Vector3 big  = orig * 1.2f;
        float dur = 0.15f;

        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(orig, big, t / dur);
            yield return null;
        }
        t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(big, orig, t / dur);
            yield return null;
        }
        transform.localScale = orig;
    }
}
